using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathGenerator {
	
	private  int rangeDX = 16;
    private int rangeDY = 4;
    private float minDX = 20;
    private float minDY = 20;
    private float sign = 1;
	
	public List<Vector3> GeneratePath(Vector3 firstPoint) {
		List<Vector3> points = new List<Vector3>();
		points.Add(firstPoint);
		
		for(var i=0; i< 20; i++ ){
			points.Add(GenerateNextPathPoint(points[points.Count-1],nextSign()));
		}
		
		//Next we'll move towards the surface until we reach it.
		
		while(points[points.Count-1].y<0) {
		
			points.Add(GenerateNextPathPoint(points[points.Count-1],+1));
		}
		
		return points;
	}
	
	public Vector3 GenerateNextPathPoint(Vector3 previousVector, float sign) {
		float x = previousVector.x + Random.value%rangeDX+minDX;
		float y = previousVector.y + ( Random.value%rangeDY+minDY)*sign;
		if(y>0) {
			y = 0;
		}
		return new Vector3(x,y,0);
	}
	

	
	private float nextSign(){
		int index =  (int)(Random.value * 2);
		if(index ==0) {
			return -1;
		} else {
			return 1;
		}
	}
	
}

