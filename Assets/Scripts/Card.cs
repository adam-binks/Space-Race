using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {

	public static GameObject cardPrefab;

	private CardTemplate template;
	private bool isCharged = false;


	public static Card createCardGO(string cardName) {
		GameObject GO = Instantiate(cardPrefab);
		Card c = GO.GetComponent<Card>();
		c.setup(cardName);
		return c;
	}


	public void setup(string cardName) {
		// TEMP - todo proper card ID stuff
		if (cardName == "Science Funding") {
			template = new DoubleDraw();
		} else {
			Debug.LogError("Invalid cardName");
		}
	}

	public cardCategory getCategory() {
		return template.cat;
	}

	public string getName() {
		return template.cardName;
	}
}
