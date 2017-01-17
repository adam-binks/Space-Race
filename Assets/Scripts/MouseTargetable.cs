using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TargetingGroup {
	NOT_ASSIGNED,
	CardInMyHandPlayable,
	CardInMyHandUnplayable,
	CardInEnemyHand,
	CardOnBoardChargeable,
	CardOnBoardUnchargeable,
	DraggedObject,
	EmptyPolicySlot,
	FullPolicySlot,
	EmptyMyOperativeSlot,
	FullMyOperativeSlot,
	EmptyEnemyOperativeSlot,
	FullEnemyOperativeSlot
}


[RequireComponent(typeof(BoxCollider2D))]
public class MouseTargetable : MonoBehaviour {

	public static List<TargetingGroup> activeTargetingGroups = new List<TargetingGroup>();
	public static List<MouseTargetable> allMouseTargetables = new List<MouseTargetable>();
	public static void SetActiveTargetingGroups(List<TargetingGroup> newTGs) {
		// check for newly added TGs
		foreach (TargetingGroup newTG in newTGs) {
			if (!activeTargetingGroups.Contains(newTG)) {
				// if this TG is newly added, alert all MTs which belong to it
				foreach (MouseTargetable MT in allMouseTargetables) {
					if (MT.targetingGroup == newTG) {
						MT.OnTGNewlyAdded();
					}
				}
			}
		}

		// check for newly removed TGs
		foreach(TargetingGroup oldTG in activeTargetingGroups) {
			if (!newTGs.Contains(oldTG)) {
				// if this TG has just been removed, alert all MTs which belong to it
				foreach (MouseTargetable MT in allMouseTargetables) {
					if (MT.targetingGroup == oldTG) {
						MT.OnTGNewlyRemoved();
					}
				}
			}
		}

		// update TGs
		activeTargetingGroups = newTGs;
	}

	/// Helper method for when setting single TGs
	public static void SetActiveTargetingGroup(TargetingGroup targetingGroup) {
		SetActiveTargetingGroups(new List<TargetingGroup> {targetingGroup});
	}


	protected TargetingGroup targetingGroup;
	// these actions are set by other scripts on this GameObject
	public List<System.Action> OnTGNewlyAddedActions = new List<System.Action>();
	public List<System.Action> OnTGNewlyRemovedActions = new List<System.Action>();
	public List<System.Action> OnMouseDownActions = new List<System.Action>();
	public List<System.Action> OnMouseDragActions = new List<System.Action>();
	public List<System.Action> OnMouseEnterActions = new List<System.Action>();
	public List<System.Action> OnMouseExitActions = new List<System.Action>();
	public List<System.Action> OnMouseOverActions = new List<System.Action>();
	public List<System.Action> OnMouseUpActions = new List<System.Action>();
	public List<System.Action> OnMouseUpAsButtonActions = new List<System.Action>();


	void Start() {
		allMouseTargetables.Add(this);
	}
	
	void Update () {
		if (targetingGroup == TargetingGroup.NOT_ASSIGNED) {
			Debug.LogError("Targeting group not assigned!", this);
		}
	}

	public void Refresh() {
		if (TargetingGroupIsActive()) {
			OnTGNewlyAdded();
		}
	}

	public void SetTargetingGroup(TargetingGroup newTG) {
		TargetingGroup oldTG = targetingGroup;

		// check if the change in targeting groups means it has gone active > inactive or vice versa
		if (activeTargetingGroups.Contains(oldTG) && !activeTargetingGroups.Contains(newTG)) {
			OnTGNewlyRemoved();
		}
		if (!activeTargetingGroups.Contains(oldTG) && activeTargetingGroups.Contains(newTG)) {
			OnTGNewlyAdded();
		}

		targetingGroup = newTG;
	}

	public TargetingGroup GetTargetingGroup() {
		return targetingGroup;
	}

	public bool TargetingGroupIsActive() {
		return activeTargetingGroups.Contains(targetingGroup);
	}

	void InvokeIfActive(List<System.Action> actions) {
		foreach (System.Action a in actions) {
			if (TargetingGroupIsActive()) {
				a.Invoke();
			}
		}
	}

	void InvokeAll(List<System.Action> actions) {
		foreach (System.Action a in actions) {
			a.Invoke();
		}
	}


	// invoke assigned actions when inputs are recieved if this is one of the active targeting groups

	protected void OnTGNewlyAdded() {
		InvokeAll(OnTGNewlyAddedActions);
	}

	protected void OnTGNewlyRemoved() {
		InvokeAll(OnTGNewlyRemovedActions);
	}

	void OnMouseDown() {
		InvokeIfActive(OnMouseDownActions);
	}

	void OnMouseDrag() {
		InvokeIfActive(OnMouseDragActions);
	}

	void OnMouseEnter() {
		InvokeIfActive(OnMouseEnterActions);
	}

	void OnMouseExit() {
		InvokeIfActive(OnMouseExitActions);
	}

	void OnMouseOver() {
		InvokeIfActive(OnMouseOverActions);
	}

	void OnMouseUp() {
		InvokeIfActive(OnMouseUpActions);
	}

	void OnMouseUpAsButton() {
		InvokeIfActive(OnMouseUpAsButtonActions);
	}
}
