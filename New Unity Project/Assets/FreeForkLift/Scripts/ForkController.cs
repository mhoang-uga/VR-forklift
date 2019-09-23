using UnityEngine;
using System.Collections;

public class ForkController : MonoBehaviour {

    public Transform fork; 
    public Transform mast;
    public float speedTranslate; //Platform travel speed
    public Vector3 maxY; //The maximum height of the platform
    public Vector3 minY; //The minimum height of the platform
    public Vector3 maxYmast; //The maximum height of the mast
    public Vector3 minYmast; //The minimum height of the mast
    public Vector3 maxX; //farthest right
    public Vector3 minX; //farthest left of the platform 
    public OVRInput.Controller forkController; // snip?

    private bool mastMoveTrue = false; //Activate or deactivate the movement of the mast
    private bool sideShiftEngaged = false;
    private HingeJoint[] levers;

    // Update is called once per frame
    void FixedUpdate () {

        Debug.Log(mastMoveTrue);
        if(fork.transform.localPosition.y >= maxYmast.y && fork.transform.localPosition.y < maxY.y)
        {
            mastMoveTrue = true;
        }
        else
        {
            mastMoveTrue = false;

        }

        if (fork.transform.localPosition.y <= maxYmast.y)
        {
            mastMoveTrue = false;
        }
        // change engaged fork lever (may need to move to hand to respond to grabbed Rigidbody (snip?)
        // OR replace sideShiftEngaged with the direction condition being reliant on which of the HingeJoints[] is being moved out of position?
        // (might have to consider how the Vector3.MoveTowards would be interacting with each other)
        // (would be able to use both levers simultaneously tho [for the speedrunners])
        if (OVRInput.Get(OVRInput.Button.Two) || Input.GetKey(KeyCode.LeftBracket)) // engage up/down
        {
            sideShiftEngaged = false;
        }
        if (OVRInput.Get(OVRInput.Button.One) || Input.GetKey(KeyCode.RightBracket)) // engage left/right
        {
            sideShiftEngaged = true;
        }
        if (Input.GetKey(KeyCode.PageUp)) // || OVRInput.Get(OVRInput.Button.One))
        {
            if (!sideShiftEngaged)
            {
                //fork.Translate(Vector3.up * speedTranslate * Time.deltaTime);
                fork.transform.localPosition = Vector3.MoveTowards(fork.transform.localPosition, maxY, speedTranslate * Time.deltaTime);
                if (mastMoveTrue)
                {
                    mast.transform.localPosition = Vector3.MoveTowards(mast.transform.localPosition, maxYmast, speedTranslate * Time.deltaTime);
                }
            } else {
                fork.transform.localPosition = Vector3.MoveTowards(fork.transform.localPosition, maxX, speedTranslate * Time.deltaTime);
            }
          
        }
        if (Input.GetKey(KeyCode.PageDown)) // || OVRInput.Get(OVRInput.Button.Two))
        {
            if (!sideShiftEngaged)
            {
                //fork.Translate(Vector3.up * speedTranslate * Time.deltaTime);
                fork.transform.localPosition = Vector3.MoveTowards(fork.transform.localPosition, minY, speedTranslate * Time.deltaTime);
                if (mastMoveTrue)
                {
                    mast.transform.localPosition = Vector3.MoveTowards(mast.transform.localPosition, minYmast, speedTranslate * Time.deltaTime);
                }
            }
            else
            {
                fork.transform.localPosition = Vector3.MoveTowards(fork.transform.localPosition, minX, speedTranslate * Time.deltaTime);
            }
        }

    }
}
