﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardID {

	private string ID;
	// TODO: download? may create more problems than it solves..
	private string[] acceptableIDs = {
		"ScienceFunding",
		"InvestigativeJournalist"
	};

	public CardID(string ID) {
		if (!acceptableIDs.Contains(ID)) {
			Debug.LogError("Card ID " + ID + " is invalid");
			return;
		}
		this.ID = ID;
	}

	public CardTemplate GetTemplate() {
		switch (ID) {
			case "ScienceFunding":
				return new ScienceFunding();
			case "InvestigativeJournalist":
				return new InvestigativeJournalist();
				
			default:
				Debug.LogError("Invalid ID " + ID);
				return null;
		}
	}

	public string GetID() {
		return this.ID;
	}
}
