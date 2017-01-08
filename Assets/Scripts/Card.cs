using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour {

	public Text titleText;
	public Text descriptionText;
	[HeaderAttribute("Juice")]
	public float flipDuration = 0.1f;

	private CardTemplate template;
	private bool isCharged = false;


	public void Setup(CardID ID) {
		template = ID.GetTemplate();

		// setup visuals
		titleText.text = template.cardName;
		descriptionText.text = template.description;
	}

	public cardCategory GetCategory() {
		return template.cat;
	}

	public string GetName() {
		return template.cardName;
	}

	public void FlipFaceUp(bool tween = true) {
		if (tween) {
			transform.DORotate(Vector3.zero, flipDuration);
		} else {
			transform.rotation = Quaternion.identity;
		}
	}

	public void FlipFaceDown(bool tween = true) {
		if (tween) {
			transform.DORotate(new Vector3(0, 180, 0), flipDuration);
		} else {
			transform.rotation = Quaternion.Euler(0, 180, 0);
		}
	}
}