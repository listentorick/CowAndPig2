using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarController : BaseController {
	
	public Collider bottomCollider;
	public Collider topCollider;
	public WheelCollider wheelFL;
	public WheelCollider wheelFR;
	public WheelCollider wheelRL;
	public WheelCollider wheelRR;
	
	public float maxTorque;
	public float maxVelocity;
	private IList<CreatureController> cargo = new List<CreatureController>();
	
	
	// Use this for initialization
	void Start () {
		this.rigidbody.centerOfMass += new Vector3(0f,-3f,1.0f);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		float velocity = this.rigidbody.velocity.magnitude;

		if(velocity < maxVelocity) {
			wheelRR.motorTorque = maxTorque;
			wheelRL.motorTorque = maxTorque;
			wheelFR.motorTorque = maxTorque;
			wheelFL.motorTorque = maxTorque;
			
		} else {
			wheelRR.motorTorque = 0;
			wheelRL.motorTorque = 0;
			wheelFR.motorTorque = 0;
			wheelFL.motorTorque = 0;
		}
	
	}
	
	public bool IsCarCollider(Collider collider) {
		return collider==bottomCollider || collider ==topCollider;
	}
	
	public bool CanAddCreatureToCargo(CreatureController c){
		return true;
	}
	
	public void AddCreatureToCargo(CreatureController c){
		cargo.Add(c);
	}
	
	public IList<CreatureController> GetCargo(){
		return cargo;
	}
	
	public void ClearCargo() {
		cargo.Clear();
	}
}
