using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum CardStatus {
	InHand,
	OnBoard
}


[RequireComponent(typeof(MouseTargetable))]
[RequireComponent(typeof(MouseDraggable))]
public class Card : ActionActor {

	public Text titleText;
	public Text descriptionText;
	public Text costText;
	public GameObject chargeableInterface;
	public Text chargeableText;
	[HeaderAttribute("Juice")]
	public float flipDuration = 0.1f;
	public float cardSlotDropLaxness = 0.2f; // when the card is dropped, look for cardSlots in a sphere of this radius
	public Hand hand;

	private CardTemplate template;
	/// Cards are concealed when the enemy draws them, this client shouldn't know what they are
	private bool isConcealed;
	private MouseTargetable mouseTargetable;
	private MouseDraggable mouseDraggable;
	private GameManager gm;
	private CardStatus status = CardStatus.InHand;
	private int tempPlayerNum;


	public void Setup(CardID ID, GameManager gm, int playerNumWhoPlayedThis) {
		this.gm = gm;
		
		template = ID.GetTemplate();
		template.authorPlayer = playerNumWhoPlayedThis;


		// setup visuals
		titleText.text = template.cardName;
		descriptionText.text = template.description;
		costText.text = template.playCost.ToString();

		isConcealed = false;

		mouseTargetable = GetComponent<MouseTargetable>();
		mouseTargetable.OnMouseUpAsButtonActions.Add(OnClick);

		mouseDraggable = GetComponent<MouseDraggable>();
		mouseDraggable.OnPickUp.Add(this.OnPickUp);
		mouseDraggable.OnDrop.Add(this.OnDrop);

		UpdateTargetingGroupForPlayability();

		if (template.isChargeable) {
			chargeableInterface.SetActive(true);
			SetIsCharged(false);
		} else {
			chargeableInterface.SetActive(false);
		}
	}

	/// When the enemy draws a card, this client shouldn't know what it is
	public void ConcealedSetup(GameManager gm, int playerNumWhoPlayedThis) {
		isConcealed = true;
		FlipFaceDown(false);

		titleText.text = "Concealed";
		descriptionText.text = "Concealed";

		mouseTargetable = GetComponent<MouseTargetable>();
		mouseTargetable.SetTargetingGroup(TargetingGroup.CardInEnemyHand);

		this.gm = gm;
		tempPlayerNum = playerNumWhoPlayedThis;
	}

	/// Flip face up, and unconceal + setup card details if the card is currently concealed
	public void Reveal(CardID ID) {
		// if the card is already revealed, do nothing
		// this is for the benefit of the player who drew the card
		if (isConcealed) {
			Setup(ID, gm, tempPlayerNum);
			FlipFaceUp(true);
		}
	}

	public void OnTurnStart() {
		SetIsCharged(false);
	}

	public CardCategory GetCategory() {
		return template.cat;
	}

	public string GetName() {
		return template.cardName;
	}

	public void FlipFaceUp(bool tween = true) {
		if (tween) {
			transform.DORotate(Vector3.zero, flipDuration);
		} else {
			transform.rotation = Quaternion.identity;
		}
	}

	public void FlipFaceDown(bool tween = true) {
		if (tween) {
			transform.DORotate(new Vector3(0, 180, 0), flipDuration);
		} else {
			transform.rotation = Quaternion.Euler(0, 180, 0);
		}
	}

	void OnPickUp() {
		// make valid card slots targetable
		TargetingGroup emptyTG = TargetingGroup.NOT_ASSIGNED;
		if (template.cat == CardCategory.Operative) {
			emptyTG = TargetingGroup.EmptyOperativeSlot;
		} else if (template.cat == CardCategory.Policy) {
			emptyTG = TargetingGroup.EmptyPolicySlot;
		}

		MouseTargetable.SetActiveTargetingGroups(new List<TargetingGroup> {
			TargetingGroup.DraggedObject,
			emptyTG
		});
	}

	void OnDrop() {
		CardSlot cardSlot = GetCollidedCardSlot();
		if (cardSlot != null && cardSlot.CardIsValid(this)) {
			// reveal this card to both players, then play it to the board
			gm.actionQueue.AddAction(new RevealCardAction(this.actorID, template.ID.GetID()));
			gm.actionQueue.AddAction(new PlayCardAction(this.actorID, cardSlot.actorID));

		} else {
			ReturnToMyHand();
		}

		MouseTargetable.SetActiveTargetingGroups(new List<TargetingGroup> {
			TargetingGroup.CardInMyHandPlayable,
			TargetingGroup.CardOnBoardChargeable });
	}

