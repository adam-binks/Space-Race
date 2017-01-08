using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

	private int currentTurn;
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
		RefillFunds();
		DrawCards();
	}

	void DrawCards() {
		if (IsMyTurn()) {
			gm.DrawMyCard();
		} else {
			gm.DrawEnemyCard();
		}
	}

	void RefillFunds() {

	}
	
	// temp: will be called by action
	public void EndTurn() {
		turnCount++;
		currentTurn = gm.GetOtherPlayer(currentTurn);
	}

	public bool IsMyTurn() {
		return currentTurn == gm.localPlayerNum;
	}


}
