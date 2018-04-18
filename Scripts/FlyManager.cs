using UnityEngine;
using System.Collections;

public class FlyManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartRound();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //Makes the camera follow the path with these settings
    void StartRound() {
        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("MainMenuFly"), "time", 600, "easetype", iTween.EaseType.linear, "orienttopath", true, "looktime", 1, "oncomplete", "StartRound"));
    }
}
