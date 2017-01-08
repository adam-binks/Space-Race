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

	private Deck localDeck;
	private int localPlayerNum;
	private int currentTurn;
	private DeckDisplay deckDisplay;
	private SlotManager slotManager;


	void Start() {
		deckDisplay = GetComponent<DeckDisplay>();
		slotManager = GetComponent<SlotManager>();
	}

	void OnJoinedRoom () {
		localPlayerNum = PhotonNetwork.isMasterClient ? 1 : 2;
		if (PhotonNetwork.isMasterClient) {
			photonView.RPC("SetCurrentTurn", PhotonTargets.All, localPlayerNum); // temp: master always starts
		}

		// temp: just make a deck here
		localDeck = new Deck(new List<CardID>() {new CardID("ScienceFunding"),
												 new CardID("ScienceFunding"),
												 new CardID("ScienceFunding"),
												 new CardID("ScienceFunding"),
												 new CardID("ScienceFunding")});
		
		deckDisplay.UpdateRemaining(localDeck.GetCount(), true);
		slotManager.SetupSlots(numPolicySlots, numOperativeSlots);

		StartTurn(); // temp: wait until other player has joined and is ready
	}

	// just for initial turn setup - future stuff should be handled by actions
	[PunRPC]
	void SetCurrentTurn(int playerNum) {
		currentTurn = playerNum;
	}

	void StartTurn() {
		if (IsMyTurn()) {
			DrawMyCard();
		} else {
			DrawEnemyCard();
		}
	}

	/// Temp: called by end turn button. TODO: call by an action (probably make private)
	public void EndCurrentTurn() {
		currentTurn = GetOtherPlayer(currentTurn);
		StartTurn();
	}

	bool IsMyTurn() {
		return currentTurn == localPlayerNum;
	}

	int GetOtherPlayer(int p) {
		return (p == 1) ? 2 : 1;
	}
	
	public Card CreateCardGO(CardID ID) {
		GameObject GO = Instantiate(cardPrefab);
		Card c = GO.GetComponent<Card>();
		c.Setup(ID);
		return c;
	}

	Card DrawMyCard() {
		Card c = deckDisplay.DrawCard(localDeck, this);

		if (c == null) {
			// deck is empty
			return null;
		}

		localHand.AddToHand(c);
		return c;
	}

	ConcealedCard DrawEnemyCard() {
		deckDisplay.DecrementEnemyRemaining();
		ConcealedCard c = deckDisplay.DrawConcealedCard();
		enemyHand.AddToHand(c);
		return c;
	}
}
