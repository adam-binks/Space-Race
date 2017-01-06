using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlot : MonoBehaviour {

	private Card card = null;
	private cardCategory requiredCat = cardCategory.None;

	public void setup(cardCategory requiredCardCategory) {
		this.requiredCat = requiredCardCategory;
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
