using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets._2D;
using UnityEngine;

public class ArmRotation : MonoBehaviour {

    public float rotationOffset = 0;
    public bool facingRight;
    public PlatformerCharacter2D playerScript;

	void Update () {
        Vector3 newPos = transform.localPosition;
        if (playerScript == null)
        {
            Debug.Log("What? no player Script?");
        }
        else
        {
            facingRight = playerScript.isFacingRight();
        }
        if (facingRight)
        {
            newPos.x = 0.23f;
            transform.localPosition = newPos;
        }
        else
        {
            newPos.x = -0.23f;
            transform.localPosition = newPos;
        }
        // difference of player and mouse position
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; 
        difference.Normalize(); //normalize magnitude (to 1)

        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f,0f, rotZ + rotationOffset);

	}
    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x = scale.x * -1;
        transform.localScale = scale;
    }
}
