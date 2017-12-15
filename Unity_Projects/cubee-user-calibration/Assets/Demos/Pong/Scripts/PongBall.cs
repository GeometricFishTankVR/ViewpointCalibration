using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongBall : MonoBehaviour
{
    private Rigidbody rBody;

    [SerializeField]
    private float InitialSpeed = 0.75F;
    private float Speed;

    [SerializeField, Tooltip( "Linear increase of speed when the ball bounces" )]
    private float BounceIncrease = 0.05F;

    private Vector3 latestVelocity;

    public PongGame Game;

    public AudioClip BounceSound;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        StartCoroutine( ResetBall() );
    }

    private void FixedUpdate()
    {
        latestVelocity = rBody.velocity;

        // Ball freezing glitch
        if( Speed > 0 )
        {
            if( latestVelocity.magnitude < Mathf.Epsilon ) rBody.velocity = AsVec3( Random.insideUnitCircle ) * Speed;
            else rBody.velocity = rBody.velocity.normalized * Speed;
        }
    }

    /// <summary>
    /// ( x, y, z ) to ( x, z )
    /// </summary>
    public static Vector2 AsVec2( Vector3 v ) { return new Vector2( v.x, v.z ); }

    /// <summary>
    /// ( x, y ) to ( x, 0, y )
    /// </summary>
    public static Vector3 AsVec3( Vector2 v ) { return new Vector3( v.x, 0, v.y ); }

    private void OnCollisionEnter( Collision collision )
    {
        Speed += BounceIncrease;

        // 
        AudioSource.PlayClipAtPoint( BounceSound, collision.contacts[0].point, 0.7F );

        if( collision.collider.name.Contains( "Net" ) )
        {
            // Out of bounds!
            StartCoroutine( ResetBall() );

            // Trigger score event
            if( collision.collider.name.Contains( "Two" ) ) Game.TriggerGoalEvent( PongPlayer.One );
            else Game.TriggerGoalEvent( PongPlayer.Two );
        }
        else
        {
            var reflect = Vector3.zero;

            foreach( var contact in collision.contacts )
            {
                var n = contact.normal.normalized;
                var v = latestVelocity.normalized;
                var r = Vector3.Reflect( v, n );

                // Debug.LogFormat( "V: {0} N: {1} R: {2}", v, n, r );

                reflect += r;
            }

            // New velocity is reflected
            rBody.velocity = reflect.normalized * Speed;
        }
    }

    public IEnumerator ResetBall()
    {
        var mr = GetComponent<MeshRenderer>();
        var tr = GetComponent<TrailRenderer>();
        mr.enabled = false;

        // Stop ball
        rBody.velocity = Vector3.zero;
        Speed = 0F;

        yield return new WaitForSeconds( 1F );

        // Clear trail
        tr.Clear();

        mr.enabled = true;

        // Move back to center
        rBody.position = Game.transform.position;

        yield return new WaitForSeconds( 1F );

        // Reset speed
        Speed = InitialSpeed;

        // Choose random up or down
        var direction = Random.value > 0.5 ? +1 : -1F;

        // Set initial rigidbody state
        rBody.velocity = Vector3.Normalize( new Vector3( Random.Range( -0.2F, +0.2F ), 0, direction ) ) * Speed;
    }
}
