using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour
{
    //public GameObject Enemy;
    Shooting.hitDel onHit;
    //Make a delegate
    public void setOnHit(Shooting.hitDel _h)
    {
        onHit = _h;
    }
    //Is being called once the bullet hits the instantiated enemy
    void OnCollisionEnter(Collision col)
    {
        onHit(col);
    }
}
