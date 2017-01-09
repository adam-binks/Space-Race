using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : ActionActor {

	public Text titleText;
	public Text descriptionText;
	[HeaderAttribute("Juice")]
	public float flipDuration = 0.1f;

	private CardTemplate template;
	private bool isCharged = false;
	/// Cards are concealed when the enemy draws them, this client shouldn't know what they are
	private bool isConcealed;


	public void Setup(CardID ID) {
		template = ID.GetTemplate();

		// setup visuals
		titleText.text = template.cardName;
		descriptionText.text = template.description;

		isConcealed = false;

		//Debug.Log("ID: " + actorID);
	}

	/// When the enemy draws a card, this client shouldn't know what it is
	public void ConcealedSetup() {
		isConcealed = true;
		FlipFaceDown(false);

		titleText.text = "Concealed";
		descriptionText.text = "Concealed";
	}

	/// Flip face up, and unconceal + setup card details if the card is currently conceal
	public void Reveal(CardID ID) {
		// if the card is already revealed, do nothing
		// this is for the benefit of the player who drew the card
		if (isConcealed) {
			Setup(ID);
			FlipFaceUp(true);
		}
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