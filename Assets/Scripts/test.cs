using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ClipperLib;
using System.Linq;

using Polygon = System.Collections.Generic.List<UnityEngine.Vector2>;
using Polygons = System.Collections.Generic.List<Polygon>;

public class test : MonoBehaviour {
	
	
	private System.Collections.Generic.List<Polygon> clip = new System.Collections.Generic.List<Polygon>();
	private System.Collections.Generic.List<Polygon> subj = new System.Collections.Generic.List<Polygon>();
	
	public Transform car;
	
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	private MeshCollider meshCollider;
	private float maxBufferLength = 20f;

	// Use this for initialization
	void Start () {
		
		meshFilter = GetComponent<MeshFilter>();
	 	meshRenderer = GetComponent<MeshRenderer>();
		meshCollider = GetComponent<MeshCollider>();
		
		
		UpdateCameraBounds();;
		InitialiseTunnelPath();
	}
	
	
	private Polygon surfacePolygon = new Polygon(4);
	
	private Vector3 bottomLeftViewPort;
	private Vector3 topRightViewPort;
	

	void UpdateCameraBounds() {
		bottomLeftViewPort = Camera.mainCamera.ViewportToWorldPoint(new Vector3(0,0, Camera.mainCamera.transform.position.z));
		topRightViewPort = Camera.mainCamera.ViewportToWorldPoint(new Vector3(1,1, Camera.mainCamera.transform.position.z));
		
	}
	
	void UpdateSurfacePolygon() {
		
		surfacePolygon.Clear();
		surfacePolygon.Add(new Vector2(bottomLeftViewPort.x+100f, 0));
		surfacePolygon.Add(new Vector2(topRightViewPort.x-100f, 0));	
		surfacePolygon.Add(new Vector2(topRightViewPort.x-100f, -100));
		surfacePolygon.Add(new Vector2(bottomLeftViewPort.x+100f, -100));

	}
	
	private Polygon tunnelPath = new Polygon();
	private bool tunnelPathUpdated = false;
	private bool firstRender = true;
	/// <summary>
	/// Initialises the tunnel path.
	/// </summary>
	void InitialiseTunnelPath() {
		tunnelPath.Clear();
		tunnelPath.Add(new Vector2(bottomLeftViewPort.x-500,TUNNEL_HEIGHT));
		tunnelPath.Add(new Vector2(30,TUNNEL_HEIGHT));
	}
	
	private float targetGradient;
	private const float maxDeltaAngle = Mathf.PI/8; //45 degrees!
	private float requestedGradient;
	
	private void UpdateRequestedGradient() {
		
		Vector3 target = UserInput.Instance.GetTarget();

		bool remainOnCurrentPath =false;
		if (target.x==0 && target.y==0 && target.z==0) {
			
			//this represents a null target.
			//in this case we continue to make use of any existing gradient...
			remainOnCurrentPath = true;
		} else {
		
			//calculate the gradient of this target
			//store it later use
		
			target.y = target.y + TUNNEL_HEIGHT;
			
			Vector2 lastPoint = new Vector2(car.position.z,car.position.y);
			Vector2 lastDiff = new Vector2(target.z,target.y) - lastPoint;
		
			//Lets calculate the new gradient
			if(!remainOnCurrentPath){
				requestedGradient = Mathf.Atan2(lastDiff.y,lastDiff.x);
			}
		}
		
	}
	
	
	/// <summary>
	/// Updates the tunnel path based upon the mouses position
	/// </summary>/
	void UpdateTunnelPath() {
		
		//We cant really calculate our gradients easily so.. lets ignore any changes
		if(this.tunnelPath.Count<2) return;
		
		//Updates the requested gradient based upon user input
		UpdateRequestedGradient();
		
		//Lets calculate the current gradient
		Vector2 secondToLastPoint = this.tunnelPath[this.tunnelPath.Count-2];
		Vector2 lastPoint = this.tunnelPath[this.tunnelPath.Count-1];
		
		Vector2 lastDiff = lastPoint - secondToLastPoint; 
		float currentGradient = Mathf.Atan2(lastDiff.y,lastDiff.x);
		
		//Are we with the buffer?
		//The buffer is distance in front of the car.
		//If the last point in the tunnel path is within this buffer
		//we need a new point
		
		float bufferX = car.position.x +  (maxBufferLength * Mathf.Cos(currentGradient));

		if(lastPoint.x < bufferX){
			
			//We need a new point!!
			
			//This is the point at which we care about our new gradient
			//its a this point we want to check that the last click/touched point is a reasonable gradient to apply
			
			//we want to make sure that the requested gradient isnt too big
			

			//gradient will negative if we're going down
			//gradient will be positive if we're going up.
			float deltaAngle = requestedGradient - currentGradient;
		
			bool isDeltaAngleToBig = (Mathf.Abs(requestedGradient)-Mathf.Abs(currentGradient)) > maxDeltaAngle;
		
			if(isDeltaAngleToBig){
				
				//was going up, still going up
				if(deltaAngle<0){
					requestedGradient = currentGradient - maxDeltaAngle;
				}
				else {
					requestedGradient = currentGradient + maxDeltaAngle;
				}
			} else {
				requestedGradient = requestedGradient;
			}
			
			
			float nextX = lastPoint.x + (maxBufferLength * Mathf.Cos(requestedGradient));
			float nextY = lastPoint.y + (maxBufferLength * Mathf.Tan(requestedGradient));
			
			if(nextY-TUNNEL_HEIGHT>0){
				nextY = TUNNEL_HEIGHT;
			}
			
			Vector2 nextPoint = new Vector2(nextX,nextY);
			this.tunnelPath.Add(nextPoint);

			tunnelPathUpdated = true;
	
    	}
		
		
	}
	
