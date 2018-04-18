using UnityEngine;
using System.Collections;

public class RaycastExample : MonoBehaviour {
    public GameObject terrain;
    private PolygonGenerator tScript;
    public GameObject target;
    private LayerMask layerMask = (1 << 0);
	// Use this for initialization
	void Start () {
        tScript = terrain.GetComponent("PolygonGenerator") as PolygonGenerator;
        
	}
	
	// Update is called once per frame
	void Update () {
        tScript.update = true;
        RaycastHit hit;

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (Physics.Raycast(transform.position, (target.transform.position - transform.position).normalized, out hit, distance, layerMask)) {
            //Shows the line in the scene view
            Debug.DrawLine(transform.position, hit.point, Color.red);
            //Checks the position of the collision and create a new Vector2 there. 
            //than we add we hadd half of the reverse wich takes us half a unit into the block, so we can read out the position of the block.
            Vector2 point = new Vector2(hit.point.x, hit.point.y);
            point += (new Vector2(hit.normal.x, hit.normal.y)) * -0.5f;

            tScript.blocks[Mathf.RoundToInt(point.x - .5f), Mathf.RoundToInt(point.y + .5f)] = 0;

        } else{
            Debug.DrawLine(transform.position, target.transform.position, Color.blue);
        }
	}
}