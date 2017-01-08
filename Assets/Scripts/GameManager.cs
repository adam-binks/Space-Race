using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PhotonView))]

public class GameManager : Photon.MonoBehaviour {

	public int numPolicySlots = 5;
	public int numOperativeSlots = 6;
	public float policySlotSpacing = 2;
	public float operativeSlotSpacing = 1.5f;
	public GameObject cardPrefab;
	public GameObject cardSlotPrefab;
	public Transform policySlotsParent;
	public Transform myOperativeSlotsParent;
	public Transform enemyOperativeSlotsParent;
	public Hand localHand;
	public Hand enemyHand;

	private Deck localDeck;
	private int localPlayerNum;
	private int currentTurn;
	private CardSlot[] policySlots;
	private CardSlot[] myOperativeSlots;
	private CardSlot[] enemyOperativeSlots;
	private DeckDisplay deckDisplay;


	void Start() {
		deckDisplay = GetComponent<DeckDisplay>();
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
		SetupSlots();

		StartTurn(); // temp: wait until other player has joined and is ready
	}

	// just for initial turn setup - future stuff should be handled by actions
	[PunRPC]
	void SetCurrentTurn(int playerNum) {
		currentTurn = playerNum;
	}

	// Setup policy and operative slots in the play area
	void SetupSlots() {
		// setup policy slots
		policySlots = new CardSlot[numPolicySlots];
		for (int i = 0; i < numPolicySlots; i++) {
			GameObject GO = Instantiate(cardSlotPrefab, policySlotsParent);
			GO.transform.localPosition = new Vector3(-numPolicySlots*0.5f*policySlotSpacing + i * policySlotSpacing, 0, 0);
			policySlots[i] = GO.GetComponent<CardSlot>();
			policySlots[i].Setup(cardCategory.Policy, true);
		}

		// setup operative slots
		myOperativeSlots = new CardSlot[numOperativeSlots];
		enemyOperativeSlots = new CardSlot[numOperativeSlots];
		for (int i = 0; i < numOperativeSlots; i++) {
			// my slot
			GameObject myGO = Instantiate(cardSlotPrefab, myOperativeSlotsParent);
			myGO.transform.localPosition = new Vector3(-numOperativeSlots*0.5f*operativeSlotSpacing + i * operativeSlotSpacing, 0, 0);
			myOperativeSlots[i] = myGO.GetComponent<CardSlot>();
			myOperativeSlots[i].Setup(cardCategory.Operative, true);
			// enemy slot
			GameObject enemyGO = Instantiate(cardSlotPrefab, enemyOperativeSlotsParent);
			enemyGO.transform.localPosition = new Vector3(-numOperativeSlots*0.5f*operativeSlotSpacing + i * operativeSlotSpacing, 0, 0);
			enemyOperativeSlots[i] = enemyGO.GetComponent<CardSlot>();
			enemyOperativeSlots[i].Setup(cardCategory.Operative, false);
		}
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
