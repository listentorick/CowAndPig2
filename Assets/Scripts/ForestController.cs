using UnityEngine;
using System.Collections;

public class ForestController : BaseController {
	
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public override Bounds GetBounds() {
		return this.collider.bounds;
		//return new Bounds(new Vector3(0,0,0),new Vector3(0,0,0));
	}
}
