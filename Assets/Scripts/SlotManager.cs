using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour {

	public GameObject cardSlotPrefab;
	public Transform policySlotsParent;
	public Transform myOperativeSlotsParent;
	public Transform enemyOperativeSlotsParent;
	public float policySlotSpacing = 2;
	public float operativeSlotSpacing = 1.5f;

	private CardSlot[] policySlots;
	private CardSlot[] myOperativeSlots;
	private CardSlot[] enemyOperativeSlots;


	// Setup policy and operative slots in the play area
	public void SetupSlots(int numPolicySlots, int numOperativeSlots) {
		// setup policy slots
		policySlots = new CardSlot[numPolicySlots];
		for (int i = 0; i < numPolicySlots; i++) {
			GameObject GO = Instantiate(cardSlotPrefab, policySlotsParent);
			GO.transform.localPosition = new Vector3(-numPolicySlots*0.5f*policySlotSpacing + i * policySlotSpacing, 0, 0);
			policySlots[i] = GO.GetComponent<CardSlot>();
			policySlots[i].Setup(CardCategory.Policy, true);
		}

		// setup operative slots
		myOperativeSlots = new CardSlot[numOperativeSlots];
		enemyOperativeSlots = new CardSlot[numOperativeSlots];
		for (int i = 0; i < numOperativeSlots; i++) {
			// my slot
			GameObject myGO = Instantiate(cardSlotPrefab, myOperativeSlotsParent);
			myGO.transform.localPosition = new Vector3(-numOperativeSlots*0.5f*operativeSlotSpacing + i * operativeSlotSpacing, 0, 0);
			myOperativeSlots[i] = myGO.GetComponent<CardSlot>();
			myOperativeSlots[i].Setup(CardCategory.Operative, true);
			// enemy slot
			GameObject enemyGO = Instantiate(cardSlotPrefab, enemyOperativeSlotsParent);
			enemyGO.transform.localPosition = new Vector3(-numOperativeSlots*0.5f*operativeSlotSpacing + i * operativeSlotSpacing, 0, 0);
			enemyOperativeSlots[i] = enemyGO.GetComponent<CardSlot>();
			enemyOperativeSlots[i].Setup(CardCategory.Operative, false);
		}
	}

	bool isFreeSlot(CardSlot[] slots) {
		foreach (CardSlot s in slots) {
			if (!s.IsOccupied()) {
				return true;
			}
		}
		return false;
	}

	CardSlot getFreeSlot(CardSlot[] slots) {
		foreach(CardSlot s in slots) {
			if (!s.IsOccupied()) {
				return s;
			}
		}

		// no free slots
		return null;
	}
}
