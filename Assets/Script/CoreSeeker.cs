using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Vuforia;

public class CoreSeeker : MonoBehaviour, ITrackableEventHandler
{



    public Camera RenderCamera;
    [Space(20)]
    bool FreezeEnable = false;

    int squadCount = 8;

    public UnityEngine.UI.Image img;
   
   
    TrackableBehaviour mTrackableBehaviour;

    public UnityAction<Texture2D> OnCoreFound;

    float heightBox = 0.0234f;

    void Start()
    {
       
        if (FreezeEnable && RenderCamera) RenderCamera.enabled = false;

        ParsingMazeArray();

        mTrackableBehaviour = transform.parent.GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

    }

    void ParsingMazeArray()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject go = transform.GetChild(i).gameObject;
            //string[] arrayName = go.name.Split(' ');

            // int indexX = int.Parse(arrayName[2]);
            // int indexY = int.Parse(arrayName[3].Substring(1,1));
            // mazeArray[indexX + indexX* indexY] = go;
          
        }


    }

    private IEnumerator WaitForTexture()
    {
        while (!RenderCamera.targetTexture)
        {
            Debug.Log("Wait for RenderTexture");
            yield return new WaitForSeconds(0.01f);
            yield return new WaitForEndOfFrame(); ;
        }


        //GetComponent<Renderer>().material.SetTexture("_MainTex", RenderCamera.targetTexture);
        StartParsingTexture();
    }

    public void StartParsingTexture()
    {


        StartCoroutine(ParsingColorMaze());

    }

    IEnumerator ParsingColorMaze()
    {
        yield return new WaitForSeconds(0.1f);

        RenderTexture rTex = RenderCamera.targetTexture;
        RenderTexture.active = rTex;
        Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        yield return null;
        // Texture2D text = new Texture2D(tex.width - 90, tex.height - 90);

        // Color[] workColor = tex.GetPixels(40, 40, text.width, text.height);       

        Texture2D text = new Texture2D(tex.width, tex.height);

        Color[] workColor = tex.GetPixels(0, 0, text.width, text.height);       

        text.SetPixels(0, 0, text.width, text.height, workColor);
        text.Apply();

        Sprite sprite = Sprite.Create(text, new Rect(0, 0, text.width, text.height), new Vector2(.5f, .5f));// обрезка лишней черной части

        img.sprite = sprite; //Image is a defined reference to an image component       

        byte[] textureByte = img.sprite.texture.GetRawTextureData();
        
        if (OnCoreFound != null)
        {
            OnCoreFound(img.sprite.texture);
        }     
               
    }

   

    Color GetAvarageColor(Texture2D texture, int x, int y)
    {
        int core = 5;
        float cRed = 0, cBlue = 0, cGreen = 0;
        for (int i = 0; i < core; i++)
        {
            for (int j = 0; j < core; j++)
            {
                Color c = texture.GetPixel(x - core + i, y - core + j);
                cRed += c.r;
                cBlue += c.b;
                cGreen += c.g;
            }
        }
        cRed /= core * core;
        cBlue /= core * core;
        cGreen /= core * core;

        Color result = new Color(cRed, cGreen, cBlue);

        return result;
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            //Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
            if (RenderCamera) StartCoroutine(WaitForTexture());
            Debug.Log("Trackable " + " found");
            //StartParsingTexture();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NOT_FOUND)
        {
            //Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
            //OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            //OnTrackingLost();
        }
    }
}
