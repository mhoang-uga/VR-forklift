using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    public Transform holder;
    public Rigidbody rb;
    private Vector3 positionHolder;
    private Quaternion rotationHolder;
    private bool saveGravity;
    private float saveMaxAngularVelocity;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (holder != null)
        {
            Vector3 destination = holder.localToWorldMatrix.MultiplyPoint(positionHolder);
            Vector3 currentPosition = this.transform.position;
            Quaternion destinationRot = holder.rotation * rotationHolder;
            Quaternion currentRot = this.transform.rotation;


            rb.velocity = (destination - currentPosition) / Time.fixedDeltaTime;
            Quaternion offRot = destinationRot * Quaternion.Inverse(currentRot);
            float angle; Vector3 axis;
            offRot.ToAngleAxis(out angle, out axis); //returns 2 values
            Vector3 rotationDiff = angle * Mathf.Deg2Rad * axis;
            rb.angularVelocity = rotationDiff / Time.fixedDeltaTime;
        }
    }

    public void grabbed(Transform t)
    {
        if (holder != null)
        {
            return;
        }
        positionHolder = t.worldToLocalMatrix.MultiplyPoint(this.transform.position);
        rotationHolder = Quaternion.Inverse(t.rotation) * this.transform.rotation;

        saveGravity = rb.useGravity;
        rb.useGravity = false;
        saveMaxAngularVelocity = rb.maxAngularVelocity;
        rb.maxAngularVelocity = Mathf.Infinity; // allows spinning as fast as you want
        holder = t;
    }

    public void released(Transform t, Vector3 vel)
    {
        if (t == holder)
        {
            rb.useGravity = saveGravity;
            rb.maxAngularVelocity = saveMaxAngularVelocity;
            rb.velocity = vel;
            holder = null;
        }
    }
}
