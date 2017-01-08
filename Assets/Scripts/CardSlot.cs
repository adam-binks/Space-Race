using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardSlot : ActionActor {

	public Sprite policySlotSprite;
	public Sprite operativeSlotSprite;
	bool animateIn = true;

	private Card card = null;
	private cardCategory requiredCat = cardCategory.None;
	private bool isMine;

	public void Setup(cardCategory requiredCardCategory, bool isMine) {
		this.requiredCat = requiredCardCategory;

		// setup slot sprite
		if (requiredCat == cardCategory.Operative) {
			GetComponentInChildren<SpriteRenderer>().sprite = operativeSlotSprite;
		} else {
			GetComponentInChildren<SpriteRenderer>().sprite = policySlotSprite;
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
		if (requiredCat != cardCategory.None && requiredCat != c.GetCategory()) {
			Debug.LogError("Cannot add card of type " + c.GetCategory() + " to this card slot", this);
			return;
		}
		card = c;
	}

	public Card RemoveCard() {
		Card c = card;
		card = null;
		return c;
	}
}
