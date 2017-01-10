using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// For MouseTargetable objects. 
/// When hovered, and this TargetingGroup is active, highlight the sprite
public class HighlightOnActiveAndHover : MonoBehaviour {

	public Color highlightColour;
	public SpriteRenderer sr;
	
	private Color normalColour;
	void Start () {
		normalColour = sr.color;
		//sr.color = Color.blue;
	}
	
	void OnNewlyTargetable() {
		sr.color = highlightColour;
		Debug.Log("highlight on");
	}

	void OnNoLongerTargetable() {
		sr.color = normalColour;
		Debug.Log("highlight off");
	}
}
