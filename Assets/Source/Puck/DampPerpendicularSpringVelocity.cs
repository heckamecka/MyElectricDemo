using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpringJoint2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class DampPerpendicularSpringVelocity : MonoBehaviour {

    //Tweakables:
    [Tooltip("Each Physics frame, reduce perpendicular velocity by this percentage")]
    [Range(0f, 1f)] [SerializeField] private float velocityReductionPerFrame = 0.05f;

    //Cache:
    private SpringJoint2D mSpring;
    private Rigidbody2D mBody;

    private Vector2 toAttached    = Vector2.zero;
    private Vector2 rightAttached = Vector2.zero;

    private void OnDrawGizmos()
    {
        if(toAttached != Vector2.zero) {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, this.transform.position + 1000f * (Vector3)toAttached);
        }

        if (rightAttached != Vector2.zero) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, this.transform.position + 1000f * (Vector3)rightAttached);
        }
    }

    private void Awake() {
        mSpring = this.GetComponent<SpringJoint2D>();
        mBody =   this.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        Vector2 velocity      = mBody.velocity;
        toAttached    = (mSpring.connectedBody.position - mBody.position).normalized;
        rightAttached = Vector3.Cross(toAttached, Vector3.forward);

        Vector2 parallelVelocity      = Vector3.Project(velocity, toAttached);
        Vector2 perpendicularVelocity = Vector3.Cross  (velocity, rightAttached);

        mBody.velocity =  parallelVelocity +                   //Use 100% of the velocity towards the target
            perpendicularVelocity * (1f - velocityReductionPerFrame); //Reduce perpenduclar velocity by velocityReductionPerFrame   
    }
}
