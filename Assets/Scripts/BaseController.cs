using UnityEngine;
using System.Collections;

public class BaseController : MonoBehaviour {
	
	
	public Transform GetTransform() {
		return this.transform;
	}
	
	public virtual Bounds GetBounds() {
		return this.collider.bounds;
	}
}


