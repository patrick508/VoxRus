using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthDropBehaviour : MonoBehaviour {
    // Use this for initialization
    void Start() {
        //Hover effect(Rotate part)
        Hashtable args = new Hashtable();
        args.Add("y",180f);
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
    }

    // Update is called once per frame
    void Update() {
    }
}
