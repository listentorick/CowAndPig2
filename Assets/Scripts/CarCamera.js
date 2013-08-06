#pragma strict
var car: Transform;
//var distance: float = 6.4;
//var height: float = 1.4;
var positionDamping: float = 5.0;
var heightDamping: float = 2.0;
var zoomRatio: float = 100;
private var rotationVector: Vector3;

var minFOV: float = 60.0;

var maxFOV: float  = 75.0; 


function Start () {

	minFOV = 30.0f; //wtf - why do i need to set this here?
	zoomRatio = 100;

}

//Executes after Update
function LateUpdate () {

	var lookAt = car.position;
   
    //var targetPosition = Vector3(80.0, 5.0, 0) + lookAt;
     var targetPosition = Vector3(160, 0, 0) + lookAt;
   
    //var nextPosition = Vector3.Lerp(transform.position, targetPosition, positionDamping * Time.deltaTime);
    
    transform.position = targetPosition;
    
	//look towards the car
	transform.LookAt(car);
	
}

function FixedUpdate () {

	
 	//var targetFieldOfView = minFOV + car.rigidbody.velocity.z * Time.deltaTime * zoomRatio;
    
   // if(targetFieldOfView > maxFOV) {
    //	targetFieldOfView = maxFOV;
  // }
    
   // if(targetFieldOfView < minFOV) {
    //	targetFieldOfView = minFOV;
   // }
   
 
    
    //this.camera.fieldOfView = targetFieldOfView;
}
