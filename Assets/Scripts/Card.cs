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


	public void Setup(CardID ID, GameManager gm) {
		template = ID.GetTemplate();

		// setup visuals
		titleText.text = template.cardName;
		descriptionText.text = template.description;

		isConcealed = false;

		mouseTargetable = GetComponent<MouseTargetable>();
		mouseTargetable.SetTargetingGroup(TargetingGroup.CardInMyHand);

		mouseDraggable = GetComponent<MouseDraggable>();
		mouseDraggable.OnPickUp.Add(this.OnPickUp);
		mouseDraggable.OnDrop.Add(this.OnDrop);

		this.gm = gm;

		//Debug.Log("ID: " + actorID);
	}

	/// When the enemy draws a card, this client shouldn't know what it is
	public void ConcealedSetup(GameManager gm) {
		isConcealed = true;
		FlipFaceDown(false);

		titleText.text = "Concealed";
		descriptionText.text = "Concealed";

		mouseTargetable = GetComponent<MouseTargetable>();
		mouseTargetable.SetTargetingGroup(TargetingGroup.CardInEnemyHand);

		this.gm = gm;
	}

	/// Flip face up, and unconceal + setup card details if the card is currently conceal
	public void Reveal(CardID ID) {
		// if the card is already revealed, do nothing
		// this is for the benefit of the player who drew the card
		if (isConcealed) {
			Setup(ID, gm);
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

		MouseTargetable.SetActiveTargetingGroup(TargetingGroup.CardInMyHand);
	}

	void ReturnToMyHand() {
		// move this back to the hand
		GameObject.Find("MyHand").GetComponent<Hand>().CorrectCardPositions();
		mouseTargetable.SetTargetingGroup(TargetingGroup.CardInMyHand);
	}

	/// Called by PlayCardAction
	public void AddToCardSlot(CardSlot cardSlot) {
		// parent to the card slot and move to it
		this.transform.SetParent(cardSlot.transform);
		this.transform.DOLocalMove(Vector2.zero, 0.1f);

		cardSlot.AddCard(this);

		mouseTargetable.SetTargetingGroup(TargetingGroup.CardOnBoard);

		hand.RemoveFromHand(this);
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
}