using UnityEngine;
using System.Collections;

public class CarController : BaseController {
	
	public WheelCollider wheelFL;
	public WheelCollider wheelFR;
	public WheelCollider wheelRL;
	public WheelCollider wheelRR;
	
	public float maxTorque;
	public float maxVelocity;

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
}
