using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBody : MonoBehaviour
{
    public Rigidbody mainBody;
    public Vector2 randomForceRange;
    public void Push(Vector3 force)
    {
        Vector2 r = Random.insideUnitCircle * Random.Range(randomForceRange.x, randomForceRange.y);

        Vector3 cross = Vector3.Cross(force, Vector3.up);
        Vector3 offset = cross * r.x + Vector3.up * r.y;
        
        mainBody.AddForceAtPosition(force * Random.Range(1,1.3f), mainBody.position + offset);
    }
}
