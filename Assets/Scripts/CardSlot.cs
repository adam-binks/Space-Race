using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardSlot : MonoBehaviour {

	public Sprite policySlotSprite;
	public Sprite operativeSlotSprite;
	bool animateIn = true;

	private Card card = null;
	private cardCategory requiredCat = cardCategory.None;
	private bool isMine;

	public void setup(cardCategory requiredCardCategory, bool isMine) {
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
	
	public bool isOccupied() {
		return card == null;
	}

	public void addCard(Card c) {
		if (isOccupied()) {
			Debug.LogError("Cannot add card to occupied card slot!");
			return;
		}
		if (requiredCat != cardCategory.None && requiredCat != c.getCategory()) {
			Debug.LogError("Cannot add card of type " + c.getCategory() + " to this card slot", this);
			return;
		}
		card = c;
	}

	public Card removeCard() {
		Card c = card;
		card = null;
		return c;
	}
}
