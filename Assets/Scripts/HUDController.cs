using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {

	public GUISkin guiSkin;
		
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private int score = 0;
	private int numPigsCollected = 0;
	
	public void CreatureReturnedToBarn(CreatureController creatureController){
		
		score = score + creatureController.GetValue();
	}
	
	void OnGUI ()
    {
    	GUI.skin = guiSkin;
    	GUI.Label (new Rect (300, 25, 1000, 40), score.ToString());
    }
}
