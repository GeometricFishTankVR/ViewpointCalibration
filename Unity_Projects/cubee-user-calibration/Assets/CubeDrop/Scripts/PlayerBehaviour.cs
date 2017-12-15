using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
// [RequireComponent( typeof( BoxCollider ) )]
public class PlayerBehaviour : MonoBehaviour
{
    public Transform ViewpointReference;

    public GameObject LaserPrefab;

    public AudioClip LaserSoundEffect;

    //private Transform Transform;
    private Rigidbody Rigidbody;
    //private SphereCollider SphereCollider;
    //private BoxCollider BoxCollider;

    private GameObject Feet;
    private GameObject Flip;

    private float TargetAimAngle = 0F;
    private float AimAngle = 0F;

    private float ShootTimer = 0F;
    private bool IsOnGround = false;
    private bool IsJumping = false;

    const float MovementSpeed = 2.1F;
    const float JumpSpeed = 7F;

    float FrontFlipTime = 0F;
    float TargetFrontFlip = 0F;
    float FrontFlip = 0F;

    void Start()
    {
        // 
        //Transform = GetComponent<Transform>();
        Rigidbody = GetComponent<Rigidbody>();
        //SphereCollider = GetComponent<SphereCollider>();
        //BoxCollider = GetComponent<BoxCollider>();

        // 
        Feet = transform.FindChild("feet").gameObject;
        Flip = transform.FindChild("flip").gameObject;
    }

    void SpawnLaser()
    {
        var laser = Instantiate(LaserPrefab, transform.parent, false);
        var laser_Rigidbody = laser.GetComponent<Rigidbody>();
        //var laser_Behaviour = laser.GetComponent<LaserBehaviour>();

        // 
        AudioSource.PlayClipAtPoint(LaserSoundEffect, Rigidbody.position, 2F);

        var vec = CreateGroundVector(AimAngle * Mathf.Deg2Rad);
        laser_Rigidbody.velocity = vec * 10F; // Laser speed
        laser.transform.localPosition = transform.localPosition + Vector3.up * 0.75F;
        laser.transform.position += vec * 0.3F;
    }

    Vector2 GetForwardAxis()
    {
        var viewpoint = ViewpointReference.position;
        var p = Vector3.ProjectOnPlane(viewpoint.normalized, Vector3.up).normalized;
        return new Vector2(p.x, p.z);
    }

