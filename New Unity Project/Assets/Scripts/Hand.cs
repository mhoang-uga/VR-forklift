using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
	public GameObject teleporterIndicatorPrefab;
	public List<GameObject> teleporterIndicators = new List<GameObject>();
	bool teleporterActive = false;
	public Vector3 teleporterSelectionPoint;
	public float teleporterPower;
	public Transform head;
	public Player player;
	public OVRInput.Controller myController;
	public float teleTriggerSqueezeThreshold; //teleportation thresholds
	public float teleTriggerReleaseThreshold;
    public float pickupTriggerThreshold; //grabbing thresholds
    public float releaseTriggerThreshold;
    public GrabbableObject currentObj = null;
    public bool disappearOnPickup;

    private bool inForklift;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!inForklift)
        {
            float triggerSqueeze = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, myController);

            if (triggerSqueeze > teleTriggerSqueezeThreshold && !teleporterActive)
            {
                teleporterActive = true;
            }
            else if (teleporterActive && triggerSqueeze < teleTriggerReleaseThreshold)
            {
                player.teleport(teleporterSelectionPoint, Vector3.zero);
                for (int i = 0; i < teleporterIndicators.Count; i++)
                {
                    GameObject.Destroy(teleporterIndicators[i]);
                }

                teleporterIndicators.Clear();
                teleporterActive = false;
            }
            int currPointInd = 1;
            Vector3 currPoint = this.transform.position;
            Vector3 currVel = this.transform.forward * teleporterPower;
            float t = 0;
            float time_increment = 0.1f;
            if (teleporterActive)
            {
                while (t < 5)
                {
                    Vector3 nextPoint = currPoint + currVel * teleporterPower * time_increment;
                    currVel = currVel + new Vector3(0, -10, 0) * time_increment;

                    RaycastHit hit;
                    bool gotHit = false;

                    if (Physics.Raycast(currPoint, (nextPoint - currPoint).normalized, out hit, (nextPoint - currPoint).magnitude))
                    {
                        teleporterSelectionPoint = hit.point + new Vector3(0, 0.5f, 0); //extra offset to prevent falling through floor
                        gotHit = true;
                    }

                    if (currPointInd > teleporterIndicators.Count)
                    {
                        teleporterIndicators.Add(GameObject.Instantiate(teleporterIndicatorPrefab));
                    }

                    if (gotHit)
                    {
                        teleporterIndicators[teleporterIndicators.Count - 1].transform.position = hit.point;
                        break;
                    }

                    teleporterIndicators[currPointInd - 1].transform.position = nextPoint;

                    currPoint = nextPoint;
                    currPointInd++;
                    t += time_increment;
                }
                List<GameObject> temp = new List<GameObject>();
                for (int i = 0; i < teleporterIndicators.Count; i++)
                {
                    if (i < currPointInd)
                        temp.Add(teleporterIndicators[i]);
                    else
                        GameObject.Destroy(teleporterIndicators[i]);
                }
                teleporterIndicators = temp;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb == null)
        {
            return;
        }
        GrabbableObject go = rb.GetComponent<GrabbableObject>();

        if (go != null)
        {
            float triggerValue;
            if (myController == OVRInput.Controller.LTouch)
            {
                triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
            }
            else
            {
                triggerValue = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
            }

            if (currentObj == null && triggerValue > pickupTriggerThreshold)
            {
                currentObj = go;
                currentObj.grabbed(this.transform);
                if (disappearOnPickup)
                {
                    MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer m in meshRenderers)
                    {
                        m.enabled = false;
                    }
                }
            }

            if (currentObj != null && triggerValue < releaseTriggerThreshold)
            {
                currentObj.released(this.transform, OVRInput.GetLocalControllerVelocity(myController));
                currentObj = null;
                if (disappearOnPickup)
                {
                    MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer m in meshRenderers)
                    {
                        m.enabled = true;
                    }
                }
            }
        }

    }
}
