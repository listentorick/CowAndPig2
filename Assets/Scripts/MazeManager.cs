using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;
	
	private List<Bounds> cells = new List<Bounds>();
	
	public void CreateCells(Vector3 offset, int numHorizontalCells, int numVerticalCells, int cellSize) {
		
		
		Mesh mesh = new Mesh();
		List<Vector3> polygons = new List<Vector3> ();
		List<int> triangles = new List<int>();
		int triangleOffset = 0;
		
		float cellX = 0f;
		float cellY = 0f;
		
		for(var r=0; r<numHorizontalCells;r++){
			cellX = offset.x + (r*cellSize) - (cellSize/2);
			for(var c=0; c<numVerticalCells;c++){
				cellY = offset.y -((c*cellSize) - (cellSize/2));
				Bounds cell = new Bounds(new Vector3(cellX,cellY,0),new Vector3(cellSize,cellSize,cellSize));
				cells.Add(cell);
				
				triangleOffset = polygons.Count;
			
				triangles.Add(triangleOffset);
				triangles.Add(triangleOffset+1);
				triangles.Add(triangleOffset+2);
				triangles.Add(triangleOffset+2);
				triangles.Add(triangleOffset+3);
				triangles.Add(triangleOffset);
								
				polygons.AddRange(CreateCellPolygon(cell));

			}
		}
		
	//	mesh.vertices = polygons.ToArray();
	//	mesh.triangles = triangles.ToArray();
	//	meshFilter.mesh = mesh;
		
	}

	public void AddToCell(BaseController controller) {
		
		Bounds cell = GetRandomCell();
		controller.transform.position = new Vector3(cell.center.x,cell.center.y,cell.center.z);
	}
	
	public Bounds GetRandomCell(){
		int cellIndex = Random.Range(0,cells.Count-1);
		return cells[cellIndex];
	}
	
	List<Vector3> CreateCellPolygon(Bounds cell) {
		
		float minX = cell.center.x - cell.extents.x;
		float maxX =  cell.center.x + cell.extents.x;
		float minY =  cell.center.y - cell.extents.y;
		float maxY =  cell.center.y + cell.extents.y;
		
		Vector3 topLeft = new Vector3(minX,minY,-10);
		Vector3 topRight = new Vector3(maxX,minY,-10);
		Vector3 bottomRight = new Vector3(maxX,maxY,-10);
		Vector3 bottomLeft = new Vector3(minX,maxY,-10);
		
		List<Vector3> polygon = new List<Vector3>();
		polygon.Add(topLeft);
		polygon.Add(topRight);
		polygon.Add(bottomRight);
		polygon.Add(bottomLeft);
	
		return polygon;
	}
}

