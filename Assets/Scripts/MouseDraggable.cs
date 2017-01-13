using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MouseTargetable))]
public class MouseDraggable : MonoBehaviour {

	public static GameObject heldGO = null; // the GO currently being dragged
	public float lerpToMouseRate = 10;
	public bool isDraggable = true;
	public List<System.Action> OnPickUp = new List<System.Action>();
	public List<System.Action> OnDrop = new List<System.Action>();

	MouseTargetable mouseTargetable;

	void Start () {
		mouseTargetable = GetComponent<MouseTargetable>();
		mouseTargetable.OnMouseDownActions.Add(PickUp);
		mouseTargetable.OnMouseUpActions.Add(Drop);
	}

	void PickUp() {
		if (heldGO == null && isDraggable && mouseTargetable.TargetingGroupIsActive()) {
			heldGO = this.gameObject;
			mouseTargetable.SetTargetingGroup(TargetingGroup.DraggedObject);
			InvokeAllActions(OnPickUp);
		}
	}

	void Drop() {
		if (heldGO == this.gameObject) {
			heldGO = null;
			InvokeAllActions(OnDrop);
		}
	}

	void InvokeAllActions(List<System.Action> actions) {
		foreach (System.Action a in actions) {
			a.Invoke();
		}
	}
	
	void Update() {
		if (heldGO == this.gameObject) {
			// drop if the item is no longer draggable
			if (mouseTargetable.GetTargetingGroup() != TargetingGroup.DraggedObject || !isDraggable) {
				Drop();
			// otherwise lerp towards the mouse cursor
			} else {
				ChaseCursor();
			}
		}
	}

	void ChaseCursor() {
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		transform.position = new Vector2(
			transform.position.x + (mousePos.x - transform.position.x) * lerpToMouseRate * Time.deltaTime,
			transform.position.y + (mousePos.y - transform.position.y) * lerpToMouseRate * Time.deltaTime
		);
	}
}
