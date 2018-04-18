using UnityEngine;
using System.Collections;

public class MinimapBehaviour : MonoBehaviour {

    public Transform Target;
	// Use this for initialization
	void Start () {
	
	}
	
	// Is called after all the other update functions
	void LateUpdate () {
        this.transform.position = new Vector3(Target.transform.position.x, this.transform.position.y, Target.transform.position.z);
	}
}
