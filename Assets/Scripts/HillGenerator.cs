using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HillGenerator : MonoBehaviour {
	
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	private MeshCollider meshCollider;
	

	// Use this for initialization
	void Start () {
		r = new Random();
		hillPath.Add(new Vector2(0,0));
		meshFilter = GetComponent<MeshFilter>();
	 	meshRenderer = GetComponent<MeshRenderer>();
		UpdateCameraBounds();
		
		
	}
	
	
	
	// Update is called once per frame
	private bool hillPathUpdated = false;
	private List<Vector2> hillPath = new List<Vector2>();
	private Vector2 bottomLeftViewPort;
	private Vector2 topRightViewPort;
	
	private Random r;
	private  int rangeDX = 16;
    private int rangeDY = 4;
    private float minDX = 8;
    private float minDY = 4;
    private float sign = 1;
	

	
	private Triangulator triangulator = new Triangulator();
	
	void Update () {
		
		UpdateCameraBounds();
		UpdateHillPath();
		if(hillPathUpdated) {
			
			List<Vector2> face = GenerateHillFace(hillPath);
			List<Vector3> polygon = GenerateHillPolygon(face);
			
			int[] triangles =triangulateFloorVertices(face);
			
			Mesh mesh = new Mesh();
			
			mesh.vertices = polygon.ToArray();
			mesh.triangles = triangles;
			
			mesh.RecalculateNormals();
        	mesh.RecalculateBounds();
			
			meshFilter.mesh = mesh;

		}
	}
	
	
	void UpdateCameraBounds() {
		
		
		bottomLeftViewPort = Camera.mainCamera.ViewportToWorldPoint(new Vector3(0,0, Camera.mainCamera.transform.position.z - this.transform.position.z));
		topRightViewPort = Camera.mainCamera.ViewportToWorldPoint(new Vector3(1,1, Camera.mainCamera.transform.position.z - this.transform.position.z));	
	}
	
	void UpdateHillPath() {
		hillPathUpdated = false;
		while(NeedNewPoint()) {
			hillPathUpdated = true;
			hillPath.Add(GenerateNextPathPoint(hillPath[hillPath.Count-1],nextSign()));
		}
	}
	
	private Vector2 GenerateNextPathPoint(Vector2 previousVector, float sign) {
		float x = previousVector.x + Random.value%rangeDX+minDX;
		float y = previousVector.y + ( Random.value%rangeDY+minDY)*sign;
		if(y<0) {
			y = 0;
		}
		return new Vector2(x,y);
	}
	
	private List<Vector2> hillFace = new List<Vector2>();
	
	private List<Vector2> GenerateHillFace(List<Vector2> hillPath){
		
		hillFace.Clear();
		
		foreach(Vector2 p in hillPath) {
			hillFace.Add(p);
		}
		
		//Initialise the polygon with the path we have
		Vector2 currentPoint;
		//We need to create the mirror of each point
		for(int i=hillFace.Count-1;  i>=0; i--){
			currentPoint = hillPath[i];
			hillFace.Add(new Vector2(currentPoint.x, 0));
		}
		
		//hillFace.Reverse();
		
		return hillFace;
	}
	
	private List<Vector3> hillPolygon = new List<Vector3>();
	
	private List<Vector3> GenerateHillPolygon(IList<Vector2> hillFace){
		hillPolygon.Clear();
		
		//for(int i=hillFace.Count-1;  i>=0; i--){
		foreach(Vector2 p in hillFace) {
			hillPolygon.Add(new Vector3(p.x,p.y,0));
			//hillPolygon.Add(new Vector3(hillFace[i].x,hillFace[i].y,0));
		}
		return hillPolygon;
	}
	
	private float nextSign(){
		int index =  (int)(Random.value * 2);
		if(index ==0) {
			return -1;
		}else {
			return 1;
		}
	}
	
	bool NeedNewPoint(){
		//is the last point within the bufferZone?
		//return hillPath[hillPath.Count-1].x<bottomLeftViewPort.x;	
		
		
		Vector3 worldPosition = new Vector3(hillPath[hillPath.Count-1].x, hillPath[hillPath.Count-1].y,this.transform.position.z);
		Vector3 pos = Camera.main.WorldToViewportPoint(worldPosition);
		
		//cube.transform.position = worldPosition;
		
		return !(pos.x >1);
		//amera.main.WorldToViewportPoint();
	}
	
	
	int[] triangulateFloorVertices(List<Vector2> verticesToTriangulate) {
    
    	//some assumptions..
    	
    	//imagine 6 points.
    	// .   .   .
    	//
    	// .   .   .
    	
    	//the triangle would be
    	//0,1,4
    	//1,2,4
  
    	//5,0,4
    	//4,2,3
    	    	
    	int lastPointToTriangulate  = (verticesToTriangulate.Count/2);
    	int lastPointIndex = verticesToTriangulate.Count - 1;
    	int numTriangles  = lastPointToTriangulate * 6;

    	int[] triangles = new int[numTriangles];
    	int currentIndex  = 0;
		bool isEven;
    	for(int index = 0; index < lastPointToTriangulate - 1; index ++ ){
    		
    		
    		isEven = index % 2==0;
    	
    		//top triangle
	    	triangles[currentIndex] = index;
	    	currentIndex++;
	    	triangles[currentIndex] = index + 1;
    		currentIndex++;
    		
    		if(isEven) {
	    		triangles[currentIndex] = lastPointIndex - (index+1);	
	    	} else {
	    		triangles[currentIndex] = lastPointIndex - index;	
	    	}
	    	
	    	
	    	//bottom triangle
	    	currentIndex++;
	    	
	    	triangles[currentIndex] = lastPointIndex - index;
	    	currentIndex++;
	    	
    		if(isEven) {
	    		triangles[currentIndex] = index;	
	    	} else {
	    		triangles[currentIndex] = index + 1;	
	    	}
	    	currentIndex++;
	    	triangles[currentIndex] = lastPointIndex - (index + 1);
	    	currentIndex++;
	    	
    	}
    	
    	return triangles;
    }
	//Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

 

//if(pos.x < 0.0) Debug.Log("I am left of the camera's view.");

//if(1.0 < pos.x) Debug.Log("I am right of the camera's view.");
	
//if(pos.y < 0.0) Debug.Log("I am below the camera's view.");

//if(1.0 < pos.y) Debug.Log("I am above the camera's view.");
}
