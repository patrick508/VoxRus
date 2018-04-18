using UnityEngine;
using System.Collections;

public class BaseEnemy : MonoBehaviour {
    public int EnemyHealth = 100;
    bool IsDead = false;
    public Transform player;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player").transform;
    }
	
	// Update is called once per frame
	void Update () {
        Despawning();
	}
    //Calls the function EnemyDestroy(from World script) when health is 0.
    public virtual void ApplyDamage(int _damage)
    {
        EnemyHealth -= _damage;
        if (EnemyHealth <= 0 && !IsDead)
        {
            IsDead = true;
            World.Instance.EnemyDestroy(this);
            BasePlayer bp = player.GetComponent<BasePlayer>();
            bp.counter--;

        }
    }
    //Despawns enemy if enemy is further away the given x amount
    void Despawning() {
        if(Vector3.Distance(this.transform.position, player.transform.position) > 55f){
            BasePlayer bp = player.GetComponent<BasePlayer>();
            bp.counter--;
            Destroy(this.gameObject);
        }
    }
    }
