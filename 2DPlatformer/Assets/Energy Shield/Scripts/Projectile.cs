using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour 
{

	float _KillTime = 10;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		_KillTime -= Time.deltaTime;

		if(_KillTime <= 0)
		{
			Destroy(gameObject);
		}
	}
}
