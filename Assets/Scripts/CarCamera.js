#pragma strict
var car: Transform;
var cameraOffset: float = 80;
function Start () {

}

//Executes after Update
function LateUpdate () {

	
    var targetPosition = Vector3(120, 0, cameraOffset) + car.position;
   	var targetLookat = car.position + Vector3(0,0,cameraOffset);
    transform.position = targetPosition;
	transform.LookAt(targetLookat);
}
