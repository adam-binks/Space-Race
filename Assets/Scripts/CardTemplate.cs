using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardCategory {
	Policy,
	Operative,
	None
}


public class CardTemplate {   // << not a monobehaviour!
	public string cardName;
	public string description;
	public int playCost;
	public bool isChargeable;
	public int chargeCost;
	public CardCategory cat;
	public CardID ID;
	public bool isCharged = false;
	public int authorPlayer;

	public CardTemplate(string cardName,
						string description,
						int playCost,
						bool isChargeable,
						int chargeCost,
						CardCategory category,
						CardID ID) {
		this.cardName = cardName;
		this.description = description;
		this.playCost = playCost;
		this.isChargeable = isChargeable;
		this.chargeCost = chargeCost;
		this.cat = category;
		this.ID = ID;
	}

	// These methods may be overriden by card templates
	public virtual bool OnRefillFunds(GameManager gm) {return true;}
	public virtual bool OnTurnStart(GameManager gm) {return true;}
	public virtual bool OnCardPlay(GameManager gm) {return true;}
	public virtual bool OnDrawCard(GameManager gm) {return true;}
	public virtual bool OnSomeCardFund(GameManager gm) {return true;}
	public virtual bool OnResearch(GameManager gm) {return true;}
	public virtual bool OnCardRemove(GameManager gm) {return true;}
	public virtual bool OnTurnEnd(GameManager gm) {return true;}
	public virtual bool OnThisCardFund(GameManager gm) {return true;}
}


public class ScienceFunding : CardTemplate {
	public ScienceFunding() 
	: base("Science Funding", "Players draw two cards at the start of each turn", 2, true, 2, 
			CardCategory.Policy, new CardID("ScienceFunding")) { }

	public override bool OnTurnStart(GameManager gm) {
		if (isCharged) {
			gm.DrawPlayersCard(authorPlayer);
		}
		return true;
	}
}