

var wheel : WheelCollider;

function Update () {

    var hit : RaycastHit;
    
    
    var wheelPos: Vector3;
    
    if ( Physics.Raycast ( wheel.transform.position, -wheel.transform.up, hit,  wheel.suspensionDistance + wheel.radius ) ) {

    	wheelPos = hit.point + (wheel.transform.up * wheel.radius);

    } else {

    	wheelPos = wheel.transform.position - wheel.transform.up * wheel.suspensionDistance;

    }
    
    this.transform.position = wheelPos;

}
