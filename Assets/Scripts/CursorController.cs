using UnityEngine;
using System.Collections;

public class CursorController : BaseController {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetMouseButton(0)==true) {
		     Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,System.Math.Abs(Camera.mainCamera.transform.position.z)));
			this.transform.position = mousePosition;
		}
	
	}
	
	
}
