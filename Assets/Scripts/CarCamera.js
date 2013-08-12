#pragma strict
var car: Transform;
var cameraOffset: float = 80;
function Start () {

}

//Executes after Update
function LateUpdate () {

	
    var targetPosition = Vector3(cameraOffset, 0, -120) + car.position;
   	var targetLookat = car.position + Vector3(cameraOffset,0,0);
   	
   	// var targetPosition = Vector3(0, 30, 0) + car.position;
   	//var targetLookat = car.position;
   	
    transform.position = targetPosition;
	transform.LookAt(targetLookat);
}
