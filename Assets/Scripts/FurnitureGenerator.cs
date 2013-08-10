using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FurnitureGenerator : MonoBehaviour {
	
	public Transform camera;
	public CarController car;
	public CreatureController creature;
	public BarnController barnPrefab;
	private BarnController barn;
	
	private List<Vector2> points;

	// Use this for initialization
	void Start () {
		
		points = new List<Vector2>();
		
		//add the start position
		points.Add(new Vector2(0,0));
		CreateBarn(new Vector3(0,0,0));
	}
	
	// Update is called once per frame
	void Update () {
		
		if(barn!=null) {
		
			if(IsObjectPassed(barn.GetTransform())) {
				Vector3 cameraToRight = Camera.mainCamera.ViewportToWorldPoint(new Vector3(1,1, camera.position.x));
				CreateBarn(new Vector3(0,0,cameraToRight.z));
			}
		}
	
	}
	

	private bool IsObjectPassed(Transform gameObject){
		Vector3 cameraToLeft = Camera.mainCamera.ViewportToWorldPoint(new Vector3(0,1, camera.position.x));
		return gameObject.position.z < cameraToLeft.z;
	}
	
	void CreateBarn(Vector3 minimumPosition) {
		
		if(barn == null) {
			barn = (BarnController)Instantiate(barnPrefab, minimumPosition + new Vector3(0,0,100), Quaternion.identity);
			barn.carController = car;
		} else {
			//just reuse our current instance and move it yonder
			barn.GetTransform().position = minimumPosition + new Vector3(0,0,100);
		}
	}
}
