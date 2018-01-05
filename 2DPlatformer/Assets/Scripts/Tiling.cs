using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]

public class Tiling : MonoBehaviour {

    public int offsetX = 2;         // offset so we dont get weird errors

    //used for checking if need to instantiate
    public bool hasARightBuddy = false;
    public bool hasALeftBuddy = false;

    public bool reverseScale = false;       //use if the object is not tilable

    private float spriteWidth = 0f;             // width of the element
    private Camera cam;
    private Transform myTransform;

    private void Awake()
    {
        cam = Camera.main;
        myTransform = transform;
    }


    // Use this for initialization
    void Start () {
        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
        spriteWidth = sRenderer.sprite.bounds.size.x;
	}
	
	// Update is called once per frame
	void Update () {
		if(!hasALeftBuddy || !hasARightBuddy)
        {
            //calculate the cameras extend (half the width) of what the camera can see in world coordinate
            float camHorizontalExtend = cam.orthographicSize * Screen.width / Screen.height;

            //calculate the x position where the camera can see the edge of the sprite (element)
            float edgeVisiblePositionRight = (myTransform.position.x + spriteWidth / 2) - camHorizontalExtend;
            float edgeVisiblePositionLeft = (myTransform.position.x - spriteWidth / 2) + camHorizontalExtend;
            //checking if we can see the edge of the element and then calling MakeNewBuddy if we can
            if(cam.transform.position.x>=edgeVisiblePositionRight-offsetX && !hasARightBuddy)
            {
                MakeNewBuddy(1);
                hasARightBuddy = true;
            } else if (cam.transform.position.x <= edgeVisiblePositionLeft + offsetX && !hasALeftBuddy)
            {
                MakeNewBuddy(-1);
                hasALeftBuddy = true;
            }
        }
	}
    //makes a buddy on the side required
    void MakeNewBuddy (int rightOrLeft) // 1 == right, -1 == left
    {
        //calculating new position for new buddy
        Vector3 newPosition = new Vector3(myTransform.position.x + spriteWidth * rightOrLeft, 
                                                myTransform.position.y, myTransform.position.z);
        Transform newBuddy = (Transform) Instantiate(myTransform, newPosition, myTransform.rotation);
        //if not tilable let's reverse the x size of our object to get rid of ugly seams
        if (reverseScale)
        {
            newBuddy.localScale = new Vector3(newBuddy.localScale.x * -1, 1, 1); //change the local scale
        }
        newBuddy.parent = myTransform.parent;
        if (rightOrLeft > 0)
        {
            newBuddy.GetComponent<Tiling>().hasALeftBuddy = true;
        }
        else
        {
            newBuddy.GetComponent<Tiling>().hasARightBuddy = true;
        }
    }
}
