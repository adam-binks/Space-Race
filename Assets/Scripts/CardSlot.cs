using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardSlot : ActionActor {

	public Sprite policySlotSprite;
	public Sprite operativeSlotSprite;
	bool animateIn = true;

	private Card card = null;
	private cardCategory slotType = cardCategory.None;
	private bool isMine;
	private MouseTargetable mouseTargetable;

	public void Setup(cardCategory requiredCardCategory, bool isMine) {
		this.slotType = requiredCardCategory;

		// setup slot sprite
		if (slotType == cardCategory.Operative) {
			GetComponentInChildren<SpriteRenderer>().sprite = operativeSlotSprite;
		} else {
			GetComponentInChildren<SpriteRenderer>().sprite = policySlotSprite;
		}

		this.isMine = isMine; // is this a local or enemy operative slot?

		if (animateIn) {
			Vector2 scale = transform.localScale;
			transform.localScale = Vector2.zero;
			transform.DOScale(scale, 0.2f);
		}

		mouseTargetable = GetComponent<MouseTargetable>();
		mouseTargetable.SetTargetingGroup(GetEmptyTargetingGroup());
	}
	
	public bool IsOccupied() {
		return card != null;
	}

	public void AddCard(Card c) {
		if (IsOccupied()) {
			Debug.LogError("Cannot add card to occupied card slot!");
			return;
		}
		if (!CardIsValid(c)) {
			Debug.LogError("Cannot add card of type " + c.GetCategory() + " to this card slot", this);
			return;
		}
		card = c;
		mouseTargetable.SetTargetingGroup(GetFullTargetingGroup());
	}

	public Card RemoveCard() {
		Card c = card;
		card = null;
		mouseTargetable.SetTargetingGroup(GetEmptyTargetingGroup());
		return c;
	}

	public bool CardIsValid(Card c) {
		return slotType == c.GetCategory()  &&  !IsOccupied();
	}

	TargetingGroup GetEmptyTargetingGroup() {
		if (slotType == cardCategory.Operative) {
			return TargetingGroup.EmptyOperativeSlot;
		} else if (slotType == cardCategory.Policy) {
			return TargetingGroup.EmptyPolicySlot;
		} else {
			Debug.LogError("Card slot must have type");
			return TargetingGroup.NOT_ASSIGNED;
		}
	}

	TargetingGroup GetFullTargetingGroup() {
		if (slotType == cardCategory.Operative) {
			return TargetingGroup.FullOperativeSlot;
		} else if (slotType == cardCategory.Policy) {
			return TargetingGroup.FullPolicySlot;
		} else {
			Debug.LogError("Card slot must have type");
			return TargetingGroup.NOT_ASSIGNED;
		}
	}
}
