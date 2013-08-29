using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathGenerator {
	
	private  int rangeDX = 16;
    private int rangeDY = 4;
    private float minDX = 8;
    private float minDY = 4;
    private float sign = 1;
	
	public List<Vector2> GeneratePath(Vector2 firstPoint) {
		List<Vector2> points = new List<Vector2>();
		points.Add(firstPoint);
		
		for(var i=0; i< 10; i++ ){
			points.Add(GenerateNextPathPoint(points[points.Count-1],nextSign()));
		}
		
		//Next we'll move towards the surface until we reach it.
		
		while(points[points.Count-1].y<0) {
		
			points.Add(GenerateNextPathPoint(points[points.Count-1],+1));
		}
		
		return points;
	}
	
	public Vector2 GenerateNextPathPoint(Vector2 previousVector, float sign) {
		float x = previousVector.x + Random.value%rangeDX+minDX;
		float y = previousVector.y + ( Random.value%rangeDY+minDY)*sign;
		if(y>0) {
			y = 0;
		}
		return new Vector2(x,y);
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

