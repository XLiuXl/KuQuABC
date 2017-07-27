using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;



/// <summary>
/// This class allows us to manage the third tutorial scene
/// </summary>
public class InstallDatabase : MonoBehaviour
{
	public UnityEngine.Object nextSceneToLoad;
	
	public RawImage video;
	private RectTransform videoRect;
	private RectTransform pauseRect;
	
	private SpriteRenderer loader;
	public float timeBeforeStart = 1f;
	
	private string pathToVideoFolder = "";
	private UnityEngine.Object[] videoPictures = null;
	private bool play = false;
	private bool pause = false;
	private bool loop = true;
	public float videoFrameRate = 5f;
	private int pictureCounter = 0;
	private DateTime previousPictureTime;
	private List<string> framesToPause = new List<string> { "00", "12", "16", "22",  "46",  "54",  "61", "88" };
	
	void Start ()
	{
		// Get the UI components
		loader = GameObject.Find("Loader").GetComponent<SpriteRenderer>();
		pauseRect = GameObject.Find("Pause").GetComponent<RectTransform>();
		
		// Get the video utils
		videoRect = video.gameObject.GetComponent<RectTransform>();
		pathToVideoFolder = "Step 8 video";
		
		// Start the verification
		StartCoroutine(sendFilesOnServer());
	}
	
	public void NextInstallationStep()
	{
		UtilsProSecure.Load(nextSceneToLoad.name);
	}
	public void PausePlay()
	{
		if(play)
		{
			pause = !pause;
			if(pause)
			{
				pauseRect.localScale = Vector3.one;
			}
			else
			{
				pauseRect.localScale = Vector3.zero;
			}
		}
	}
	
	private IEnumerator sendFilesOnServer()
	{
		// Wait for the customer to know what we are testing here
		yield return new WaitForSeconds(timeBeforeStart);
		loader.color = new Color (255, 255, 255, 0);
		
		// Then play the video
		Play();
	}
	
	private void showLoader()
	{
		loader.color = new Color (255, 255, 255, 1);
	}
	private void hideLoader()
	{
		loader.color = new Color (255, 255, 255, 0);
	}
	private void showVideo()
	{
		videoRect.localScale = Vector3.one;
	}
	private void hideVideo()
	{
		videoRect.localScale = Vector3.zero;
	}
	
	private void Play()
	{
		play = false;
		hideLoader ();
		videoPictures = Resources.LoadAll(pathToVideoFolder);
		pictureCounter = 0;
		previousPictureTime = DateTime.Now;
		showVideo ();
		play = true;
	}
	
	void Update ()
	{
		if(play && !pause)
		{
			DateTime now = DateTime.Now;
			if((now - previousPictureTime).TotalMilliseconds > videoFrameRate)
			{
				previousPictureTime = now;
				Texture2D textureToUse = (Texture2D) videoPictures[pictureCounter];
				video.texture = textureToUse;
				pictureCounter++;
				
				// If the frame must be freezed, pause the video until next click
				if(framesToPause.Contains(textureToUse.name))
				{
					PausePlay();
				}
			}
			
			if(pictureCounter >= videoPictures.Length)
			{
				if(loop)
				{
					pictureCounter = 0;
				}
				else
				{
					play = false;
					hideVideo();
					showLoader();
				}
			}
		}
	}
}
