using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public enum textStyle {
	Title,
	Description
}

public class TextRender : MonoBehaviour {
	public GameObject renderArea; // The camera, text etc GOs used to generate the text texture2D
	public RenderTexture renderTex;
	public Camera renderCamera;
	public Text titleText;


	public IEnumerator GenerateTextSprite(string text, textStyle style) {
		RenderTexture currentRT = RenderTexture.active;
		RenderTexture.active = renderTex;

		renderArea.SetActive(true);

		// TODO textStyles
		titleText.text = text;

		yield return new WaitForEndOfFrame();

		// create a new texture2D and read the pixels of the renderCamera onto it
		Texture2D tex = new Texture2D(1000, 
									  1000, 
									  TextureFormat.ARGB32, 
									  false);
		//tex.ReadPixels(renderCamera.pixelRect, 0, 0);
		tex.ReadPixels(new Rect(0, 0, 1000, 1000), 0, 0);
		tex.Apply();

		// revert to previous active render tex
		RenderTexture.active = currentRT;
		
		renderArea.SetActive(false);

		// Encode texture into PNG
		byte[] bytes = tex.EncodeToPNG();

		// For testing purposes, also write to a file in the project folder
		File.WriteAllBytes(Application.dataPath + "/SavedScreen.png", bytes);

		// Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
	}

}
