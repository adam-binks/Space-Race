using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Inherits from Card to allow Hand etc to work for local and enemy hand.
/// Looks like the back of a card, but doesn't contain its actual contents as an anti cheat measure
public class ConcealedCard : Card {

	/// Create a Card and put it exactly where this one is, then flip it. 
	/// Should create the illusion that this was a real card all along :3
	public void Reveal(CardID ID, GameManager gm) {
		Card c = gm.CreateCardGO(ID);
		c.transform.localPosition = transform.localPosition;
		c.FlipFaceDown(false); // set rotation to face down
		c.FlipFaceUp(true); // then tween back the right way up
	}

}
