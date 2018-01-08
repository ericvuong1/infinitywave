using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spells : MonoBehaviour {

    public Transform ShieldParticlePrefab;
    public string shield;
    public GameObject ShieldPrefab;

    public float ShieldCooldown = 5.0f;
    public float ShieldDuration = 2.0f;
    private float shieldTimeNextCast = 0.0f;
    private float shieldTimeDuration = 0.0f;
	// Use this for initialization
	void Start () {
        if (ShieldPrefab == null)
        {
            Debug.LogError("No Shield???");
        }
		
	}

    // Update is called once per frame
    void Update() {
        Shield();
    }

    void Shield()
    {
        //manage shield cooldown
        if (Input.GetButtonDown(shield) && Time.time > shieldTimeNextCast)
        {
            shieldTimeNextCast = Time.time + ShieldCooldown;
            shieldTimeDuration = Time.time + ShieldDuration;
        }
        if(Input.GetButtonUp(shield) && Time.time < shieldTimeDuration)
        {
            shieldTimeDuration = 0.0f;
        }
        ShieldPrefab.SetActive((Input.GetButton(shield) && Time.time < shieldTimeNextCast) && Time.time < shieldTimeDuration);
    }
    
        
         
        
    

}