	void ReturnToMyHand() {
		// move this back to the hand
		gm.myHand.CorrectCardPositions();
		mouseTargetable.SetTargetingGroup(TargetingGroup.CardInMyHandPlayable);
		status = CardStatus.InHand;
	}

	/// Called by PlayCardAction
	public void AddToCardSlot(CardSlot cardSlot) {
		// parent to the card slot and move to it
		this.transform.SetParent(cardSlot.transform);
		this.transform.DOLocalMove(Vector2.zero, 0.1f);

		cardSlot.AddCard(this);
		hand.RemoveFromHand(this);
		status = CardStatus.OnBoard;

		PlayerFunds funds = (template.authorPlayer == gm.localPlayerNum) ? gm.myFunds : gm.enemyFunds;
		funds.deductFromFunds(template.playCost);

		UpdateTargetingGroupForChargeability();
		mouseDraggable.isDraggable = false; // disable dragging
	}

	CardSlot GetCollidedCardSlot() {
		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, cardSlotDropLaxness);
		foreach (Collider2D col in hitColliders) {
			if (col.tag == "CardSlot") {
				return col.GetComponent<CardSlot>();
			}
		}
		return null; // no collided card slot found
	}

	/// This card in hand should only be interactable if it is currently playable (can afford + is a free slot)
	public void UpdateTargetingGroupForPlayability() {
		if (IsPlayable()) {
			mouseTargetable.SetTargetingGroup(TargetingGroup.CardInMyHandPlayable);
		} else {
			mouseTargetable.SetTargetingGroup(TargetingGroup.CardInMyHandUnplayable);
		}
	}

	bool IsPlayable() {
		if (status != CardStatus.InHand) {
			Debug.LogError("Shouldn't be checking whether a card is playable when it isn't in hand!", this);
			return false;
		}

		// check if the player can afford this card
		if (!gm.myFunds.canAfford(template.playCost)) {
			return false;
		}
		
		// check there's a free slot
		if (template.cat == CardCategory.Operative) {
			if (!gm.slotManager.IsFreeOperativeSlot()) {
				return false;
			}
		} else {
			if (!gm.slotManager.IsFreePolicySlot()) {
				return false;
			}
		}

		// all checks passed
		return true;
	}

	public void UpdateTargetingGroupForChargeability() {
		if (IsChargeable()) {
			mouseTargetable.SetTargetingGroup(TargetingGroup.CardOnBoardChargeable);
		} else {
			mouseTargetable.SetTargetingGroup(TargetingGroup.CardOnBoardUnchargeable);
		}
	}

	bool IsChargeable() {
		if (status != CardStatus.OnBoard) {
			Debug.LogError("Cannot check chargeability, this card isn't on board!", this);
			return false;
		}

		if (template.isChargeable == false || template.isCharged) {
			return false;
		}

		if (!gm.myFunds.canAfford(template.chargeCost)) {
			Debug.Log("Can't afford to charge this card", this);
			return false;
		}

		return true;
	}

	void OnClick() {
		if (status == CardStatus.OnBoard) {
			StartCharge();
		}
	}

	/// Try and start charging this card if its possible
	void StartCharge() {
		if (template.isCharged) {
			Debug.Log("Can't charge a card which has already been charged this turn");
			return;
		}
		
		// Can the player afford to charge this card?
		if (!gm.myFunds.canAfford(template.chargeCost)) {
			Debug.Log("Not enough funds to charge!", this);
			return;
		}

		if (template.cat == CardCategory.Policy) {
			gm.actionQueue.AddAction(new ChargePolicyCardAction(this.actorID));
		} else {
			// TODO operative charging
		}
	}

	/// Called by ChargePolicyCardAction
	public void ChargePolicy() {
		if (status == CardStatus.InHand) {
			Debug.LogError("Cannot charge a card in hand!", this);
			return;
		}

		SetIsCharged(true);

		PlayerFunds funds = (template.authorPlayer == gm.localPlayerNum) ? gm.myFunds : gm.enemyFunds;
		funds.deductFromFunds(template.chargeCost);
	}

	void SetIsCharged(bool newIsCharged) {
		if (newIsCharged) {
			chargeableText.text = "Charged";
		} else {
			chargeableText.text = "Charge (" + template.chargeCost.ToString() + ")";
		}
		template.isCharged = newIsCharged;
		if (status == CardStatus.OnBoard) {
			UpdateTargetingGroupForChargeability();
		}
	}
}