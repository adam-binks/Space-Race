using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingLayerManager : MonoBehaviour {

	private List<SpriteRenderer> SRs;
	private List<Canvas> canvases;


	public void Setup () {
		// get all the SpriteRenderers on this object and its children
		SRs = new List<SpriteRenderer>(GetComponents<SpriteRenderer>());
		SRs.AddRange(new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>()));

		// get all the Canvases on this object and its children
		canvases = new List<Canvas>(GetComponents<Canvas>());
		canvases.AddRange(new List<Canvas>(GetComponentsInChildren<Canvas>()));
	}
	
	public void SetSortingLayer(string sortingLayer) {
		foreach (SpriteRenderer SR in SRs) {
			SR.sortingLayerName = sortingLayer;
		}

		foreach(Canvas c in canvases) {
			c.sortingLayerName = sortingLayer;
		}
	}
}
