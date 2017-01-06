using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

	public List<Card> cards = new List<Card>();

	// TODO max hand size?

	public void addToHand(Card c) {
		cards.Add(c);
	}

	public void removeFromHand(Card c) {
		cards.Remove(c);
	}
}
