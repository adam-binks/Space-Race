using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardID {

	private string ID;
	// TODO: download? may create more problems than it solves..
	private string[] acceptableIDs = {
		"ScienceFunding"
	};

	public CardID(string ID) {
		if (!acceptableIDs.Contains(ID)) {
			Debug.LogError("Card ID " + ID + " is invalid");
			return;
		}
		this.ID = ID;
	}

	public CardTemplate getTemplate() {
		switch (ID) {
			case "ScienceFunding":
				return new ScienceFunding();
			default:
				Debug.LogError("Invalid ID " + ID);
				return null;
		}
	}

	public string getID() {
		return this.ID;
	}
}
