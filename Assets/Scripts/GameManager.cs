using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PhotonView))]

public class GameManager : Photon.MonoBehaviour {

	public int numPolicySlots = 5;
	public int numOperativeSlots = 6;
	public GameObject cardPrefab;
	public Hand localHand;
	public Hand enemyHand;
	[HideInInspector]
	public int localPlayerNum;
	[HideInInspector]
	public ActionQueue actionQueue;
	public TurnManager turnManager;

	private Deck localDeck;
	private DeckDisplay deckDisplay;
	private SlotManager slotManager;


	void Start() {
		deckDisplay = GetComponent<DeckDisplay>();
		slotManager = GetComponent<SlotManager>();
		turnManager = GetComponent<TurnManager>();
		actionQueue = GetComponent<ActionQueue>();
	}

	void OnJoinedRoom () {
		localPlayerNum = PhotonNetwork.isMasterClient ? 1 : 2;

		// temp: just make a deck here
		localDeck = new Deck(new List<CardID>() {new CardID("ScienceFunding"),
												 new CardID("ScienceFunding"),
												 new CardID("ScienceFunding"),
												 new CardID("ScienceFunding"),
												 new CardID("ScienceFunding")});
		
		deckDisplay.UpdateRemaining(localDeck.GetCount(), true);
		slotManager.SetupSlots(numPolicySlots, numOperativeSlots);

		turnManager.StartGame(); // temp: wait until other player has joined and is ready
	}

	public int GetOtherPlayer(int p) {
		return (p == 1) ? 2 : 1;
	}
	
	public Card CreateCardGO(CardID ID) {
		GameObject GO = Instantiate(cardPrefab);
		Card c = GO.GetComponent<Card>();
		c.Setup(ID);
		return c;
	}

	public Card DrawMyCard() {
		Card c = deckDisplay.DrawCard(localDeck, this);

		if (c == null) {
			// deck is empty
			return null;
		}

		localHand.AddToHand(c);
		return c;
	}

	public ConcealedCard DrawEnemyCard() {
		deckDisplay.DecrementEnemyRemaining();
		ConcealedCard c = deckDisplay.DrawConcealedCard();
		enemyHand.AddToHand(c);
		return c;
	}
}