    static Vector2 GetMovementAxis()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        return new Vector2(x, y);
    }

    static Vector2 GetAimAxis()
    {
        var x = Input.GetAxis("HorizontalTurn");
        var y = Input.GetAxis("VerticalTurn");
        return new Vector2(x, y);
    }

    static float ComputeAngle(Vector2 v)
    {
        return Mathf.Atan2(-v.y, v.x);
    }

    static Vector3 CreateGroundVector(float angle)
    {
        float x = Mathf.Sin(angle);
        float z = Mathf.Cos(angle);
        return new Vector3(x, 0, z);
    }

    void FixedUpdate()
    {
        // Get Axis
        var aim = GetAimAxis();
        var mov = GetMovementAxis();
        var fwd = GetForwardAxis();

        // Get Angles
        var fwdAngle = ComputeAngle(fwd);
        var aimAngle = ComputeAngle(aim) + fwdAngle;
        var movAngle = ComputeAngle(mov) + fwdAngle;

        // == Aim & Shoot == //

        // Computes the motion vector ( left axis on floor )
        var motion = Vector3.zero;
        if (mov.magnitude > 0.0F)
        {
            motion += CreateGroundVector(movAngle);
            motion *= mov.magnitude;
        }

        // If aiming, point and shoot
        if (aim.magnitude > 0F)
        {
            // Accumulate time
            ShootTimer += Time.fixedDeltaTime;

            // If a tenth of a second has passed, shoot a laser!
            if (ShootTimer > 0.1F)
            {
                ShootTimer -= 0.1F;
                SpawnLaser();
            }

            // Updates the player objects angle.
            TargetAimAngle = aimAngle;
        }
        else
        {
            //
            if (mov.magnitude > 0)
                TargetAimAngle = movAngle;

            // Not shooting, no time.
            ShootTimer = 0F;
        }

        // Rotate the character to point where the axis is aiming
        AimAngle = Mathf.LerpAngle(AimAngle, TargetAimAngle * Mathf.Rad2Deg, 0.33F);
        transform.localRotation = Quaternion.AngleAxis(AimAngle, Vector3.up);

        // == Platforming == //

        // Apply even more gravity
        Rigidbody.AddForce(Physics.gravity * Rigidbody.mass);

        //
        RaycastHit hit;

        // Detect distance to floor
        // var aboveGround = Physics.Raycast( new Ray( Feet.transform.position, Vector3.down ), out hit );
        var aboveGround = Physics.BoxCast(Feet.transform.position + Vector3.up * 0.5F, Vector3.one * 0.15F, Vector3.down, out hit);

        // Debug.LogFormat( "{0} : {1}", hit.distance, aboveGround );

        // Animate Front Flip
        FrontFlip = Mathf.Lerp(FrontFlip, TargetFrontFlip * Mathf.Rad2Deg, 0.15F);
        Flip.transform.localRotation = Quaternion.AngleAxis(-FrontFlip, Vector3.left);

        if (FrontFlipTime > 0) FrontFlipTime -= Time.fixedDeltaTime;
        if (FrontFlipTime > -1 && FrontFlipTime <= 0)
        {
            TargetFrontFlip += Mathf.PI * 2F;
            FrontFlipTime = -1;
        }

        if (Rigidbody.velocity.y <= 0)
            IsJumping = false;

        // Am I standing on ground?
        if (aboveGround && hit.distance < 0.455F)
        {
            if (!IsOnGround)
            {
                // Debug.Log( "Ground!" );
                IsOnGround = true;
            }
        }
        else
        {
            if (IsOnGround)
            {
                // Debug.Log( "Falling!" );
                IsOnGround = false;
            }
        }

        // Rigidbody.useGravity = !IsOnGround;

        // == Character Controller == //

        // BoxCollider.enabled = !IsJumping;

        if (IsOnGround)
        {
            // Cause player to move
            Rigidbody.AddForce(motion * MovementSpeed, ForceMode.Impulse);

            // Was the jump button pressed?
            if (!IsJumping && Input.GetButton("XboxOneA"))
            {
                // Debug.Log( "Jump!" );

                Rigidbody.AddForce(Vector3.up * JumpSpeed, ForceMode.Impulse);

                // 
                if (mov.magnitude > 0.95F)
                {
                    Rigidbody.AddForce(motion.normalized * JumpSpeed * 1.25F, ForceMode.Impulse);
                    FrontFlipTime = 0.08F;
                }

                // 
                IsJumping = true;
                IsOnGround = false;
            }

            // Limit lateral velocity
            var v = Rigidbody.velocity;
            var m = new Vector2(v.x, v.z) * 0.5F;
            Rigidbody.velocity = new Vector3(m.x, v.y, m.y);
        }
        else
        {
            // Apply small force to player ( to 'air drift' )
            Rigidbody.AddForce(motion * MovementSpeed / 5F, ForceMode.Impulse);

            // Limit lateral velocity
            var v = Rigidbody.velocity;
            var m = new Vector2(v.x, v.z) * 0.93F;
            Rigidbody.velocity = new Vector3(m.x, v.y, m.y);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "drop_Cube")
        {
            var cube = col.gameObject;
            var cube_CubeBehaviour = cube.GetComponent<CubeBehaviour>();
            //var cube_Rigidbody = cube.GetComponent<Rigidbody>();
            //var cube_Collider = cube.GetComponent<Collider>();

            // Vertical displacement 
            var speed = cube_CubeBehaviour.DeltaPosition.y;

            // Detect if squashed
            var isAbove = cube.transform.position.y > transform.position.y;
            var isFalling = speed < -0.05F;

            // If above, and moving down with no "grace" player was squished!
            if (isAbove && isFalling)
            {
                //var p1 = new Vector2(Rigidbody.position.x, Rigidbody.position.z);
                //var p2 = new Vector2(cube_Rigidbody.position.x, cube_Rigidbody.position.z);
                //var distance = Vector2.Distance(p1, p2);

                // Debug.Log( distance + " / " + Collider.radius );

                //if( distance <= SphereCollider.bounds.extents.magnitude * ( 2 / 3F ) )
                //    SceneManager.LoadScene( SceneManager.GetActiveScene().name );
            }
        }
    }
}
