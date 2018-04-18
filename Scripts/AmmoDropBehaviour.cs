using UnityEngine;
using System.Collections;

public class AmmoDropBehaviour : MonoBehaviour {

    public Transform player;

    // Use this for initialization
    void Start () {
        //Hover effect(Rotate part)
        Hashtable args = new Hashtable();
        args.Add("y", 180f);
        args.Add("looptype", "loop");
        args.Add("easetype", "linear");
        args.Add("time", 5);
        iTween.RotateAdd(this.gameObject, args);

        //Hover effect(up & down part)
        Hashtable args2 = new Hashtable();
        args2.Add("y", .2f);
        args2.Add("looptype", "pingPong");
        args2.Add("easetype", "easeInOutSine");
        args2.Add("time", 2);
        iTween.MoveBy(this.gameObject, args2);
        player = GameObject.Find("Player").transform;
    }
	
	// Update is called once per frame
	void Update () {
        Despawning();
    }

    //Despawn ammo when player is further away than given x amount
    void Despawning() {
        if (Vector3.Distance(this.transform.position, player.transform.position) > 55f) {
            Destroy(this.gameObject);
        }
    }
}
