#pragma strict

var  backgroundMaterial : Material;
var width: int = 1024;
var height: int = 680;
function Start () {
		
		//var size = 1000;
		var vertices : Vector3[] = [
			Vector3(0,0,0),
			Vector3(0,height,0),
			Vector3(width,height,0),
			Vector3(width,0,0)
		];
		
		var triangles : int[] = [
			0,1,2,
			0,2,3
		];
		
		var mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
       mesh.uv = [
          // Vector2(1,0),
		//	Vector2(1,1),
		//	Vector2(0,1),
		//	Vector2(0,0)
		
		Vector2(0,0),
			Vector2(0,1),
			Vector2(1,1),
			Vector2(1,0)
        ];
        mesh.RecalculateNormals();

        (GetComponent(MeshFilter) as MeshFilter).mesh = mesh;   
      //  (GetComponent(MeshCollider) as MeshCollider).mesh = mesh; 	
        (GetComponent(MeshRenderer) as MeshRenderer).material = backgroundMaterial;
        
        

    	
}

function Update () {
	
	this.transform.position.x = -450;
		this.transform.position.y = Camera.main.transform.position.y - height/2;
		this.transform.position.z = Camera.main.transform.position.z - width/2;
	
}

@script RequireComponent(MeshFilter)
@script RequireComponent(MeshRenderer)
