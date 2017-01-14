using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum cardCategory {
	Policy,
	Operative,
	None
}


public class CardTemplate {   // << not a monobehaviour!
	public string cardName;
	public string description;
	public int playCost;
	public int chargeCost;
	public cardCategory cat;
	public CardID ID;

	public CardTemplate(string cardName,
						string description,
						int playCost,
						int chargeCost,
						cardCategory category,
						CardID ID) {
		this.cardName = cardName;
		this.description = description;
		this.playCost = playCost;
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
	: base("Science Funding", "Players draw two cards at the start of each turn", 3, 2, 
			cardCategory.Policy, new CardID("ScienceFunding")) { }

	public override bool OnTurnStart(GameManager gm) {
		// TODO: if charged, draw an extra card
		// gm.addAction(new Action(actionType.drawCard));
		return true;
	}
}