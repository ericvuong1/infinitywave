using UnityEngine;
using System.Collections;

public class ImpactTesting : MonoBehaviour
{
	public GameObject[] m_Targets;
	public GameObject m_Projectile;
	[Range(1, 240)]
	public int m_ShotsPerMinute = 30;
	public bool m_RandomDirection;
	float _Timer;
	GameObject _Camera;

	private bool toggleTxt = false;

	void Start() 
	{
		_Camera = GameObject.Find("Main Camera").gameObject;

		StartCoroutine("AutoFire");
	}

	void Update() 
	{

	}

	IEnumerator AutoFire()
	{
		while(this)
		{
			Fire();

			yield return new WaitForSeconds((float)60/m_ShotsPerMinute);
		}
	}

	void Fire()
	{
		if(m_Targets.Length <= 0 || m_Projectile == null)
		{
			return;
		}

		Vector3 start= new Vector3();

		if(m_RandomDirection)
		{
			start = new Vector3(Random.Range(-1.0f,1.0f), Random.Range(-1.0f,1.0f), Random.Range(-1.0f,1.0f));

			start.Normalize();
			
			start *= 5;
		}
		else
		{
			start = _Camera.transform.position;
			start.y -= 1;
		}
				
		GameObject projectile = (GameObject)Instantiate(m_Projectile, start, Quaternion.identity);

		int i = Random.Range(0, int.MaxValue) % m_Targets.Length;

		projectile.transform.LookAt(m_Targets[i].transform.position);
		
		if(projectile.gameObject.GetComponent<Rigidbody>())
		{
			projectile.gameObject.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * 10, ForceMode.Impulse);
		}
	}

	void OnGUI()
	{
		m_RandomDirection = GUI.Toggle(new Rect(10, 10, 200, 30), m_RandomDirection, "Random Projectile Direction");
		GUI.Label(new Rect(10, 40, 180, 30), "Rate of Fire");
		m_ShotsPerMinute = (int)GUI.HorizontalSlider(new Rect(10, 70, 180, 30), m_ShotsPerMinute, 1, 240);
	}
}