	private bool HasGradientChanged(Vector2 newPoint){
		float previousAngle = Vector2.Angle( this.tunnelPath[this.tunnelPath.Count-2], this.tunnelPath[this.tunnelPath.Count-1]);
		float nextAngle = Vector2.Angle( this.tunnelPath[this.tunnelPath.Count-1], newPoint);
		return previousAngle != nextAngle;
	}
	
	private void CullTunnelPath() {
		//find the first point which is less that topLeftViewPort.x and remove it!
		for(int i=tunnelPath.Count-1;  i>=0; i--){
			if(tunnelPath[i].x < (bottomLeftViewPort.x - 20f)){
				tunnelPath.RemoveRange(0,i);
				tunnelPathUpdated = true;
				break;
			}
		}
		
	}
	
	private const int TUNNEL_HEIGHT = 20;
	private const int TUNNEL_DEPTH = 10;
	
	
		
	private Polygon tunnelPolygon = new Polygon();
		
	void UpdateTunnelPolygon() {
		this.tunnelPolygon = CreateSymmetricalPolygonFromPath(tunnelPath, -TUNNEL_HEIGHT);
	}
		

	private Polygons frontPolygons = new Polygons();
		
	private Clipper clipper = new Clipper();
	void UpdateFrontPolygon(Polygon surface, Polygon tunnel) {
		
		frontPolygons.Clear();
				
		//Lets generate the polygons which represent the landscape.
		//Clipper c = new Clipper();
		clipper.AddPolygon(surface, PolyType.Subject);
		clipper.AddPolygon(tunnel, PolyType.Clip);
		bool result = clipper.Execute(ClipType.Difference, frontPolygons, 
	  	PolyFillType.EvenOdd, PolyFillType.EvenOdd);
		clipper.Clear();
	}
	
	private List<Vector3> allVerticesToRender = new List<Vector3>();
	private List<int> allTriangles = new List<int>();
	private List<List<Vector2>> adaptedPolygons =  new List<List<Vector2>>();
	private Triangulator triangulator = new Triangulator();
	
