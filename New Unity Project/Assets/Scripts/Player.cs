using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public Transform head;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void teleport(Vector3 playerFeetWorldSpace, Vector3 playerDirectionWorldSpace)
	{
		Vector3 currFootPosTrackSpace = this.transform.worldToLocalMatrix.MultiplyPoint(head.position);
		currFootPosTrackSpace.y = 0;
		Vector3 currFootPosWorldSpace = this.transform.localToWorldMatrix.MultiplyPoint(currFootPosTrackSpace);

		Vector3 footOffsetWorldSpace = playerFeetWorldSpace - currFootPosWorldSpace;
		this.transform.Translate(footOffsetWorldSpace, Space.World);
	}
}
