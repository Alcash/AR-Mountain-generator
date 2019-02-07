using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RC_UI_Texture : MonoBehaviour {

	private Texture UI_Texture;
	public Camera RenderCamera;
	public RawImage UI_Image;


	void Start () {
		StartCoroutine(WaitForTexture());
	}
		

	private IEnumerator WaitForTexture() 
	{
		yield return new WaitForEndOfFrame ();

		if (RenderCamera && RenderCamera.targetTexture)
		{
			UI_Image.texture = RenderCamera.targetTexture;
			UI_Image.rectTransform.sizeDelta = new Vector2(RenderCamera.targetTexture.width/2, RenderCamera.targetTexture.height/2);
		}

		else StartCoroutine(WaitForTexture());
	}
}
