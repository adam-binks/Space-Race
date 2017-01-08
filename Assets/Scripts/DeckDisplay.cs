using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckDisplay : MonoBehaviour {
	
	public Text myDeckRemaining;
	public Text enemyDeckRemaining;
	public Transform myDeck;
	public Transform enemyDeck;
	public GameObject concealedCardPrefab;

	private int enemyCardsRemaining = 30;

	
	public void UpdateRemaining(int remaining, bool isMine) {
		Text remainingText = null;
		if (isMine) {
			remainingText = myDeckRemaining;
		} else {
			remainingText = enemyDeckRemaining;
		}

		remainingText.text = remaining.ToString() + " cards remaining";
	}

	public void DecrementEnemyRemaining() {
		enemyCardsRemaining--;
		UpdateRemaining(enemyCardsRemaining, false);
	}


	/// Draw card from local deck
	public Card DrawCard(Deck deck, GameManager gm) {
		CardID ID = deck.DrawCard();

		if (ID == null) {
			// deck is empty
			// TODO: fatigue damage? shuffle discard back in?
			return null;
		}

		Card c = gm.CreateCardGO(ID);
		// position it on the local deck
		c.transform.position = myDeck.transform.position;
		// start upside down then tween face up
		c.FlipFaceDown(false);
		c.FlipFaceUp(true);

		UpdateRemaining(deck.GetCount(), true);
		return c;
	}

	/// Draw a concealed card from the enemy deck
	public ConcealedCard DrawConcealedCard() {
		return Instantiate(concealedCardPrefab, enemyDeck.transform.position, Quaternion.identity).GetComponent<ConcealedCard>();
	}
}
