using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy_modified : MonoBehaviour
{
    [SerializeField] float waterLevel = 0.0f;

    [Tooltip("Maximum buoyancy force when the object is fully submerged (in kg)." +
        "Adjust this to be higher than rb.mass to make it buoyant, lesser to make it sink.")]
    [SerializeField] float totalBuoyancy;

    Rigidbody rb;
    BoxCollider boxCollider;
    //float dt;

    //Vector3 colliderTop;
    Vector3 colliderBottom;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        //dt = Time.fixedDeltaTime;

        if (rb == null || boxCollider == null)
        {
            Debug.LogError("Rigidbody and BoxCollider components are required.");
            enabled = false;
        }

        //colliderTop = transform.position + transform.up * (boxCollider.size.y * 0.5f * transform.localScale.y);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        colliderBottom = transform.position - transform.up * (boxCollider.size.y * 0.5f * transform.localScale.y);
        //Debug.Log(colliderBottom.y);    

        float submersionDepth = Mathf.Clamp(waterLevel - colliderBottom.y, 0, boxCollider.size.y * transform.localScale.y);
        float submersionPercentage = submersionDepth / (boxCollider.size.y * transform.localScale.y);
        //Debug.Log(submersionPercentage);

        Vector3 buoyancyForce = - totalBuoyancy * submersionPercentage * Physics.gravity;
        //Debug.Log(buoyancyForce);
        rb.AddForce(buoyancyForce);
    }
}
