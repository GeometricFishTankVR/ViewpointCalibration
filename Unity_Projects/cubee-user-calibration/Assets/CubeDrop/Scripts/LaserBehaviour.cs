using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class LaserBehaviour : MonoBehaviour
{
    private Rigidbody Rigidbody;
    //private BoxCollider Collider;

    private float TimeToLive = 0.5F;

    void Start()
    {
        // 
        Rigidbody = GetComponent<Rigidbody>();
        //Collider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        //
        var vel = Rigidbody.velocity.normalized;
        var angle = Mathf.Atan2(vel.x, vel.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

        TimeToLive -= Time.smoothDeltaTime;
        if (TimeToLive <= 0)
            Destroy(gameObject);

        Rigidbody.velocity *= 1.1F;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "drop_Cube")
        {
            Destroy(gameObject);

            var cube = col.gameObject;
            var cube_Behaviour = cube.GetComponent<CubeBehaviour>();
            cube_Behaviour.TakeDamage();
        }
    }
}