	// Update is called once per frame
	void Update () {
		
		this.tunnelPathUpdated = false;
		
	
		this.UpdateCameraBounds();
		this.UpdateTunnelPath();
		//this.CullTunnelPath();
		
		if(tunnelPathUpdated || firstRender) {
			firstRender = false;
			
			Mesh mesh = new Mesh();
			
			allVerticesToRender.Clear();
			adaptedPolygons.Clear();
			allTriangles.Clear();
			
			UpdateTunnelPolygon();
			UpdateSurfacePolygon();
			
			UpdateFrontPolygon(surfacePolygon, tunnelPolygon);
			
			
			
			Triangulator tr;
			int[] triangulatedPolys;

			int triangleOffset = 0;
			
			
			//from the Front Polygons we will build the additional face
			foreach(Polygon polygon in frontPolygons){
				
				//lets treat the polygon as a path
				//the result of this is JUST used for triangulation
				adaptedPolygons.Add(CreateSymmetricalPolygonFromPath2(polygon, TUNNEL_DEPTH));
				
				allVerticesToRender.AddRange(CreateSymmetricalPolygonFromFace(polygon,0, TUNNEL_DEPTH));
			
			}
			
			foreach(List<Vector2> adaptedPolygon in adaptedPolygons){
			
				triangulatedPolys = triangulateFloorVertices(adaptedPolygon);
				triangulatedPolys = triangulatedPolys.Select(t => {t=t+triangleOffset;return t;}).ToArray();
				allTriangles.AddRange(triangulatedPolys);
				triangleOffset += adaptedPolygon.Count;
				
			}
			
			adaptedPolygons.Clear();
			
			//Now draw the front face.

			foreach(Polygon polygon in frontPolygons){
				
				List<Vector2> adaptedPolygon = new List<Vector2>();
			  	adaptedPolygons.Add(adaptedPolygon);
				foreach(Vector2 point in polygon){
					adaptedPolygon.Add(new Vector2(point.x,point.y));
					allVerticesToRender.Add(new Vector3(point.x,point.y,0));
				}
			}
			

			foreach(List<Vector2> adaptedPolygon in adaptedPolygons){
			
				triangulatedPolys = triangulator.Triangulate(adaptedPolygon.ToArray());
				triangulatedPolys = triangulatedPolys.Select(t => {t=t+triangleOffset;return t;}).ToArray();
				allTriangles.AddRange(triangulatedPolys);
				triangleOffset += adaptedPolygon.Count;
			
			}
		
			//create the 'back'
			List<Vector3> back = new List<Vector3>();
			foreach(Vector2 v in surfacePolygon){
				back.Add(new Vector3(v.x,v.y,TUNNEL_DEPTH));
			}
			
			allVerticesToRender.AddRange(back);
			
			triangulatedPolys = triangulator.Triangulate(surfacePolygon.ToArray());
			triangulatedPolys = triangulatedPolys.Select(t => {t=t+triangleOffset;return t;}).ToArray();
			allTriangles.AddRange(triangulatedPolys);
			
			triangleOffset = allVerticesToRender.Count;
			
			allVerticesToRender.AddRange(CreateSymmetricalPolygonFromFace(surfacePolygon, TUNNEL_DEPTH, TUNNEL_DEPTH*6));
			List<Vector2> backSides = CreateSymmetricalPolygonFromPath2(surfacePolygon, TUNNEL_DEPTH);
			
			
			
			triangulatedPolys = triangulateFloorVertices(backSides);
			triangulatedPolys = triangulatedPolys.Select(t => {t=t+triangleOffset;return t;}).ToArray();
			allTriangles.AddRange(triangulatedPolys);
			//triangleOffset += adaptedPolygon.Count;
			
			
			mesh.vertices = allVerticesToRender.ToArray();
			mesh.triangles = allTriangles.ToArray();
			
			mesh.RecalculateNormals();
        	mesh.RecalculateBounds();
			
			
			meshCollider.sharedMesh = mesh; 	
			meshFilter.mesh = mesh;
		}

		
	}
	
	
	List<Vector3> CreateSymmetricalPolygonFromFace(Polygon face, float minZ, float maxZ) {
		
		
		List<Vector3> sexyPolygon = new List<Vector3>();
		foreach(Vector2 point in face){
			sexyPolygon.Add(new Vector3(point.x,point.y,minZ));
		}
		
		//add the first point AGAIN so we close this poly....
		sexyPolygon.Add(new Vector3(face[0].x,face[0].y,minZ));
		sexyPolygon.Add(new Vector3(face[0].x,face[0].y,maxZ));
		
		//Initialise the polygon with the path we have
		Vector2 currentPoint;
		//We need to create the mirror of each point
		for(int i=face.Count-1;  i>=0; i--){
			currentPoint = face[i];
			sexyPolygon.Add(new Vector3(currentPoint.x, currentPoint.y,maxZ));
		}
		
		return sexyPolygon;
	}
	
	
	/// <summary>
	/// Helper to create a symmetrical polygon based upon a path. 
	/// </summary>
	/// <returns>
	/// The symmetrical polygon from path.
	/// </returns>
	/// <param name='path'>
	/// Path.
	/// </param>
	/// <param name='offset'>
	/// Offset.
	/// </param>
	Polygon CreateSymmetricalPolygonFromPath2(Polygon path, float yOffset) {
		
		Polygon face = new Polygon();
		
		foreach(Vector2 p in path) {
			face.Add(new Vector2(p.x,p.y + (long)yOffset));
		}
		//Initialise the polygon with the path we have
		//Polygon face = new Polygon(path);
		
		face.Add(new Vector2(path[0].x,path[0].y+ (long)yOffset));
		face.Add(new Vector2(path[0].x,path[0].y ));
		
		Vector2 currentPoint;
		//We need to create the mirror of each point
		for(int i=path.Count-1;  i>=0; i--){
			currentPoint = path[i];
			face.Add(new Vector2(currentPoint.x, currentPoint.y ));
		}
		
		return face;
	}
	
	Polygon CreateSymmetricalPolygonFromPath(Polygon path, float yOffset) {
 		

 		//Initialise the polygon with the path we have
		Polygon face = new Polygon(path);

 		
		//face.Add(new Vector2(path[0].x,path[0].y));
//		face.Add(new Vector2(path[0].x,path[0].y + (long)yOffset));

 		
 		Vector2 currentPoint;
 		//We need to create the mirror of each point
 		for(int i=path.Count-1;  i>=0; i--){
 			currentPoint = path[i];
			face.Add(new Vector2(currentPoint.x, currentPoint.y + (long)yOffset));

 		}
 		
 		return face;
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
}


	
