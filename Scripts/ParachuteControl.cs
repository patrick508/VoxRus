using UnityEngine;
using System.Collections;

public class ParachuteControl : MonoBehaviour {
    public float distanceToSee;
    RaycastHit hit;
    public GameObject backpack;
    CharacterController controller;
    void Awake() {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
   void Update() {
        //Fals down from the sky if character controller is not enabled
        if (controller.enabled == false) {
                this.transform.position -= transform.up * Time.deltaTime * 5;
        }
        //Draws a ray to check if close to the ground & If close turn backpack & parachute of & turn on EnemyAI & RaycastJump
       Debug.DrawRay(this.transform.position, this.transform.up * -1f * distanceToSee, Color.red);
       Physics.Raycast(this.transform.position, this.transform.up*-1f, out hit, distanceToSee);
        if (Vector3.Distance(this.transform.position, hit.point) <= 10f) {
            backpack.active = false;
            controller.enabled = true;
            this.GetComponent<enemyAIScript01>().enabled = true;

        }
    }
}
