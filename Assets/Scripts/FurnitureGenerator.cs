using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FurnitureGenerator : MonoBehaviour {
	
	public Transform camera;
	public CarController car;
	public CreatureController cowPrefab;
	public CreatureController pigPrefab;
	public BarnController barnPrefab;
	private BarnController barn;
	
	private List<CreatureController> activeCows;
	private List<CreatureController> cowPool;
	
	private List<Vector2> points;

	// Use this for initialization
	void Start () {
		
		activeCows = new List<CreatureController>();
		cowPool = new List<CreatureController>();
		
		points = new List<Vector2>();
		
		//add the start position
		points.Add(new Vector2(0,0));
		CreateBarn(new Vector3(100,0,0));
	}
	
	// Update is called once per frame
	void Update () {
		
		Vector3 cameraToRight = Camera.mainCamera.ViewportToWorldPoint(new Vector3(0,1, camera.position.z));
		
		if(barn!=null) {
			if(IsObjectPassed(barn)) {
				
				
				CreateBarn(new Vector3(cameraToRight.x,0,0) + new Vector3(100,0,0));
			}
		}
		
		foreach(CreatureController c in activeCows){
			if(IsObjectPassed(c)) {
				//is the cow is no longer on screen, remove it and place it in the cow pool.
				RemoveCreatureFromScene(c);
			}
		}
		
		if(activeCows.Count == 0) {
			CreateCow(new Vector3(cameraToRight.x,0,0) + new Vector3(100,0,5));
		}
	
	}
	
	private bool IsObjectPassed(BaseController gameObject){
		Vector3 cameraToLeft = Camera.mainCamera.ViewportToWorldPoint(new Vector3(1,1, camera.position.z));
		//Bounds bounds = gameObject.GetBounds().extents.x;
		
		return (gameObject.GetTransform().position.x + gameObject.GetBounds().size.x) < cameraToLeft.x;
	}
	
	void CreateCow(Vector3 position){
		
		CreatureController cow;
		if(cowPool.Count>0){
			cow = cowPool[0];
			activeCows.Add(cow);
		} else {
			cow = (CreatureController)Instantiate(cowPrefab, position, Quaternion.identity);
			cow.Caught+=new CaughtEventHandler(CreatureCaught);
			activeCows.Add(cow);
		}	
		
		position = position + new Vector3(0,(cow.GetBounds().size.y/2),0);
		cow.GetTransform().position = position;
		
		cow.carController = car;
	}
	
	public void CreatureCaught(CreatureController creature){ 
		RemoveCreatureFromScene(creature);
	}
	
	public void RemoveCreatureFromScene(CreatureController creature) {
		activeCows.Remove(creature);
		cowPool.Add(creature);
		creature.GetTransform().position = new Vector3(0,0,-1000);
	}
	
	void CreateBarn(Vector3 position) {
		
		if(barn == null) {
			barn = (BarnController)Instantiate(barnPrefab,position, Quaternion.identity);
			barn.carController = car;
		} 
		
		barn.GetTransform().position = position + new Vector3(0,0,30);
	}
}
