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
	EndTurn,
	RevealCard,
	PlayCard,
	ChargePolicyCard,
	ChargeOperativeCard
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


	// The four values that make up the action
	public int actorID;
	public int targetID;
	public int actionID; // store as int
	public string parameter; // parameter for any additional info. For example, reveal actions use this as cardID
	public ActionStatus status;


	public Action(int actorID, int targetID, Act action, string parameter) {
		this.actorID = actorID;
		this.targetID = targetID;
		this.actionID = (int)action;
		this.parameter = parameter;
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
		Card c;
		CardSlot cs;

		switch ((Act)actionID) {

			case Act.EndTurn:
				gm.turnManager.EndTurn();
				status = ActionStatus.Acted;
				break;
			
			case Act.RevealCard:
				c = ActionActor.GetCardByID(actorID);
				c.Reveal(new CardID(parameter));
				status = ActionStatus.Acted;
				break;
			
			case Act.PlayCard:
				c = ActionActor.GetCardByID(actorID);
				cs = ActionActor.GetCardSlotByID(targetID);
				c.AddToCardSlot(cs);
				status = ActionStatus.Acted;
				break;
			
			case Act.ChargePolicyCard:
				c = ActionActor.GetCardByID(actorID);
				c.ChargePolicy();
				status = ActionStatus.Acted;
				break;
			
			case Act.ChargeOperativeCard:
				c = ActionActor.GetCardByID(actorID);
				c.ChargeOperativeUntargeted();
				status = ActionStatus.Acted;
				break;

				// don't forget ActionStatus.Acted!

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

public class RevealCardAction : Action {
	public RevealCardAction(int actorID, string cardID) {
		this.actorID = actorID; // card to be revealed
		targetID = GetActorID("No actor");
		parameter = cardID;
		SetAction(Act.RevealCard);
	}
}

public class PlayCardAction : Action {
	public PlayCardAction(int cardActorID, int cardSlotActorID) {
		actorID = cardActorID;
		targetID = cardSlotActorID;
		SetAction(Act.PlayCard);
	}
}

public class ChargePolicyCardAction : Action {
	public ChargePolicyCardAction(int cardActorID) {
		actorID = cardActorID;
		targetID = GetActorID("No actor");
		SetAction(Act.ChargePolicyCard);
	}
}

public class ChargeOperativeCardUntargetedAction : Action {
	public ChargeOperativeCardUntargetedAction(int cardActorID) {
		actorID = cardActorID;
		targetID = GetActorID("No actor");
		SetAction(Act.ChargeOperativeCard);
	}
}