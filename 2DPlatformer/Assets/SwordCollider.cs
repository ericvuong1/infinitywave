using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Enemy")
        {
            Ennemy ennemy = collision.gameObject.GetComponent<Ennemy>();
            if (ennemy != null)
            {
                int force = GetComponentInParent<Sword>().knockback;
                Vector2 dir = ( -1 * ennemy.transform.position).normalized;
                ennemy.DamageEnnemy(GetComponentInParent<Sword>().damage);
                ennemy.GetComponent<Rigidbody2D>().AddForce(dir*force);
            }
        }
    }
}
