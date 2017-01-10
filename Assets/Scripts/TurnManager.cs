using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

	[HideInInspector]
	public int currentTurn;

	private int turnCount = 0;
	private GameManager gm;

	void Start () {
		gm = GetComponent<GameManager>();
	}

	public void StartGame() {
		currentTurn = 1; // temp: master client goes first always

		MouseTargetable.SetActiveTargetingGroups(new List<TargetingGroup> {TargetingGroup.CardInMyHand});

		StartTurn();
	}

	void StartTurn() {
		if (IsMyTurn()) {
			// on my turn, make cards in hand interactable
			MouseTargetable.SetActiveTargetingGroups(new List<TargetingGroup> {TargetingGroup.CardInMyHand});
		} else {
			// make nothing interactable on the enemy's turn
			MouseTargetable.SetActiveTargetingGroups(new List<TargetingGroup> {} ); 
		}

		RefillFunds();
		DrawCards();
	}

	void RefillFunds() {

	}

	void DrawCards() {
		if (IsMyTurn()) {
			gm.DrawMyCard();
		} else {
			gm.DrawEnemyCard();
		}
	}
	
	/// Called by End Turn button click
	public void CreateEndTurnAction() {
		// end the turn for both players (as long as no card is being held)
		if (MouseTargetable.heldCard == null) {
			gm.actionQueue.AddAction(new EndTurnAction());
		}
	}

	public void EndTurn() {
		gm.actionQueue.EndTurn();
		turnCount++;
		currentTurn = gm.GetOtherPlayer(currentTurn);
		StartTurn();
	}

	public bool IsMyTurn() {
		return currentTurn == gm.localPlayerNum;
	}

	public int GetTurnNumber() {
		return turnCount;
	}
}
