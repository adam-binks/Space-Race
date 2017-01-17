using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardSlot : ActionActor {

	public Sprite policySlotSprite;
	public Sprite operativeSlotSprite;
	bool animateIn = true;

	private Card card = null;
	private CardCategory slotType = CardCategory.None;
	private bool isMine; // only applies to operative cardSlots
	private MouseTargetable mouseTargetable;

	public void Setup(CardCategory requiredCardCategory, bool isMine) {
		this.slotType = requiredCardCategory;

		// setup slot sprite
		if (slotType == CardCategory.Operative) {
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

	public Card GetCard() {
		return card;
	}

	public void AddCard(Card c) {
		if (IsOccupied()) {
			Debug.LogError("Cannot add card to occupied card slot!");
			return;
		}
		if (!CardCategoryIsValid(c)) {
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

	public bool CardCategoryIsValid(Card c) {
		return slotType == c.GetCategory()  &&  !IsOccupied();
	}

	public bool CanPlayCardHere(Card c) {
		return CardCategoryIsValid(c) && (slotType == CardCategory.Policy || isMine);
	}

	TargetingGroup GetEmptyTargetingGroup() {
		if (slotType == CardCategory.Operative) {
			return isMine ? TargetingGroup.EmptyMyOperativeSlot : TargetingGroup.EmptyEnemyOperativeSlot;
		} else if (slotType == CardCategory.Policy) {
			return TargetingGroup.EmptyPolicySlot;
		} else {
			Debug.LogError("Card slot must have type");
			return TargetingGroup.NOT_ASSIGNED;
		}
	}

	TargetingGroup GetFullTargetingGroup() {
		if (slotType == CardCategory.Operative) {
			return isMine ? TargetingGroup.FullMyOperativeSlot : TargetingGroup.EmptyMyOperativeSlot;
		} else if (slotType == CardCategory.Policy) {
			return TargetingGroup.FullPolicySlot;
		} else {
			Debug.LogError("Card slot must have type");
			return TargetingGroup.NOT_ASSIGNED;
		}
	}
}
