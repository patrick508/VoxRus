using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Shooting : MonoBehaviour
{
    public delegate void hitDel(Collision _c);
    public float waitTime = 0.5F;
    private float nextFire = 0.0F;
    public int Damage = 20;
    public int fSpeed = 50;
    Vector3 impact = Vector3.zero;
    float mass = 3.0f;

    public int AmmoCount;
    public Text AmmoText;

    GameObject prefab_bullet;
    GameObject prefab_bullet_2;
    private CharacterController character;
    // Use this for initialization
    void Start()
    {
        prefab_bullet = Resources.Load("bullet") as GameObject;
        prefab_bullet_2 = Resources.Load("bullet_2") as GameObject;
        character = GetComponent<CharacterController>();
        AmmoCount = 40;
        SetAmmoText();
    }
    // Update is called once per frame
    void Update(){
    }
   public void BulletShooting() {
        //If mouse is clicked fires a prefab(bullet) in a line from the player and calls fucntions apply damage and destroy (defined Baseplayer and BaseEnemy)
        //Bullet does damage equel to the amound set above, also makes a timer run down wich prevents the player from spamming bullets
        if (Input.GetMouseButton(0) && Time.time > nextFire && AmmoCount > 0) {
            nextFire = Time.time + waitTime;
            GameObject bullet = Instantiate(prefab_bullet) as GameObject;
            bullet.transform.position = transform.position + Camera.main.transform.forward * 2;
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            AmmoCount = AmmoCount -1;
            SetAmmoText();
            BulletScript bs = bullet.GetComponent<BulletScript>();
            hitDel functieVar = new hitDel((Collision _c) => {
                BaseEnemy be = _c.gameObject.GetComponent<BaseEnemy>();
                if (be != null) {
                    be.ApplyDamage(Damage);
                    Destroy(bs.gameObject);
                }
            });
            bs.setOnHit(functieVar);


            rb.velocity = Camera.main.transform.forward * 40;

            Destroy(bullet.gameObject, 3);

        }

        //If other mouse is clicked fires a prefab(bullet) in a line from the player(Dealing 5 damage)
        //Bullet fires the enemy back (pushes enemy away), also makes a timer run down wich prevents the player from spamming bullets
        else if (Input.GetMouseButton(1) && Time.time > nextFire) {
            nextFire = Time.time + waitTime;
            GameObject bullet_2 = Instantiate(prefab_bullet_2) as GameObject;
            bullet_2.transform.position = transform.position + Camera.main.transform.forward * 2;
            Rigidbody rb = bullet_2.GetComponent<Rigidbody>();
            BulletScript bs = bullet_2.GetComponent<BulletScript>();
            hitDel functieVar = new hitDel((Collision _c) => {
                BaseEnemy be = _c.gameObject.GetComponent<BaseEnemy>();
                if (be != null) {
                    //Uses ImpactReceiver script and pushes the enemy away from the side the ball is hitting it from
                    ImpactReceiver script = _c.gameObject.GetComponent<ImpactReceiver>();
                    if (script) script.AddImpact(-_c.relativeVelocity.normalized, 50f);
                    be.ApplyDamage(5);
                    Destroy(bs.gameObject);
                }
            });
            bs.setOnHit(functieVar);


            rb.velocity = Camera.main.transform.forward * 40;

            Destroy(bullet_2.gameObject, 3);

        }
    }
    //Update Ammo text
    public void SetAmmoText() {
        AmmoText.text = "" + AmmoCount;
    }
}