using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]

public class ActionQueue : Photon.MonoBehaviour {

	/// All actions from this game
	private List<Action> allActions = new List<Action>();
	/// Access actions by actionsByTurn[turnNumber][actionNumInTurn]
	private List<List<Action>> actionsByTurn = new List<List<Action>>();
	private GameManager gm;
	private Action currentAction = null; // action currently being performed


	void Start() {
		gm = GetComponent<GameManager>();
		actionsByTurn.Add(new List<Action>()); // add an empty list for turn 0
	}

	void Update() {
		// start next action if this one's done
		if (currentAction == null || currentAction.status == ActionStatus.Acted) {
			currentAction = GetNextAction();
			if (currentAction != null) {
				currentAction.status = ActionStatus.Acting;
				currentAction.Perform(gm);
			}
		}
	}

	public void AddAction(Action a) {
		photonView.RPC("AddActionRPC", PhotonTargets.All, a.actorID, a.targetID, a.actionID);
	}

	[PunRPC]
	void AddActionRPC(int actorID, int targetID, int actionID) {
		Action a = new Action(actorID, targetID, (Act)actionID);

		// a.printDetails()

		allActions.Add(a);
		actionsByTurn[gm.turnManager.GetTurnNumber()].Add(a);
	}

	/// Returns the first action in the queue that hasn't yet been performed. 
	/// Or null if all actions in the queue are done
	Action GetNextAction() {
		foreach(Action a in allActions) {
			if (a.status == ActionStatus.Queueing) {
				return a;
			}
		}
		return null; // no actions in the queue that haven't been done
	}

	public void EndTurn() {
		// add a new empty list to the actionsByTurn, to be populated as the turn is played
		actionsByTurn.Add(new List<Action>());
	}
}
