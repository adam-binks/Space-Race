using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hand : MonoBehaviour {

	public float spacing = 150;
	public float tweenToCorrectPosTime = 0.5f;

	private List<Card> cards = new List<Card>();

	// TODO max hand size?

	/// tween the cards to their proper positions based on the number of cards in the hand
	public void CorrectCardPositions() {
		// cancel any current rearranging tweens
		DOTween.Kill("HandRearrange");

		// start tweens to proper new locations for each card
		for (int i = 0; i < cards.Count; i++) {
			Tweener t = cards[i].transform.DOLocalMove(GetCardPosInHand(i), tweenToCorrectPosTime);
			t.SetId("HandRearrange");
		}
	}

	public void AddToHand(Card c) {
		cards.Add(c);
		c.transform.SetParent(transform, true);
		CorrectCardPositions();
	}

	public void RemoveFromHand(Card c) {
		cards.Remove(c);
		CorrectCardPositions();
	}

	Vector2 GetCardPosInHand(int index) {
		return new Vector2( -0.5f * cards.Count * spacing  +  spacing * index , 0);
	}
}
