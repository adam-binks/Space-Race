using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TargetingGroup {
	None,
	CardInHand,
	PolicySlot,
	OperativeSlot
}


/// Add this prefab as a child of any GO that has mouse interaction
/// It will call methods of the format OnMouseEnterAndActive()
/// when its TargetingGroup is one of the active TargetingGroups

public class MouseTargetable : MonoBehaviour {
	
	public static List<TargetingGroup> activeTargetingGroups = new List<TargetingGroup> {
		TargetingGroup.CardInHand
	};

	public TargetingGroup targetingGroup;

	private BoxCollider2D col;

	void Start () {
		col = GetComponent<BoxCollider2D>();
	}

	void Update() {
		if (targetingGroup == TargetingGroup.None) {
			Debug.LogError("Didn't set targeting group!", this);
		}
	}

	void OnMouseEnter() {
		SendMessageIfTargetingGroupIsActive("OnMouseEnterAndActive");
	}

	void OnMouseOver() {
		SendMessageIfTargetingGroupIsActive("OnMouseOverAndActive");
	}

	void OnMouseExit() {
		SendMessageIfTargetingGroupIsActive("OnMouseExitAndActive");
	}

	void SendMessageIfTargetingGroupIsActive(string msg) {
		if (activeTargetingGroups.Contains(targetingGroup)) {
			SendMessageUpwards(msg, SendMessageOptions.DontRequireReceiver);
		}
	}
}
