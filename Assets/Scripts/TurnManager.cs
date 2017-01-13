﻿using System.Collections;
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

		StartTurn();
	}

	void StartTurn() {
		if (IsMyTurn()) {
			MouseTargetable.SetActiveTargetingGroup(TargetingGroup.CardInMyHand); // make my cards in hand selectable on my turn
		} else {
			MouseTargetable.SetActiveTargetingGroups( new List<TargetingGroup> {} ); // make nothing selectable on enemy turn
		}

		RefillFunds();
		DrawCards();
	}

	/// Refill the current funds of the current player to their max funds
	void RefillFunds() {
		PlayerFunds funds = IsMyTurn() ? gm.myFunds : gm.enemyFunds;

		funds.increaseMaxFunds();
		funds.refillFunds();
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
		gm.actionQueue.AddAction(new EndTurnAction());
	}

	/// Called by EndTurnAction
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
