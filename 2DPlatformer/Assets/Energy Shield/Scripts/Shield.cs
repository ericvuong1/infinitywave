using UnityEngine;
using System.Collections.Generic;

public class Shield : MonoBehaviour 
{

	public Color m_ShieldColour = new Color(1,1,1,1);

	public int m_MaxNumberOfImpacts = 4;
	public GameObject m_RippleEffect;

	Material _ShieldMat;
	List<ImpactEvent> _ImpactEvents;
	List<int> _CleanUp;

	bool _HardSphere = false;

	Triangle[] _Triangles;

	[Range(1, 30)]
	public float m_Speed = 1;

	float _Time;
	Color _White = new Color(1,1,1,1);
	Color _Black = new Color(0,0,0,0);
	

	void Start() 
	{
		if(m_RippleEffect == null)
		{
			Debug.LogError("There is no Ripple Effect object assigned! Please assign a Ripple Effect prefab in the inspector.");
		}

		_ShieldMat = (Material)GetComponent<Renderer>().material;
		if(_ShieldMat == null)
		{
			Debug.LogError("There is no Material assigned to this object! Please assign a Material in the inspector.");
		}

		_ImpactEvents = new List<ImpactEvent>();
		_CleanUp = new List<int>();

		if(gameObject.name.Contains("Hard"))
		{
			_HardSphere = true;
		}

		SetColours();
	}

	void SetColours()
	{
		if(_HardSphere || gameObject.name.Contains("Hard"))
		{
			if(gameObject.name.Contains("Smooth"))
			{
				GetComponent<Renderer>().sharedMaterial.DisableKeyword("HARD_FACES");
				return;
			}

			Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
			int[] triangles = mesh.triangles;

			_Triangles = new Triangle[triangles.Length / 3];

			Color[] colours = new Color[mesh.vertices.Length];

			int index = 0;
			for(int i = 0; i < triangles.Length; i+=3)
			{
				float value = Random.Range(0.0f, 1.0f);
				Color faceColour = new Color(value, value, value, value);

				_Triangles[index].currentOffset = Random.Range(0.0f, 1.0f);
				_Triangles[index].currentScale = Random.Range(0.0f, 1.0f);
				_Triangles[index].currentPos = Random.Range(0.0f, 1.0f);
				_Triangles[index].newOffset = Random.Range(0.0f, 1.0f);
				_Triangles[index].newScale = Random.Range(0.0f, 1.0f);
				_Triangles[index].newPos = Random.Range(0.0f, 1.0f);

				colours[i] = faceColour;
				colours[i+1] = faceColour;
				colours[i+2] = faceColour;

				index++;
			}
			
			mesh.colors = colours;

			GetComponent<Renderer>().sharedMaterial.EnableKeyword("HARD_FACES");
		}
	}

	void ShiftColour()
	{
		if(_HardSphere)
		{
			Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
			int[] triangles = mesh.triangles;
			Color[] colours = mesh.colors;

			_Time += Time.deltaTime * m_Speed;

			for(int i = 0; i < _Triangles.Length; i++)
			{
				Vector3 currentVal = new Vector3(_Triangles[i].currentOffset, _Triangles[i].currentScale, _Triangles[i].currentPos);
				Vector3 newVal = new Vector3(_Triangles[i].newOffset, _Triangles[i].newScale, _Triangles[i].newPos);
				currentVal = Vector3.MoveTowards(currentVal, newVal, Time.deltaTime);

				_Triangles[i].currentOffset = currentVal.x;
				_Triangles[i].currentScale = currentVal.y;
				_Triangles[i].currentPos = currentVal.z;

				if(currentVal == newVal)
				{
					_Triangles[i].newOffset = Random.Range(0.0f, 1.0f);
					_Triangles[i].newScale = Random.Range(0.0f, 1.0f);
					_Triangles[i].newPos = Random.Range(0.0f, 1.0f);
				}
			}

			int index = 0;
			for(int i = 0; i < triangles.Length; i+=3)
			{
				float scale = Random.Range(0.0f, 1.0f);
				float offset = Random.Range(0.0f, 1.0f);

				Color newColour = Color.Lerp(_Black, _White, 
                         _Triangles[index].currentScale * Mathf.Sin(m_Speed * (Time.deltaTime - _Triangles[index].currentOffset))
                         + _Triangles[index].currentPos);

				colours[i]	 = newColour;
				colours[i+1] = newColour;
				colours[i+2] = newColour;

				index++;
			}
			
			mesh.colors = colours;
		}
	}

