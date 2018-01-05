using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour {

    public Transform[] backgrounds;         //Array (list) of all the back- and foregrounds to be parallaxed
    private float[] parallaxScales;         //Proportion of camera's movement to move the backgrounds by
    public float smoothing = 1f;            //How smooth the parralax is going to be. Have to be above 0.

    private Transform cam;                  //Stores the camera's transform.
    private Vector3 previousCamPos;         //The position of camera in previous frame

    //Is called before Start(). Great for references.
    void Awake()
    {
        //setup camera reference
        cam = Camera.main.transform;
    }

	// Use this for initialization
	void Start () {
        //The previous frame had the current frame's camera position
        previousCamPos = cam.position;
        
        //assigning corresponding parallaxScales
        parallaxScales = new float[backgrounds.Length];
        for(int i = 0; i<backgrounds.Length; i++){
            parallaxScales[i] = backgrounds[i].position.z * -1;
        }

	}
	
	// Update is called once per frame
	void Update () {
        //for each background
        for (int i = 0; i < backgrounds.Length; i++)
        {
            //parallax is the opposite of the camera movement
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];
            // set a target x position which is the current position + the parallax 
            float backgroundTargetPosX = backgrounds[i].position.x + parallax;
            // create a target position which is the background's current position with its target x position
            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX,
                        backgrounds[i].position.y, backgrounds[i].position.z);
            //fade between current position and the target position using lerp
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
        }

        //set the previousCamPos to the camera's position at the end of the frame
        previousCamPos = cam.position;
    }
}
