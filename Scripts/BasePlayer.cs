using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BasePlayer : MonoBehaviour {
    GameObject prefab_enemy;
    GameObject prefab_ammo;
    public int counter = 0;
    public float health = 100;
    CharacterController controller;
    public Slider mySlider;

    private bool Invoke_Ammo = false;
    // Use this for initialization
    void Start () {
        prefab_enemy = Resources.Load("Enemy") as GameObject;
        prefab_ammo = Resources.Load("ammo") as GameObject;
    }
	
	// Update is called once per frame
	void Update () {
        Screen.lockCursor = true;
        Cursor.visible = false;
        EnemySpawn();
        AmmoSpawnManager();
        Die();
    }
    void Awake() {
        controller = GetComponent<CharacterController>();
    }
    //Instantiates x +1 enemy's at given location(GetSpawnLocation). Adds 1 to the counter for every instantiated object(enemy)
    void EnemySpawn()
    {
        if (counter <= 7) {
            Instantiate(prefab_enemy, GetSpawnLocation(), Quaternion.identity);
            counter++;
        }
    }
    //Calls function AmmoSpawn every x seconds.
    void AmmoSpawnManager() {
        if (Invoke_Ammo == false) {
            InvokeRepeating("AmmoSpawn", 5f, 5f);
            Invoke_Ammo = true;
        }
    }
    //Only loops trough this function if the random range number is lower than the given number.
    void AmmoSpawn() {
        if (Random.Range(0f, 1f) < .3f) {
            Instantiate(prefab_ammo, GetAmmoSpawnLocation(), Quaternion.identity);
            print("Ik spawn nu ammo");
        }
        CancelInvoke("AmmoSpawn");
        Invoke_Ammo = false;
    }

    //Picks a random location at a x(random range) amount away from the player & checks if they're not to close to the set amount
    Vector3 GetSpawnLocation() {
        Vector3 spawn_enemy = transform.position;
        float x = (Random.Range(-40f, 40f));
        float z = (Random.Range(-40f, 40f));
        float y = (20f);
        spawn_enemy.x += x;
        spawn_enemy.z += z;
        spawn_enemy.y += y;
        //If the enemy is spawned in a range of x amount or smaller to the player respawn it.
        if (Vector3.Distance(this.transform.position, spawn_enemy) <= 10) {
            return GetSpawnLocation();
        }
        else return spawn_enemy;
        
    }
    //Picks a random location at a x(random range) amount away from the player & checks if it's not to close to the set amount. Than changes the y location
    //To the calculated y location from World script, and uses this is y position
    Vector3 GetAmmoSpawnLocation() {
        Vector3 spawn_ammo = transform.position;
        float x = (Random.Range(-40f, 40f));
        float z = (Random.Range(-40f, 40f));
        float y = 20f;
        spawn_ammo.x += x;
        spawn_ammo.z += z;
        spawn_ammo.y = World.Instance.GetHeight(new Vector3(spawn_ammo.x, y, spawn_ammo.z)) + 1;
            //If the ammo is spawned in a range of x amount or smaller to the player respawn it.
            if (Vector3.Distance(this.transform.position, spawn_ammo) <= 10) {
                return GetAmmoSpawnLocation();
            }
        else return spawn_ammo;

    }

    //If player health = 0, load game over scene
    void Die(){
        if (health == 0){
            Application.LoadLevel("Game_Over");
        }
    }
    //Adds x amount of health to healthbar once walked over healthdrop & destroys after. Also Adds x amount of ammo and than destroys aswell.
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Health") {
            mySlider.value = mySlider.value +0.2f;
            health = health + 20f;
            Destroy(other.gameObject);
        } else if(other.gameObject.tag == "ammo") {
            Shooting sh = this.GetComponent<Shooting>();
            sh.AmmoCount = sh.AmmoCount + 20;
            sh.SetAmmoText();
            Destroy(other.gameObject);
        }
    }
}
