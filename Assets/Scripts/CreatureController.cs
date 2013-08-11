using UnityEngine;
using System.Collections;

public class CreatureController : BaseController {
	
	public CarController carController;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public event CaughtEventHandler Caught;
	
	protected virtual void OnCaught() 
    {
         if (Caught != null)
            Caught(this);
    }
	
	void OnTriggerEnter (Collider other ) {
		
		//the barn is responsible for asking the truck if it has any little creatures.
		//if so it should remove them from the car...
		if(carController.collider == other) {
			
			OnCaught();
			//Debug.Log("WOOP");
		}
	}
}

 public delegate void CaughtEventHandler(CreatureController sender);