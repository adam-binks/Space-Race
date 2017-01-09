using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Superclass for anything that can be the actor or target of an action (except for players (?))
/// Every actor has a unique actorID that should be consistent between the two clients
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

	public static Card GetCardByID(int ID) {
		ActionActor a = GetActorByID(ID);
		if (a is Card) {
			return (Card)a;
		} else {
			Debug.LogError("ID " + ID + " does not correspond to a Card!");
			return null;
		}
	}

	public static CardSlot GetCardSlotByID(int ID) {
		ActionActor a = GetActorByID(ID);
		if (a is CardSlot) {
			return (CardSlot)a;
		} else {
			Debug.LogError("ID " + ID + " does not correspond to a CardSlot!");
			return null;
		}
	}


	void Awake() {
		SetActorID(GetNextUniqueID());
	}

	void SetActorID(int ID) {
		actorID = ID;
		allActors[ID] = this;
	}
}
