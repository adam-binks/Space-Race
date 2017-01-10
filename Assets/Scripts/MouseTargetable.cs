using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TargetingGroup {
	None,
	CardInMyHand,
	CardInEnemyHand,
	HeldCard,
	EmptyPolicySlot,
	EmptyOperativeSlot,
	FilledPolicySlot,
	FilledOperativeSlot
}


/// Add this prefab as a child of any GO that has mouse interaction
/// It will call methods of the format OnMouseEnterAndActive()
/// when its TargetingGroup is one of the active TargetingGroups

public class MouseTargetable : MonoBehaviour {
	

	public static Card heldCard = null; // keep track of which card is being dragged about by the mouse
	public static Card justDroppedCard = null; // to tell cardslots when the card has been dropped

	private static List<TargetingGroup> activeTargetingGroups;
	private static List<MouseTargetable> allMouseTargetables = new List<MouseTargetable>();

	public static void SetActiveTargetingGroups(List<TargetingGroup> newTargetingGroups) {
		activeTargetingGroups = newTargetingGroups;
		foreach (MouseTargetable MT in allMouseTargetables) {
			MT.OnChangeOfTargetables();
		}
	}


	public TargetingGroup targetingGroup;
	private bool wasActive = false;


	void Start () {
		allMouseTargetables.Add(this);
	}

	void Update() {
		if (targetingGroup == TargetingGroup.None) {
			Debug.LogError("Didn't set targeting group!", this);
		}
		justDroppedCard = null;
	}

	public void SetTargetingGroup(TargetingGroup newTG) {
		targetingGroup = newTG;
		OnChangeOfTargetables(); // in case the change means it has gone active > inactive or vice versa
	}

	// Send the Unity OnMouseDoesX() messages upwards if this MouseTargetable is currently one of the  active targeting groups

	void OnMouseEnter() {
		SendMessageIfTargetingGroupIsActive("OnMouseEnterAndActive");
	}

	void OnMouseOver() {
		SendMessageIfTargetingGroupIsActive("OnMouseOverAndActive");
	}

	void OnMouseExit() {
		SendMessageIfTargetingGroupIsActive("OnMouseExitAndActive");
	}

	void OnMouseDown() {
		SendMessageIfTargetingGroupIsActive("OnMouseDownAndActive");
	}

	void OnMouseUpAsButton() {
		SendMessageIfTargetingGroupIsActive("OnClickAndActive");
	}

	void OnMouseUp() {
		SendMessageIfTargetingGroupIsActive("OnMouseUpAndActive");
	}

	void SendMessageIfTargetingGroupIsActive(string msg) {
		if (isTargetingActive()) {
			SendMessageUpwards(msg, SendMessageOptions.DontRequireReceiver);
		}
	}

	/// When the ActiveTargetingGroups are changed, SendMessages if this affects this MouseTargetable's activeness
	public void OnChangeOfTargetables() {
		if (wasActive && !isTargetingActive()) {
			NoLongerTargetable();
		} else if (!wasActive && isTargetingActive()) {
			NewlyTargetable();
		}

		wasActive = isTargetingActive();
	}

	void NoLongerTargetable() {
		SendMessageUpwards("OnNoLongerTargetable", SendMessageOptions.DontRequireReceiver);
	}

	void NewlyTargetable() {
		Debug.Log("new t");
		SendMessageUpwards("OnNewlyTargetable", SendMessageOptions.DontRequireReceiver);
	}

	bool isTargetingActive() {
		return activeTargetingGroups.Contains(targetingGroup);
	}
}
