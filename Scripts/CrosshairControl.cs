using UnityEngine;
using System.Collections;

public class CrosshairControl : MonoBehaviour {
    Animator anim;

    // Use this for initialization
    void Start () {

        anim = GetComponent<Animator>();

	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.LeftShift)){
            // Plays the open animation
            anim.Play("Cross_open");
        }

        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            // Plays the close animation
            anim.Play("Cross_closer");
        }
    }
    }
