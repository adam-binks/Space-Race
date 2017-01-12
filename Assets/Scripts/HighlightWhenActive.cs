using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MouseTargetable))]
public class HighlightWhenActive : MonoBehaviour {

	public Color highlightColour = new Color(144, 255, 159); // default to light green
	public SpriteRenderer spriteRenderer;

	private Color normalColour;
	private MouseTargetable mouseTargetable;


	void Start () {
		normalColour = spriteRenderer.color;

		mouseTargetable = GetComponent<MouseTargetable>();
		mouseTargetable.OnTGNewlyAddedActions.Add(TurnOnHighlight);
		mouseTargetable.OnTGNewlyRemovedActions.Add(TurnOffHighlight);
		mouseTargetable.Refresh(); // set the highlight going if valid
	}
	
	void TurnOnHighlight() {
		spriteRenderer.color = highlightColour;
	}

	void TurnOffHighlight() {
		spriteRenderer.color = normalColour;
	}
}
