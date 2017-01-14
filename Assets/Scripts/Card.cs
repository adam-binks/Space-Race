using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(MouseTargetable))]
[RequireComponent(typeof(MouseDraggable))]
public class Card : ActionActor {

	public Text titleText;
	public Text descriptionText;
	public Text costText;
	[HeaderAttribute("Juice")]
	public float flipDuration = 0.1f;
	public float cardSlotDropLaxness = 0.2f; // when the card is dropped, look for cardSlots in a sphere of this radius
	public Hand hand;

	private CardTemplate template;
	private bool isCharged = false;
	/// Cards are concealed when the enemy draws them, this client shouldn't know what they are
	private bool isConcealed;
	private MouseTargetable mouseTargetable;
	private MouseDraggable mouseDraggable;
	private GameManager gm;
	private int playerNumWhoPlayedThis;


	public void Setup(CardID ID, GameManager gm, int playerNumWhoPlayedThis) {
		this.gm = gm;
		this.playerNumWhoPlayedThis = playerNumWhoPlayedThis;

		template = ID.GetTemplate();

		// setup visuals
		titleText.text = template.cardName;
		descriptionText.text = template.description;
		costText.text = template.playCost.ToString();

		isConcealed = false;

		mouseTargetable = GetComponent<MouseTargetable>();
		mouseDraggable = GetComponent<MouseDraggable>();
		mouseDraggable.OnPickUp.Add(this.OnPickUp);
		mouseDraggable.OnDrop.Add(this.OnDrop);

		UpdateTargetingGroupForPlayability();
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
		this.playerNumWhoPlayedThis = playerNumWhoPlayedThis;
	}

	/// Flip face up, and unconceal + setup card details if the card is currently conceal
	public void Reveal(CardID ID) {
		// if the card is already revealed, do nothing
		// this is for the benefit of the player who drew the card
		if (isConcealed) {
			Setup(ID, gm, playerNumWhoPlayedThis);
			FlipFaceUp(true);
		}
	}

	public cardCategory GetCategory() {
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
		if (template.cat == cardCategory.Operative) {
			emptyTG = TargetingGroup.EmptyOperativeSlot;
		} else if (template.cat == cardCategory.Policy) {
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

		MouseTargetable.SetActiveTargetingGroup(TargetingGroup.CardInMyHandPlayable);
	}

	void ReturnToMyHand() {
		// move this back to the hand
		GameObject.Find("MyHand").GetComponent<Hand>().CorrectCardPositions();
		mouseTargetable.SetTargetingGroup(TargetingGroup.CardInMyHandPlayable);
	}

	/// Called by PlayCardAction
	public void AddToCardSlot(CardSlot cardSlot) {
		// parent to the card slot and move to it
		this.transform.SetParent(cardSlot.transform);
		this.transform.DOLocalMove(Vector2.zero, 0.1f);

		cardSlot.AddCard(this);

		mouseTargetable.SetTargetingGroup(TargetingGroup.CardOnBoard);

		hand.RemoveFromHand(this);

		PlayerFunds funds = playerNumWhoPlayedThis == gm.localPlayerNum ? gm.myFunds : gm.enemyFunds;
		funds.deductFromFunds(template.playCost);
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

	/// This card should only be interactable if it is currently playable (can afford + is a free slot)
	public void UpdateTargetingGroupForPlayability() {
		if (IsPlayable()) {
			mouseTargetable.SetTargetingGroup(TargetingGroup.CardInMyHandPlayable);
		} else {
			mouseTargetable.SetTargetingGroup(TargetingGroup.CardInMyHandUnplayable);
		}
	}

	bool IsPlayable() {
		// check if the player can afford this card
		if (!gm.myFunds.canAfford(template.playCost)) {
			return false;
		}
		
		// check there's a free slot
		if (template.cat == cardCategory.Operative) {
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
}