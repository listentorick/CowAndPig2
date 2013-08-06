#pragma strict

	var terrainMaterial : Material;
    var landscapeDepth: int = 100;
    var windowSize: int = 2;
    var mesh : Mesh;
	var vertexWindow: Vector2[];
	
	var rangeDX : float = 100.0f;//8;
    var rangeDY : float = 20f;
    var minDX :float = 50f;
    var minDY : float = 5f;//10;
    var sign: float = 1f;
    
    var car: Transform;
    
   
    
 	function Start () {
 	
    	windowSize = 20;
    	rangeDX  = 100.0f;//8;
        rangeDY = 10f;
     minDX = 50f;
     minDY  = 5f;//10;
     sign  = 1f;

		vertexWindow = new Vector2[2];
		lastIndex = 0;
		vertexWindow[0] = new Vector2(-100,0); //the start
		lastIndex+=1;
		vertexWindow[lastIndex] = new Vector2(40,0); //the start
		
		
		//vertexWindow = createNPoints(vertexWindow, 1, 2);
    	 
    	//mesh = new Mesh();

    	makeLandscape(vertexWindow);
    	
    }
    
    var lastIndex :int = 0;

    function Update () {
    
		//if(Input.GetMouseButtonDown(0)) {
    	
    		//var clickedPosition : Vector3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    	
    		
    	var screenClickPosition : Vector3 = new Vector3(0,Input.mousePosition.y, Input.mousePosition.x);
    	
    	var worldClickPosition : Vector3 = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,Camera.mainCamera.transform.position.x));
    		
    			
    		
    		
    		var needsRender : boolean = false;
    			
    		/*
			var topLeftViewPort : Vector3 = Camera.mainCamera.ViewportToWorldPoint (Vector3 (0,1, Camera.mainCamera.transform.position.x));
				
			for(var i=vertexWindow.Length-1; i>=0;i--) {
    			if(vertexWindow[i].x<topLeftViewPort.z){
					//
					var lastIndex : int = vertexWindow.Length -1 - i;
					var newWindow : Vector2[] = new Vector2[lastIndex];
					Debug.Log(lastIndex + " " + i);
					
					System.Array.Copy(vertexWindow, i, newWindow, 0, lastIndex);
					
					needsRender = true;
					break;
				}
			}*/
    		
    		//if the gap between the car and the last vertex is then...
    		var bufferZ: float = car.position.z + 20;
    		var nextZ : float = bufferZ + 1;
    		var nextY: float = worldClickPosition.y;
    		var lastY: float = vertexWindow[vertexWindow.length-1].y;
    		var dY: float = nextY - lastY;
    	
    		
    		if(vertexWindow[vertexWindow.length-1].x < bufferZ){
    		
    			//Debug.Log(worldClickPosition.x + " " + worldClickPosition.y + " " + worldClickPosition.z);
    			
    			if(dY > 0) {;
    				nextY = lastY + 0.3f;	
    			}
    			
    			if(dY < 0) {;
    				nextY = lastY - 0.3f;	
    			}
    		
    			lastIndex += 1;
    			System.Array.Resize.<Vector2>(vertexWindow, lastIndex+1);
    			vertexWindow[lastIndex] = new Vector2(nextZ,nextY);
    		
    			needsRender = true;
    			
    		}
    		
    		if(needsRender) {
    			makeLandscape(vertexWindow);
    		}
    	
    	//}
    	
    	//grab the last point 
    	
    	
    	/*
    	
		var cameraMaxX : float = Camera.mainCamera.transform.position.z - Camera.mainCamera.orthographicSize;
		
		
		//Gets the position of the top right
		var topRightViewPort : Vector3 = Camera.mainCamera.ViewportToWorldPoint (Vector3 (1,1, Camera.mainCamera.transform.position.x));
		var topLeftViewPort : Vector3 = Camera.mainCamera.ViewportToWorldPoint (Vector3 (0,1, Camera.mainCamera.transform.position.x));
		
		var max = topRightViewPort.z + 10;
		var min = topLeftViewPort.z - 10;
		
		if(vertexWindow[windowSize-1].x < max) {
			Debug.Log("need more");
			for(var i=windowSize-1; i>=0;i--) {
				
				if(vertexWindow[i].x<min){
					Debug.Log("remove " + i);
					var newWindow : Vector2[] = new Vector2[windowSize];
					var lastIndex : int = windowSize -1 - i;
					System.Array.Copy(vertexWindow, i, newWindow, 0, lastIndex);
					vertexWindow = createNPoints(newWindow, lastIndex, i);
					makeLandscape(vertexWindow);
					
					break;
				}
			}
			
			
		
		}
		return;
		*/
	}
    
    /*
    var hillSegmentWidth : int = 10;
    var maxPoints = 100;
    
    //These are the major points....
    var pointWindow : Vector2 = new Vector2[maxPoints];
    
    function fillWindow(intialPoint: Vector2) {
    
    
    }
    
    public void populateWithNPoints(Vector2 lastPoint, int numberOfPoints) {

		var nextPoint : Vector2;
		while(borderPoints.size()<numberOfPoints) {
			nextPoint = generateNextTerrainPoint(lastPoint,sign);
			createTriangulatedSegment(lastPoint,nextPoint,borderPoints, hillVertices, hillTextCoords);
			sign = nextSign();
			lastPoint = nextPoint;
		}
	}*/
	
	function createNPoints(window: Vector2[], index : int, numPoints: int) : Vector2[]{ 
		
		var lastPoint: Vector2 = window[index-1];
		for(var i=index; i<window.Length;i++){
			lastPoint = generateNextTerrainPoint(lastPoint, nextSign());
			window [i] = lastPoint;
		}
		
		return window;
	}
    

 	//var r: System.Random = new System.Random();
    
    function nextSign() : float {
    	return Random.value <0.5? 1: -1;
    }
    
    function generateNextTerrainPoint(previousVector :Vector2,  sign: float) : Vector2{
    	var dx : float = Random.Range(0,rangeDX) +  minDX;
    	var dy : float = Random.Range(0,rangeDY) + minDY;
    	//Debug.Log(rangeDX + " " + dx + " " + dy);
		var x : float = previousVector.x + dx;
		var y : float = previousVector.y + (dy * sign);
		return new Vector2(x,y);
	}
    
    /*
    function createCurvedLandscape(p0 :Vector2, p1 : Vector2, ArrayList<Vector2> borderVertices,  ArrayList<Vector2> hillVertices,  ArrayList<Vector2> hillTexCoords){
		 
		//float minY=engine.getCamera().getYMax() + 1000;
	  // float minY =100000;
	  
	  	var points : Vector2
		var pt0 : Vector2;
		var pt1: Vector2;
		var hSegments: int;
		var dx: float;
		var dy: float;
		var da: float;
		var ymid: float;
		var ampl: float;
		
       // triangle strip between p0 and p1
       hSegments = Math.Floor(((p1.x-p0.x)/hillSegmentWidth));
       dx = (p1.x - p0.x) / hSegments;
       da = (float) (Math.PI / hSegments);
       ymid = (p0.y + p1.y) / 2;
       ampl = (p0.y - p1.y) / 2;
       pt0 = p0;
    
       for (int j=1; j<hSegments+1; j++) {
       	pt1 = new Vector2();
           pt1.x = p0.x + j*dx;
           pt1.y = (float) (ymid + ampl * Math.cos(da*j));
           borderVertices.add(pt1);   
           pt0 = pt1;
       }
   
	}*/

	

 	function copyVector2ArrayInto(source : Vector2[], target : Vector2[]) : Vector2[]{
    	var currentPoint : Vector2;
     	for(var i=0; i<source.Length; i++){
    	 	currentPoint = source[i];
    	 	target[i] = new Vector2(currentPoint.x, currentPoint.y);
    	}
    	return target;
    }
    
    function copyVector3ArrayInto(source : Vector2[], target : Vector2[]) : Vector2[]{
    	var currentPoint : Vector2;
     	for(var i=0; i<source.Length; i++){
    	 	currentPoint = source[i];
    	 	target[i] = new Vector2(currentPoint.x, currentPoint.y);
    	}
    	return target;
    }
    
    function createVectorsForTopPlaneFromLine(source : Vector2[]) : Vector2[]{
    	var currentPoint : Vector2;
    	var destination : Vector2[] = new Vector2[source.Length];
    	for(var i=0; i<source.Length; i++){
    	 	currentPoint = source[i];
    	 	destination[i] = new Vector2(currentPoint.x, landscapeDepth);
    	}
    	return createVectorsForPlaneFromLine(destination,false, 0);
    }
    
    function createVectorsForPlaneFromLine(lineVertexList : Vector2[], mirror: boolean, baseLine :int) : Vector2[]{
    	//we need twice as many Vectors
    	var planeVertexList = new Vector2[lineVertexList.Length*2]; 
    	
    	//add the line to the our planeVertexList
    	planeVertexList = copyVector2ArrayInto(lineVertexList, planeVertexList);
    		
    	//now create the remaining points
    	var currentPoint : Vector2;
    	 
    	//last point to first in the line....
    	var index : int = lineVertexList.Length-1;
		for(var i=lineVertexList.Length-1;  i>=0; i--){
    	 	currentPoint = lineVertexList[i];
    	 	index++;
    	 	if(mirror) {
    	 		planeVertexList[index] = new Vector2(currentPoint.x, currentPoint.y + baseLine);
    		} else {
    			planeVertexList[index] = new Vector2(currentPoint.x, baseLine);
    		}
    	}
    	    	
    	return planeVertexList;
    }
    
    function logVector2Array(vertices : Vector2[]) {
    	for (var j=0; j<vertices.Length; j++) {
     		Debug.Log(vertices[j].x + " " + vertices[j].y);
     	}
    }
    
     function logVector3Array(vertices : Vector3[]) {
    	for (var j=0; j<vertices.Length; j++) {
    		if(vertices[j]!=null){
     			Debug.Log(vertices[j].x + " " + vertices[j].y + " " +  vertices[j].z);
     		}
     	}
    }
    

        
    function makeLandscape (lineVertexList : Vector2[]) {
    	 
		var planeVertexList: Vector2[] = createVectorsForPlaneFromLine(lineVertexList,true, 20);
    	 
    	//Create the front
        var tr : Triangulator = new Triangulator();
        tr.initTriangulator(planeVertexList);
       	var frontTriangles:int[] = tr.Triangulate(); 
 
        // Create the front vertices
		var frontVertices : Vector3[] = new Vector3[planeVertexList.Length];
		for (var i=0; i<planeVertexList.Length; i++) {
		    frontVertices[i] = new Vector3(planeVertexList[i].x, planeVertexList[i].y, 50);    
		}
		
		//create the top
		var topPlaneVertexList : Vector2 [] = createVectorsForTopPlaneFromLine(lineVertexList);

       	var topTriangles = triangulateTopVertices(topPlaneVertexList);
       	
       	//var topVertices = makeRoofFromVertices(topPlaneVertexList, planeVertexList);
       	
       	
       	// Create the top vertices
		var topVertices : Vector3[] = new Vector3[topPlaneVertexList.Length];
		var lineVertexListIndex : int = 0;
		for (var j=0; j<topPlaneVertexList.Length; j++) {
			
		   
			topVertices[j] = new Vector3(topPlaneVertexList[j].x, planeVertexList[lineVertexListIndex].y, topPlaneVertexList[j].y);    

		    if(j < lineVertexList.Length -1) {
		    
		    	lineVertexListIndex++;
		    } else if(j>=lineVertexList.Length){ 
		  
		    	lineVertexListIndex--;
		    }
		    
		   
		}
		
        //combined vertices
        var combinedVertices : Vector3[] = new Vector3[frontVertices.length + topVertices.length];
        System.Array.Copy(frontVertices, combinedVertices, frontVertices.Length);
 		
 		System.Array.Copy(topVertices, 0, combinedVertices, frontVertices.Length, topVertices.Length);
 
    	var combinedTriangles = new int[frontTriangles.Length + topTriangles.Length]; 
    	System.Array.Copy(frontTriangles, combinedTriangles, frontTriangles.Length);
 		
 		//we need to increment the indexs for the top...
 		for(var k=0; k<topTriangles.Length;k++) {
 			topTriangles[k] = topTriangles[k] + topVertices.Length; 
 		}
 		
 		System.Array.Copy(topTriangles, 0, combinedTriangles, frontTriangles.Length, topTriangles.length);

    	var mesh : Mesh = new Mesh();
        mesh.vertices = combinedVertices;
        mesh.triangles = combinedTriangles;
        
        var texture = terrainMaterial.mainTexture;
        var textureWidth : int = texture.width;
        var textureHeight : int = texture.height;
        
        var uvX;
        var uvY;
        var uv : Vector2[]  = new Vector2[combinedVertices.Length];
      	for(var l =0 ;l< combinedVertices.length;l++) {
      		if(l<frontVertices.Length) {
      			uvX = frontVertices[l].x / 10;
      			uvY = frontVertices[l].y / 10;
      		} else {
      			uvX = 0;
      			uvY = 0;
      		}
      		
      		uv[l] = new Vector2(uvX ,uvY );
      	
      	}
      	
        mesh.uv = uv;
        mesh.RecalculateNormals();
        
        mesh.RecalculateBounds();


        (GetComponent(MeshFilter) as MeshFilter).sharedMesh = mesh;   
        (GetComponent(MeshCollider) as MeshCollider).sharedMesh = mesh; 	
        (GetComponent(MeshRenderer) as MeshRenderer).material = terrainMaterial;

    } 
    
    function triangulateTopVertices(topVertices : Vector2[]) : int[]{
    
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
    	    	
    	var lastPointToTriangulate : int = (topVertices.Length/2);
    	var lastPointIndex : int = topVertices.Length - 1;
    	var numTriangles : int = lastPointToTriangulate * 6;

    	var triangles: int[] = new int[numTriangles];
    	var currentIndex : int = 0;
		var isEven : boolean;
    	for(var index = 0; index < lastPointToTriangulate - 1; index ++ ){
    		
    		
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
       
    @script RequireComponent(MeshFilter)
    @script RequireComponent(MeshRenderer)
    @script RequireComponent(MeshCollider)