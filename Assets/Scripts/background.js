#pragma strict

var  backgroundMaterial : Material;
var width: int = 4096;
var height: int = 2720;
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
       
       // (GetComponent(MeshRenderer) as MeshRenderer).material.mainTextureOffset =  Vector2 (0, -0.6);
        
         //(GetComponent(MeshRenderer) as MeshRenderer).material.mainTextureScale=  Vector2 (2, 2);
         //(GetComponent(MeshRenderer) as MeshRenderer).material.mainTexture.wrapMode = TextureWrapMode.Repeat;

    	
}

function Update () {
	
	this.transform.position.z = 800;
		this.transform.position.y = Camera.main.transform.position.y - height/2;
		this.transform.position.x = Camera.main.transform.position.x - width/2;
	
}

@script RequireComponent(MeshFilter)
@script RequireComponent(MeshRenderer)
