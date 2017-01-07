using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

/// This class is only used to represent the **local** player's deck. 
/// The enemy deck is not represented as the local client should not know its contents. 
/// Therefore methods like shuffle() are not RPCs. 
/// Cards are stored simply as cardIDs to save on memory (does eliminate some possibilities though). 
/// Deck() is just for data, the visual representation is handled separately (that is common between
/// both clients).
public class Deck {

	private List<CardID> deck;


	public Deck() {
		deck = new List<CardID>();
	}

	public Deck(List<CardID> deck) {
		this.deck = deck;
	}

	/// Add a cardID to a random position in the deck
	public void shuffleInCardID(CardID c) {
		int location = UnityEngine.Random.Range(0, deck.Count);
		deck.Insert(location, c);
	}

	/// Not perfectly random but good enough for now
	public void shuffle() {
		deck = deck.OrderBy(a => Guid.NewGuid()).ToList();
	}

	/// Remove the top cardID from the deck and return it
	public CardID drawCard() {
		if (deck.Count == 0) {
			Debug.Log("Deck is empty - can't draw card!");
			return null;
		}
		CardID ID = deck[0];
		deck.Remove(ID);
		return ID;
	}

	public int getCount() {
		return deck.Count;
	}
}
