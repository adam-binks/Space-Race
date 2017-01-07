using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckDisplay : MonoBehaviour {
	
	public Text myDeckRemaining;
	public Text enemyDeckRemaining;

	
	public void UpdateRemaining(int remaining, bool isMine) {
		Text remainingText = null;
		if (isMine) {
			remainingText = myDeckRemaining;
		} else {
			remainingText = enemyDeckRemaining;
		}

		remainingText.text = remaining.ToString() + " cards remaining";
	}
}
