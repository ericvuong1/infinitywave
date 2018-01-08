using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    public float fireRate = 0;
    public float SlashCoolDown = 1.0f;
    public float SlashDuration = 0.2f;

    public int damage = 30;
    public int knockback = 1;
    private float slashTimeNextCast = 0.0f;
    private float slashTimeDuration = 0.0f;

    public GameObject sword;

    public float rotationOffSet = 0;
    public bool facingRight;
    public string slash = "Fire1";


    public void Awake()
    {
        sword.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        // difference of player and mouse position
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.Normalize(); //normalize magnitude (to 1)

        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffSet);
        Slash();

    }
    void Slash()
    {
        if (Input.GetButtonDown(slash) && Time.time > slashTimeNextCast)
        {
            slashTimeNextCast = Time.time + SlashCoolDown;
            slashTimeDuration = Time.time + SlashDuration;
        }
        if (Input.GetButtonUp(slash) && Time.time < slashTimeDuration)
        {
            slashTimeDuration = 0.0f;
        }
         sword.SetActive((Input.GetButton(slash) && Time.time < slashTimeNextCast) && Time.time < slashTimeDuration);
   }
}
