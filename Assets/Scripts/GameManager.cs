using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]

public class GameManager : Photon.MonoBehaviour {

	public int numPolicySlots = 5;
	public int numOperativeSlots = 6;
	public SpriteRenderer spriteRenderer;

	private int localPlayerNum;
	private int currentTurn;
	private CardSlot[] policySlots;
	private CardSlot[] myOperativeSlots;
	private CardSlot[] enemyOperativeSlots;
	private TextRender textRender;


	void Start() {
		textRender = GetComponent<TextRender>();
		spriteRenderer.sprite = GenerateTextSprite("Test", textStyle.Title);
	}

	void OnJoinedRoom () {
		localPlayerNum = PhotonNetwork.isMasterClient ? 1 : 2;
		if (PhotonNetwork.isMasterClient) {
			photonView.RPC("setCurrentTurn", PhotonTargets.All, localPlayerNum); // temp: master always starts
		}
		setupSlots();
	}

	// just for initial turn setup - future stuff should be handled by actions
	[PunRPC]
	void setCurrentTurn(int playerNum) {
		currentTurn = playerNum;
	}

	// Setup policy and operative slots in the play area
	void setupSlots() {
		policySlots = new CardSlot[numPolicySlots];
		myOperativeSlots = new CardSlot[numOperativeSlots];
		enemyOperativeSlots = new CardSlot[numOperativeSlots];
	}

	void EndCurrentTurn() {
		currentTurn = getOtherPlayer(currentTurn);
	}

	int getOtherPlayer(int p) {
		return (p == 1) ? 2 : 1;
	}

	/// Wrap the TextRender function to make it more easily accessible
	public Sprite GenerateTextSprite(string text, textStyle style) {
		textRender.GenerateTextSprite(text, style);
		return null;
	}
}
