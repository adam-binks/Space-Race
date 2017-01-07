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
	public Deck localDeck;

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
			photonView.RPC("setCurrentTurn", PhotonTargets.All, localPlayerNum); // temp: master always starts
		}

		// temp: just make a deck here
		localDeck = new Deck(new List<CardID>() {new CardID("ScienceFunding"),
												 new CardID("ScienceFunding"),
												 new CardID("ScienceFunding"),
												 new CardID("ScienceFunding"),
												 new CardID("ScienceFunding")});
		
		deckDisplay.UpdateRemaining(localDeck.getCount(), true);
		setupSlots();
	}

	// just for initial turn setup - future stuff should be handled by actions
	[PunRPC]
	void setCurrentTurn(int playerNum) {
		currentTurn = playerNum;
	}

	// Setup policy and operative slots in the play area
	void setupSlots() {
		// setup policy slots
		policySlots = new CardSlot[numPolicySlots];
		for (int i = 0; i < numPolicySlots; i++) {
			GameObject GO = Instantiate(cardSlotPrefab, policySlotsParent);
			GO.transform.localPosition = new Vector3(-numPolicySlots*0.5f*policySlotSpacing + i * policySlotSpacing, 0, 0);
			policySlots[i] = GO.GetComponent<CardSlot>();
			policySlots[i].setup(cardCategory.Policy, true);
		}

		// setup operative slots
		myOperativeSlots = new CardSlot[numOperativeSlots];
		enemyOperativeSlots = new CardSlot[numOperativeSlots];
		for (int i = 0; i < numOperativeSlots; i++) {
			// my slot
			GameObject myGO = Instantiate(cardSlotPrefab, myOperativeSlotsParent);
			myGO.transform.localPosition = new Vector3(-numOperativeSlots*0.5f*operativeSlotSpacing + i * operativeSlotSpacing, 0, 0);
			myOperativeSlots[i] = myGO.GetComponent<CardSlot>();
			myOperativeSlots[i].setup(cardCategory.Operative, true);
			// enemy slot
			GameObject enemyGO = Instantiate(cardSlotPrefab, enemyOperativeSlotsParent);
			enemyGO.transform.localPosition = new Vector3(-numOperativeSlots*0.5f*operativeSlotSpacing + i * operativeSlotSpacing, 0, 0);
			enemyOperativeSlots[i] = enemyGO.GetComponent<CardSlot>();
			enemyOperativeSlots[i].setup(cardCategory.Operative, false);
		}
	}

	void EndCurrentTurn() {
		currentTurn = getOtherPlayer(currentTurn);
	}

	int getOtherPlayer(int p) {
		return (p == 1) ? 2 : 1;
	}
	
	public Card createCardGO(CardID ID) {
		GameObject GO = Instantiate(cardPrefab);
		Card c = GO.GetComponent<Card>();
		c.setup(ID);
		return c;
	}
}
