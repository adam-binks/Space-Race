using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Superclass for anything that can be the actor or target of an action (except for players (?))
public class ActionActor : MonoBehaviour {
	private static int nextUniqueID = 1000;
	private static Dictionary<int, ActionActor> allActors = new Dictionary<int, ActionActor>();
	public int actorID;


	static int GetNextUniqueID() {
		return nextUniqueID++;
	}

	static ActionActor GetActorByID(int ID) {
		if (allActors[ID] == null) {
			Debug.LogError("No actor with ID " + ID);
		}
		return allActors[ID];
	}


	void Awake() {
		SetActorID(GetNextUniqueID());
	}

	void SetActorID(int ID) {
		actorID = ID;
		allActors[ID] = this;
	}

	/// It's important that the other actor with that ID is destroyed!
	/// Potential for disaster?
	/// Necessary for the ConcealedCard thing?
	public void OverrideID(int newID) {
		allActors[actorID] = null;

		Destroy(allActors[newID]);
		SetActorID(newID);
	}
}
