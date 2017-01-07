using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour {

	public Text titleText;
	public Text descriptionText;

	private CardTemplate template;
	private bool isCharged = false;


	public void setup(CardID ID) {
		template = ID.getTemplate();

		// setup visuals
		titleText.text = template.cardName;
		descriptionText.text = template.description;
	}

	public cardCategory getCategory() {
		return template.cat;
	}

	public string getName() {
		return template.cardName;
	}
}
