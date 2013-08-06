#pragma strict

var wheelFL: WheelCollider;
var wheelFR: WheelCollider;
var wheelRL: WheelCollider;
var wheelRR: WheelCollider;

var maxTorque: float; //3800;
var maxVelocity: float;

function Start () {
this.rigidbody.centerOfMass += new Vector3(0,-3,1.0);
}

function FixedUpdate () {
	if(Input.GetAxis("Vertical")) {
	
		this.rigidbody.AddForce(Vector3.up * 100000,ForceMode.Force);
	}
	
	
	
	var velocity : float = this.rigidbody.velocity.magnitude;
	//Debug.Log(velocity);
	
	
	if(velocity < maxVelocity) {
	
	//wheelRL.motorTorque = maxTorque * Input.GetAxis("Vertical");
	
	//for (var touch : Touch in Input.touches) {
		//if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary) {
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
	//	} else {
		//	wheelFR.motorTorque = 0;
		//	wheelFL.motorTorque = 0;
		//}
	//}
	
}

