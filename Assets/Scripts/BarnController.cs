using UnityEngine;
using System.Collections;

public class BarnController : BaseController {
	
	public CarController carController;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	

	public event BarnEnterEventHandler BarnEnter;
	
	protected virtual void OnBarnEnter() 
    {
         if (BarnEnter != null)
            BarnEnter(this);
    }
	
	
	void OnTriggerEnter (Collider other ) {
		
		//the barn is responsible for asking the truck if it has any little creatures.
		//if so it should remove them from the car...
		if(carController.IsCarCollider(other)) {
			OnBarnEnter();
		}
	}
}

 public delegate void BarnEnterEventHandler(BarnController sender);