	void OnValidate()
	{
		if(_HardSphere || gameObject.name.Contains("Hard"))
		{
			SetColours();
		}
		else
		{
			GetComponent<Renderer>().sharedMaterial.DisableKeyword("HARD_FACES");
		}
	}

	void FixedUpdate()
	{
		ShiftColour();
	}

	void Update() 
	{
		_ShieldMat.color = m_ShieldColour;

		bool cleanUp = false;

		for(int i = 0; i < _ImpactEvents.Count; i++)
		{
			_ImpactEvents[i].offset.x -= Time.deltaTime;
			_ImpactEvents[i].material.mainTextureOffset = _ImpactEvents[i].offset;
			_ImpactEvents[i].material.color = m_ShieldColour;

			if(_ImpactEvents[i].offset.x < 0)
			{
				cleanUp = true;
				_CleanUp.Add(i);
			}
		}

		if(cleanUp)
		{
			for(int i = 0; i < _CleanUp.Count; i++)
			{
				Destroy(_ImpactEvents[i].RippleEffect);
				_ImpactEvents.RemoveAt(_CleanUp[i]);
			}

			_CleanUp.Clear();
			cleanUp = false;
		}
	}

	void OnCollisionEnter(Collision other)
	{
		Vector3 impact = (other.contacts[0].point) - transform.position;

		if(_ImpactEvents.Count < m_MaxNumberOfImpacts)
		{
			if(m_RippleEffect != null)
			{
				GameObject shell = (GameObject)Instantiate(m_RippleEffect, transform.position, Quaternion.identity);
				shell.transform.LookAt(impact + transform.position);
				Vector3 offset = shell.transform.position;
				offset.z += Random.Range(-1f, 1f) / 100;
                //shell.transform.position = offset;
                //shell.transform.localScale = transform.localScale;
                shell.transform.parent = transform;
                shell.transform.localScale = Vector3.one * 0.95f;

                ImpactEvent newEvent = new ImpactEvent();
				newEvent.RippleEffect = shell;
				newEvent.pos = transform.position + impact * 3;
				newEvent.material = shell.GetComponent<Renderer>().material;
				newEvent.material.SetVector("_ImpactPos", new Vector4(newEvent.pos.x, newEvent.pos.y, newEvent.pos.z));

				newEvent.offset.x = 2;

				_ImpactEvents.Add(newEvent);
			}
		}
		else
		{
			// Maximum number of Ripple effect objects reached, re-using one of the existing ones
			float lowest = 10.0f;
			int index = -1;

			for(int i = 0; i < _ImpactEvents.Count; i++)
			{
				if(_ImpactEvents[i].offset.x < lowest)
				{
					lowest = _ImpactEvents[i].offset.x;
					index = i;
				}
			}

			_ImpactEvents[index].RippleEffect.transform.LookAt(impact);
			_ImpactEvents[index].pos = transform.position + impact * 3;
			_ImpactEvents[index].material.SetVector("_ImpactPos", new Vector4(
																			_ImpactEvents[index].pos.x, 
																			_ImpactEvents[index].pos.y, 
																			_ImpactEvents[index].pos.z));
			_ImpactEvents[index].offset.x = 2;

		}
	}

	class ImpactEvent
	{
		public Vector3 pos;
		public Vector2 offset;
		public GameObject RippleEffect;
		public Material material;
	}

	struct Triangle
	{
		public float currentOffset;
		public float newOffset;
		public float currentScale;
		public float newScale;
		public float currentPos;
		public float newPos;
	};
}
