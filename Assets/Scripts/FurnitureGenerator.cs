using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FurnitureGenerator : MonoBehaviour {
	
	public Transform camera;
	public CarController car;
	public CreatureController cowPrefab; //used as a template for instantiating
	public CreatureController pigPrefab;
	public BaseController treePrefab;
	public BarnController barnPrefab;
	public HUDController hudController;
	private BarnController barn;
	public ForestController forestPrefab;
	public ForestController pineForestPrefab;
	
	private List<CreatureController> activeCows;
	private List<CreatureController> cowPool;
	
	private List<ForestController> treePool;
	private List<ForestController> activeTrees;
	
	
	private List<RockController> rockPool;
	private List<RockController> activeRocks;

	
	private Vector3 lastForestPosition = new Vector3(0,0,0);
	
	private List<Vector3> points;
	
	public MazeManager mazeManager;	
	private PathGenerator pathGenerator;
	
	private List<Bounds> cells;
	
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;
	
	public RockController rock1Prefab;
	
	
	private int numHorizontalCells = 20;
	private	int numVerticalCells = 20;
		

	// Use this for initialization
	void Start () {
		
		
		pathGenerator = new PathGenerator();
		
		activeCows = new List<CreatureController>();
		cowPool = new List<CreatureController>();
		
		treePool = new List<ForestController> ();
		activeTrees = new List<ForestController>();
		
		rockPool = new List<RockController>();
		activeRocks = new List<RockController>();
		
		for(var i=0;i<30;i++) {
			RockController rock = (RockController)Instantiate(rock1Prefab, new Vector3(-1000,0,0), Quaternion.identity);
			rockPool.Add(rock);
		}
		
		//Populate the pool of trees
		
		for(var i=0;i<10;i++) {
			ForestController forest = (ForestController)Instantiate(forestPrefab, new Vector3(-1000,0,0), Quaternion.identity);
			treePool.Add(forest);
		}
		
		
		points = pathGenerator.GeneratePath(new Vector2(0,0));
		
		//lets get the last cell
		Vector3 firstCell = points[0];
		Vector3 lastCell = points[points.Count-1];
		int numCells = (int)System.Math.Ceiling((lastCell.x - firstCell.x)/20f);
		
		
		//now
		
			
		mazeManager.CreateCells(new Vector3(0,0,0),numCells,10,20);
		
			
		List<Bounds> bounds = mazeManager.GetCellsNotIntersectedBy(points);
		
		//mazeManager.MarkCellsAsPath(bounds);
		
		mazeManager.RenderCells(bounds);
	
		
		foreach(RockController rock in rockPool){
			mazeManager.AddToCell(rock, bounds);
		}

		
		//add the start position
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
		
		foreach(ForestController t in activeTrees){
			if(IsObjectPassed(t)) {
				//is the cow is no longer on screen, remove it and place it in the cow pool.
				RemoveTreeFromScene(t);
			}
		}
		
		//if theres anything in the pool, add a new forest.
		if(treePool.Count > 0) {
			lastForestPosition = new Vector3(lastForestPosition.x + 100,0,100f + 100f * Random.value);
			CreateForest(lastForestPosition);
		}
	
	}
	
	private bool IsObjectPassed(BaseController gameObject){
		Vector3 cameraToLeft = Camera.mainCamera.ViewportToWorldPoint(new Vector3(1,1, camera.position.z));
		//Bounds bounds = gameObject.GetBounds().extents.x;
		
		return (gameObject.GetTransform().position.x + gameObject.GetBounds().size.x) < cameraToLeft.x;
	}
	
	/*
	void CreateTree(Vector3 position) {
		BaseController tree;
		if(treePool.Count>0){
			tree = treePool[0];
			activeTrees.Add(tree);
		} else {
			tree = (BaseController)Instantiate(treePrefab, position, Quaternion.identity);
			activeTrees.Add(tree);
		}	
		tree.GetTransform().position = position;
		
	}*/
	
	ForestController CreateForest(Vector3 position) {
		ForestController forest;
		forest = treePool[0];
		activeTrees.Add(forest);
		treePool.Remove(forest);
		forest.GetTransform().position = position;
		return forest;
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
		if(car.CanAddCreatureToCargo(creature)){
			car.AddCreatureToCargo(creature);
			RemoveCreatureFromScene(creature);
		}
	}
	
	public void RemoveCreatureFromScene(CreatureController creature) {
		activeCows.Remove(creature);
		cowPool.Add(creature);
		creature.GetTransform().position = new Vector3(0,0,-1000);
	}
	
	public void RemoveTreeFromScene(ForestController tree) {
		activeTrees.Remove(tree);
		treePool.Add(tree);
		tree.GetTransform().position = new Vector3(0,0,-1000);
	}
	
	void CreateBarn(Vector3 position) {
		if(barn == null) {
			barn = (BarnController)Instantiate(barnPrefab,position, Quaternion.identity);
			barn.carController = car;
			barn.BarnEnter+=new BarnEnterEventHandler(BarnEnter);
		} 
		barn.GetTransform().position = position + new Vector3(0,0,30);
	}
		
	public void BarnEnter(BarnController barn) {
		//int newlyCollected = car.GetCargo().Count;
		
		//update the score...
		//scoreController.addCollected
		foreach(CreatureController c in car.GetCargo()){
			hudController.CreatureReturnedToBarn(c);
		}
		car.ClearCargo();
	}
}
