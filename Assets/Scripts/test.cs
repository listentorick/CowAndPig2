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

	// Use this for initialization
	void Start () {
		
		meshFilter = GetComponent<MeshFilter>();
	 	meshRenderer = GetComponent<MeshRenderer>();
		meshCollider = GetComponent<MeshCollider>();
		
		
		UpdateCameraBounds();;
		InitialiseTunnelPath();
	}
	
	
	private Polygon surfacePolygon = new Polygon(4);
	
	private Vector3 topLeftViewPort;
	private Vector3 topRightViewPort;
	

	void UpdateCameraBounds() {
		topLeftViewPort = Camera.mainCamera.ViewportToWorldPoint(new Vector3(0,1, Camera.mainCamera.transform.position.x));
		topRightViewPort = Camera.mainCamera.ViewportToWorldPoint(new Vector3(1,1, Camera.mainCamera.transform.position.x));
	}
	
	void UpdateSurfacePolygon() {
			
		surfacePolygon.Clear();
		surfacePolygon.Add(new Vector2(topLeftViewPort.z, 0));
		surfacePolygon.Add(new Vector2(topRightViewPort.z, 0));	
		surfacePolygon.Add(new Vector2(topRightViewPort.z, -1000));
		surfacePolygon.Add(new Vector2(topLeftViewPort.z, -1000));

	}
	
	private Polygon tunnelPath = new Polygon();
	private bool tunnelPathUpdated = false;
	
	/// <summary>
	/// Initialises the tunnel path.
	/// </summary>
	void InitialiseTunnelPath() {
		tunnelPath.Clear();
		tunnelPath.Add(new Vector2(topLeftViewPort.z-500,TUNNEL_HEIGHT));
		tunnelPath.Add(new Vector2(0,TUNNEL_HEIGHT));
	}
	
	private Vector3 target;
	private Vector3 lastTarget;
	
	/// <summary>
	/// Updates the tunnel path based upon the mouses position
	/// </summary>/
	void UpdateTunnelPath() {
			
		target = UserInput.Instance.GetTarget();
		
		bool remainOnCurrentPath =false;
		if (target.x==0 && target.y==0 && target.z==0) {;
			remainOnCurrentPath = true;
		}
				
		float nextY = 0;
		
		if(this.tunnelPath.Count<2) return;
		
		int secondToLastPointIndex = this.tunnelPath.Count-2;
		Vector2 secondToLastPoint = this.tunnelPath[this.tunnelPath.Count-2];
		Vector2 lastPoint = this.tunnelPath[this.tunnelPath.Count-1];
		Vector2 newPoint;
		float lastY = lastPoint.y;
    	float dY;
		
		Vector2 diff = lastPoint - secondToLastPoint; 
		
		float angle = Mathf.Atan2(diff.y,diff.x);
		float bufferZ = car.position.z +  (20f * Mathf.Cos(angle));
		float nextZ  = bufferZ + 1;
		
		Vector2 targetAsVector2 = new Vector2(target.z,target.y);
		
		if(lastPoint.x < bufferZ){
			
			//whats the new gradient?
			diff = targetAsVector2 - lastPoint; 
			angle = Mathf.Atan2(diff.y,diff.x);
			
			nextZ = lastPoint.x + (20f * Mathf.Cos(angle));
			nextY = lastPoint.y + (20f * Mathf.Tan(angle));
			//our new x and y are...
			
			//The last 
			
			if(remainOnCurrentPath) {
				
				//if(lastTarget==null){
					nextY = lastY;
					nextZ = lastPoint.x + 20f;
		//		} else {
					//nextZ = lastTarget.x + 20f;
					//nextY = lastTarget.y;
		//		}
								
			} else {

				if(nextY-TUNNEL_HEIGHT>0){ //
					nextY = TUNNEL_HEIGHT;
				}
			}
			
			newPoint = new Vector2(nextZ,nextY);
			
			//newPoint = Vector2.Lerp(lastPoint,newPoint,0.5f);
			
			//if(!isFlat || HasGradientChanged(newPoint)){
				this.tunnelPath.Add(newPoint);
			//} else {
			//	Debug.Log("updating old point");
				//just update the old last point...
			//	this.tunnelPath[this.tunnelPath.Count-1] = newPoint;
			//}
			
		
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
			if(tunnelPath[i].x < (topLeftViewPort.z - 20f)){
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
		
	private List<Vector2> tunnelFloorPolygon = new List<Vector2>();
	private List<Vector3> tunnelFloorPolygonToRender = new List<Vector3>();
	
	
	void UpdateTunnelFloorPolygon() {
		
		tunnelFloorPolygonToRender.Clear();
	
		tunnelFloorPolygon.Clear();
		
		foreach(Vector2 p in tunnelPath) {
			tunnelFloorPolygon.Add(new Vector2(p.x,TUNNEL_DEPTH));
			tunnelFloorPolygonToRender.Add(new Vector3(p.x, p.y-TUNNEL_HEIGHT,TUNNEL_DEPTH));
		}
		
		for(int i=tunnelPath.Count-1;  i>=0; i--){
			tunnelFloorPolygon.Add(new Vector2(tunnelPath[i].x, 0));
			tunnelFloorPolygonToRender.Add(new Vector3(tunnelPath[i].x, tunnelPath[i].y-TUNNEL_HEIGHT,0));
		}	
	}
	
	private List<List<Vector2>> ceilingPolygons = new List<List<Vector2>>();
	private List<Vector3> ceilingVerticesToRender = new List<Vector3>();
	
	void UpdateTunnelCeilingPolygon() {
		
		ceilingVerticesToRender.Clear();
		ceilingPolygons.Clear();

		//First lets seperate out our paths
		List<Vector2> currentPolygon = new List<Vector2>();
		//List<Vector3> currentPolygonToRender = new List<Vector3>();
		//ceilingVerticesToRender.Add(currentPolygonToRender);
		ceilingPolygons.Add(currentPolygon);
		bool foundBoundary = false;
		//Vector2 lastPoint = tunnelPath[tunnelPath.Count-1];
		foreach(Vector2 p in tunnelPath) {
			
			if(p.y>=0){ //make sure the ceiling stops once we get to ground level
				
				//need to add a point for where boundary was crossed...
				
				foundBoundary = true;
			} else {
				
				//if(foundBoundary==true || p==lastPoint){
					
					//next add the bottom to our current path
				//	for(int i=currentPolygon.Count-1;  i>=0; i--){
				//		currentPolygon.Add(new Vector2(currentPolygon[i].x, 0));
				//		ceilingVerticesToRender.Add(new Vector3(currentPolygon[i].x, p.y,0));
				//	}
					
				//	currentPolygon.Reverse();
				//}
					//reverse the path so the normal points down.
					//currentPolygon.Reverse();
					//currentPolygonToRender.Reverse();
					
				if(foundBoundary==true){
						
					foundBoundary = false;
					
					//We only need to create a new poly if nothings being added to the previous one...
					if(currentPolygon.Count>0){
						currentPolygon = new List<Vector2>();
						ceilingPolygons.Add(currentPolygon);
					}
				}
				
				//add the point
				currentPolygon.Add(new Vector2(p.x,p.y));
				//ceilingVerticesToRender.Add(new Vector3(p.x, p.y,TUNNEL_DEPTH));
			}
			
		}
		
		
		foreach(List<Vector2> path in ceilingPolygons){
		
			//add vertices for current paths
			foreach(Vector2 v in path){
				ceilingVerticesToRender.Add(new Vector3(v.x,v.y,TUNNEL_DEPTH));
				//v.y = TUNNEL_DEPTH;
			}
			//int i=path.Count-1;
			//add closest points
			for(int i=path.Count-1;  i>=0; i--){
				path.Add(new Vector2(path[i].x, 0));
				
				ceilingVerticesToRender.Add(new Vector3(path[i].x, path[i].y,0));
				//path[i].y = TUNNEL_DEPTH;
				path[i] = new Vector2(path[i].x,TUNNEL_DEPTH);
			}
			
			
			
			
			path.Reverse();
		}
		
		
		ceilingVerticesToRender.Reverse();
	}
	
	private List<Vector2> tunnelWallPolygon = new List<Vector2>();
	private List<Vector3> tunnelWallPolygonToRender = new List<Vector3>();

	void UpdateTunnelWallPolygon() {
		
		tunnelWallPolygonToRender.Clear();
		float y =0;
		foreach(Vector2 p in tunnelPolygon) {
			if(p.y > 0) {
				y=0;
			} else {
				y = p.y;
			}
			tunnelWallPolygonToRender.Add(new Vector3(p.x, y,TUNNEL_DEPTH));
		}
			
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
		
		if(tunnelPathUpdated) {
			
			allVerticesToRender.Clear();
			adaptedPolygons.Clear();
			allTriangles.Clear();
			
			UpdateTunnelPolygon();
			UpdateSurfacePolygon();
			UpdateTunnelFloorPolygon();
			UpdateFrontPolygon(surfacePolygon, tunnelPolygon);
			UpdateTunnelWallPolygon();
			UpdateTunnelCeilingPolygon();
					
			//Convert to list of list of vector2 rather than points		
			
			
			foreach(Polygon polygon in frontPolygons){
				
				List<Vector2> adaptedPolygon = new List<Vector2>();
			  	adaptedPolygons.Add(adaptedPolygon);
				foreach(Vector2 point in polygon){
					adaptedPolygon.Add(new Vector2(point.x,point.y));
					allVerticesToRender.Add(new Vector3(point.x,point.y,0));
				}
			}
			

			Mesh mesh = new Mesh();
			Triangulator tr;
			int[] triangulatedPolys;
			
			//List<int> allTriangles = new List<int>();
			
			int triangleOffset = 0;
			
			foreach(List<Vector2> adaptedPolygon in adaptedPolygons){
			
				triangulatedPolys = triangulator.Triangulate(adaptedPolygon.ToArray());
				triangulatedPolys = triangulatedPolys.Select(t => {t=t+triangleOffset;return t;}).ToArray();
				allTriangles.AddRange(triangulatedPolys);
				triangleOffset += adaptedPolygon.Count;
			}
			
			//Now lets add the floor polygon...
			allVerticesToRender.AddRange(tunnelFloorPolygonToRender);
			
			//now we triangulate the floor..
			triangulatedPolys = triangulateFloorVertices(tunnelFloorPolygon);
			triangulatedPolys = triangulatedPolys.Select(t => {t=t+triangleOffset;return t;}).ToArray();
			allTriangles.AddRange(triangulatedPolys);
			triangleOffset = allVerticesToRender.Count;
			
					
			//Now triangulate the back wall
			allVerticesToRender.AddRange(tunnelWallPolygonToRender);
			triangulatedPolys = triangulateFloorVertices(tunnelPolygon);
			//triangulatedPolys = triangulator.Triangulate(tunnelPolygon.ToArray());
			triangulatedPolys = triangulatedPolys.Select(t => {t=t+triangleOffset;return t;}).ToArray();
			allTriangles.AddRange(triangulatedPolys);
			
			
			
			
			//add ceiling polys
			triangleOffset = allVerticesToRender.Count;
			allVerticesToRender.AddRange(ceilingVerticesToRender);
			
			foreach(List<Vector2> adaptedPolygon in ceilingPolygons){
			
				triangulatedPolys = triangulateFloorVertices(adaptedPolygon);
				triangulatedPolys = triangulatedPolys.Select(t => {t=t+triangleOffset;return t;}).ToArray();
				allTriangles.AddRange(triangulatedPolys);
				triangleOffset+= adaptedPolygon.Count;
			}
			
			
			
			
			mesh.vertices = allVerticesToRender.ToArray();
			mesh.triangles = allTriangles.ToArray();
			
			mesh.RecalculateNormals();
        	mesh.RecalculateBounds();
			
			
			meshCollider.sharedMesh = mesh; 	
			meshFilter.mesh = mesh;

		}
		
		//Debug.Log("end start");
		
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
	Polygon CreateSymmetricalPolygonFromPath(Polygon path, float yOffset) {
		
		//Initialise the polygon with the path we have
		Polygon face = new Polygon(path);
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


	
