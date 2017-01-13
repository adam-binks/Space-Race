using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFunds : MonoBehaviour {

	public int startingFunds = 1;
	public int defaultFundsIncreasePerTurn = 1;
	public Text fundsDisplay;

	private int maxFunds;
	private int currentFunds;
	private int fundsIncreasePerTurn;


	void Start() {
		currentFunds = 0;
		maxFunds = startingFunds;
		fundsIncreasePerTurn = 1;
	}

	public void refillFunds() {
		currentFunds = maxFunds;
		UpdateDisplay();
	}

	public void increaseMaxFunds() {
		maxFunds += fundsIncreasePerTurn;
	}

	public bool canAfford(int amount) {
		return amount <= currentFunds;
	}

	public void deductFromFunds(int amount) {
		currentFunds -= amount;
		if (currentFunds <= 0) {
			currentFunds = 0;
			Debug.LogWarning("Amount deducted greater than remaining funds - potential problem?", this);
		}
		UpdateDisplay();
	}

	void UpdateDisplay() {
		fundsDisplay.text = "Current funds: " + currentFunds.ToString() + "/" + maxFunds.ToString();
	}
}
