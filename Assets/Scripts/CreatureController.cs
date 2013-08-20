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
		
		//Some creatures may not be always collectable.
		//So the creature decides if it can be collection.
		//if so, it'll throw its caught event.
		if(carController.IsCarCollider(other)) {
			if(CanCreatureBeCaught()){
				OnCaught();
			}
		}
	}
	
	public virtual bool CanCreatureBeCaught(){
		return true;
	}
	
	public int GetValue(){
		return 10;
	}
}

 public delegate void CaughtEventHandler(CreatureController sender);