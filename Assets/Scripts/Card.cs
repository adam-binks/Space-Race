using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public enum CardStatus {
	InHand,
	OnBoard
}

public class Card : ActionActor {

	public Text titleText;
	public Text descriptionText;
	[HeaderAttribute("Juice")]
	public float flipDuration = 0.1f;
	public float lerpToMouseRate = 0.1f;

	private CardTemplate template;
	private bool isCharged = false;
	/// Cards are concealed when the enemy draws them, this client shouldn't know what they are
	private bool isConcealed;
	private CardStatus status;
	private MouseTargetable mouseTargetable;


	public void Setup(CardID ID) {
		template = ID.GetTemplate();

		// setup visuals
		titleText.text = template.cardName;
		descriptionText.text = template.description;

		isConcealed = false;

		mouseTargetable = GetComponentInChildren<MouseTargetable>();
		mouseTargetable.SetTargetingGroup(TargetingGroup.CardInMyHand);

		//Debug.Log("ID: " + actorID);
	}

	/// When the enemy draws a card, this client shouldn't know what it is
	public void ConcealedSetup() {
		isConcealed = true;
		FlipFaceDown(false);

		mouseTargetable = GetComponentInChildren<MouseTargetable>();
		mouseTargetable.SetTargetingGroup(TargetingGroup.CardInEnemyHand);

		titleText.text = "Concealed";
		descriptionText.text = "Concealed";
	}

	void Update() {
		if (MouseTargetable.heldCard == this) {
			LerpTowardsMousePos();
		}
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

	public CardCategory GetCategory() {
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

	void OnMouseDownAndActive() {
		PickUp();
	}

	void PickUp() {
		if (MouseTargetable.heldCard == null && isConcealed == false) {
			MouseTargetable.heldCard = this;

			if (template.cat == CardCategory.Operative) {
				MouseTargetable.SetActiveTargetingGroups(new List<TargetingGroup> {TargetingGroup.EmptyOperativeSlot,
																				   TargetingGroup.HeldCard});
			} else {
				MouseTargetable.SetActiveTargetingGroups(new List<TargetingGroup> {TargetingGroup.EmptyPolicySlot,
																				   TargetingGroup.HeldCard});
			}

			mouseTargetable.SetTargetingGroup(TargetingGroup.HeldCard);
		}
	}

	void OnMouseUpAndActive() {
		Drop();
	}

	void Drop() {
		MouseTargetable.heldCard = null;
		mouseTargetable.SetTargetingGroup(TargetingGroup.CardInMyHand);
		MouseTargetable.SetActiveTargetingGroups(new List<TargetingGroup> {TargetingGroup.CardInMyHand} );

		GameObject.Find("MyHand").GetComponent<Hand>().CorrectCardPositions();
		// TODO
	}

	void LerpTowardsMousePos() {
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		transform.position = new Vector2(
			transform.position.x + (mousePos.x - transform.position.x) * lerpToMouseRate * Time.deltaTime,
			transform.position.y + (mousePos.y - transform.position.y) * lerpToMouseRate * Time.deltaTime
		);
	}

}