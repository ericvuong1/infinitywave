using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public float fireRate = 0;
    public int Damage = 10;
    public LayerMask whatToHit;
    

    public Transform BulletTrailPrefab;
    public Transform MuzzleFlashPrefab;
    public Transform HitPrefab;
    public Transform NinjaStarPrefab;

    //camera shake
    public float camShakeAmt = 0.001f;
    public float camShakeLength = 0.1f;
    CameraShake camShake;


    float timeToSpawnEffect = 0;
    float timeToFire = 0;
    public float effectSpawnRate = 10;
    Transform firePoint;




	// Use this for initialization
	void Awake () {
#pragma warning disable CS0618 // Type or member is obsolete
        firePoint = transform.FindChild("FirePoint");
#pragma warning restore CS0618 // Type or member is obsolete
        if (firePoint == null)
        {
            Debug.LogError("No firepoint ? WHAT?!");
        }
	}
    private void Start()
    {
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        if(camShake == null)
        {
            Debug.LogError("No CameraShake Script found on GM object.");
        }
    }

    // Update is called once per frame
    void Update () {
        if (fireRate == 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if(Input.GetButton("Fire1") && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1 / fireRate;
                Shoot();
            }
        }
		
	}
    Vector2 debugStart;
    Vector2 debugDir;
    void Shoot()
    {
        //set raycast
        Vector2 mousePosition = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x,
        Camera.main.ScreenToWorldPoint (Input.mousePosition).y);

        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
        RaycastHit2D hit = Physics2D.Raycast (firePointPosition,mousePosition-firePointPosition, 100, whatToHit);

        debugStart = firePointPosition;
        debugDir = (mousePosition - firePointPosition).normalized * Mathf.Sqrt(Mathf.Pow(Screen.width, 2) + Mathf.Pow(Screen.height, 2)) * 1.05f / 100;
        
       // Debug.DrawLine(firePointPosition, (mousePosition-firePointPosition)*100, Color.cyan);
        if (hit.collider != null)
        {
            //Debug.DrawLine(firePointPosition, hit.point, Color.red);
            Ennemy ennemy = hit.collider.GetComponent<Ennemy>();
            if (ennemy != null)
            {
                Debug.Log("We hit " + hit.collider.name + " and did " + Damage + " damage.");
                ennemy.DamageEnnemy(Damage);
            }
        }
        if (Time.time >= timeToSpawnEffect)
        {
            Vector3 hitPos;
            Vector3 hitNormal;
            if (hit.collider == null)
            {
                hitPos = (mousePosition - firePointPosition) * 30;
                hitNormal = new Vector3(9999, 9999, 9999);
            }
            else
            {
                hitPos = hit.point;
                hitNormal = hit.normal;
            }
            Effect(hitPos, hitNormal);
            timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
        }
    }

    void Effect(Vector3 hitPos, Vector3 hitNormal)
    {
        //bullet
        Transform trail = Instantiate(BulletTrailPrefab, firePoint.position, firePoint.rotation);
        LineRenderer lr = trail.GetComponent<LineRenderer>();
        if(lr != null)
        {
            lr.SetPosition(0, firePoint.position);
            lr.SetPosition(1, hitPos);
        }
        Destroy(trail.gameObject, 0.04f);
        if(hitNormal != new Vector3 (9999, 9999, 9999))
        {
            Transform star = (Transform)Instantiate(NinjaStarPrefab, hitPos, transform.rotation);
            Transform hitParticle = (Transform) Instantiate(HitPrefab, hitPos, Quaternion.FromToRotation(Vector3.forward, hitNormal));
            Destroy(hitParticle.gameObject, 1f);
            Destroy(star.gameObject, 1f);
        }

        //muzzle
        Transform clone = (Transform) Instantiate(MuzzleFlashPrefab, firePoint.position, firePoint.rotation);
        clone.parent = firePoint;
        float size = Random.Range(0.3f, 0.6f);          //generate random scale 
        clone.localScale = new Vector3(size, size, size);
        Destroy(clone.gameObject, 0.02f);

        //Shake Camera
        camShake.ShakeCamera(camShakeAmt, camShakeLength);
     }

    //private void OnDrawGizmos()
    //{
    //    //See raycast for shot
    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawLine(debugStart, debugDir + debugStart);
    //}
}
