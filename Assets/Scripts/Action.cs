using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ActionStatus {
	Queueing,
	Acting,
	Acted
}

public enum Act {
	EndTurn
}


/// Superclass for all possible actions.
/// This is the bit that is transmitted between players and added to the queue so just use three ints.
public class Action {
	// This dict allows subclasses to access IDs by descriptions to hopefully
	// prevent me inputting the wrong IDs
	public static Dictionary<int, string> actorAndTargetIDDescriptions = new Dictionary<int, string> {
		{-1, "No actor"},
		{ 1, "Player one" },
		{ 2, "Player two" }
	};

	public static int GetActorID(string description) {
		return GetKeyByValue(actorAndTargetIDDescriptions, description);
	}

	static int GetKeyByValue(Dictionary<int, string> dict, string dictValue) {
		return actorAndTargetIDDescriptions.First(x => x.Value == dictValue).Key;
	}


	// The three values that make up the action
	public int actorID;
	public int targetID;
	public int actionID; // store as int
	public ActionStatus status;


	public Action(int actorID, int targetID, Act action) {
		this.actorID = actorID;
		this.targetID = targetID;
		this.actionID = (int)action;
	}

	protected Action() {
		// IDs are left blank - set by the subclass
	}

	public void PrintDetails() {
		Debug.Log("Action details:  actorID " + actorID + 
				  ", targetID " + targetID + 
				  ", actionID " + ((Act)actionID).ToString());
	}

	/// Perform this action
	public void Perform(GameManager gm) {
		switch ((Act)actionID) {

			case Act.EndTurn:
				gm.turnManager.EndTurn();
				status = ActionStatus.Acted;
				break;
				
			default:
				Debug.LogError("Invalid actionID " + actionID);
				break;
		}
	}

	public void SetAction(Act act) {
		actionID = (int)act;
	}
}


// The following subclasses are just Actions with some fields already filled out


public class EndTurnAction : Action {
	public EndTurnAction() {
		actorID = GetActorID("No actor");
		targetID = GetActorID("No actor");
		SetAction(Act.EndTurn);
	}
}