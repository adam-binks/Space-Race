using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardSlot : ActionActor {

	public Sprite policySlotSprite;
	public Sprite operativeSlotSprite;
	bool animateIn = true;

	private Card card = null;
	private CardCategory requiredCat = CardCategory.None;
	private bool isMine;

	public void Setup(CardCategory requiredCardCategory, bool isMine) {
		this.requiredCat = requiredCardCategory;

		// setup slot sprite
		if (requiredCat == CardCategory.Operative) {
			GetComponentInChildren<SpriteRenderer>().sprite = operativeSlotSprite;
			GetComponentInChildren<MouseTargetable>().SetTargetingGroup(TargetingGroup.EmptyOperativeSlot);
		} else {
			GetComponentInChildren<SpriteRenderer>().sprite = policySlotSprite;
			GetComponentInChildren<MouseTargetable>().SetTargetingGroup(TargetingGroup.EmptyPolicySlot);
			GetComponentInChildren<MouseTargetable>().SetTargetingGroup(TargetingGroup.EmptyPolicySlot);
		}

		this.isMine = isMine; // is this a local or enemy operative slot?

		if (animateIn) {
			Vector2 scale = transform.localScale;
			transform.localScale = Vector2.zero;
			transform.DOScale(scale, 0.2f);
		}
	}
	
	public bool IsOccupied() {
		return card == null;
	}

	public void AddCard(Card c) {
		if (IsOccupied()) {
			Debug.LogError("Cannot add card to occupied card slot!");
			return;
		}
		if (requiredCat != CardCategory.None && requiredCat != c.GetCategory()) {
			Debug.LogError("Cannot add card of type " + c.GetCategory() + " to this card slot", this);
			return;
		}
		card = c;

		c.transform.SetParent(this.transform);
		c.transform.DOMove(transform.position, 0.1f);

		// set the MouseTargetable TargetingGroup to the filled version of this card slot
		TargetingGroup filled;
		if (requiredCat == CardCategory.Operative) {
			filled = TargetingGroup.FilledOperativeSlot;
		} else {
			filled = TargetingGroup.FilledPolicySlot;
		}
		GetComponentInChildren<MouseTargetable>().SetTargetingGroup(filled);
	}

	public Card RemoveCard() {
		Card c = card;
		card = null;

		// set the MouseTargetable TargetingGroup to the empty version of this card slot
		TargetingGroup empty;
		if (requiredCat == CardCategory.Operative) {
			empty = TargetingGroup.EmptyOperativeSlot;
		} else {
			empty = TargetingGroup.EmptyPolicySlot;
		}
		GetComponentInChildren<MouseTargetable>().SetTargetingGroup(empty);

		return c;
	}

	bool CardIsValid(Card c) {
		return !IsOccupied()  &&  c.GetCategory() == requiredCat;
	}

	void OnMouseUpAndActive() {
		// check if a card was just dropped on this card slot
		if (MouseTargetable.justDroppedCard != null && CardIsValid(MouseTargetable.justDroppedCard)) {
			AddCard(MouseTargetable.justDroppedCard);
			// fix the hand so there's no gap
			GameObject.Find("MyHand").GetComponent<Hand>().CorrectCardPositions();
		}
	}
}
