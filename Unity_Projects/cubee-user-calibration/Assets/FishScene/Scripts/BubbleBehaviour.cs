using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleBehaviour : MonoBehaviour
{
    private SphereCollider Collider;

    [Tooltip( "Base speed for how quickly the bubble rises." )]
    public float FloatingSpeed = 1F;

    [Range( 0F, 1F )]
    [Tooltip( "Random variance on the floating speed." )]
    public float FloatingSpeedVariance = 0.1F;

    [Range( 0F, 1F )]
    [Tooltip( "Scale of the bubble." )]
    public float Scale = 0.8F;

    [Range( 0F, 1F )]
    [Tooltip( "Random variance on the scale of the bubble." )]
    public float ScaleVariance = 0.1F;

    private float _TargetScale;
    private float _CurrentScale;
    private float _Speed;

    void Start()
    {
        Collider = GetComponent<SphereCollider>();
        Collider.enabled = false;
        
        // 
        _CurrentScale = 0F;
        _TargetScale = GetRandomScale();
        _Speed = GetRandomSpeed();

        // 
        Invoke( "EnableCollider", 2F );
    }

    private float GetRandomSpeed()
    {
        var r = Random.Range( 0F, FloatingSpeedVariance );
        return FloatingSpeed * ( 1F - r );
    }

    private float GetRandomScale()
    {
        var r = Random.Range( 0F, ScaleVariance );
        return Scale * ( 1F - r );
    }

    void Update()
    {
        // Rise
        var pos = transform.position;
        pos.y += ( _Speed * 0.03F ) * ( 1F - Mathf.Pow( 1F - transform.localScale.x, 2F ) );
        transform.position = pos;

        // Computes a wobble
        var wx = Mathf.Sin( Time.time * _TargetScale * 3F );
        var wy = Mathf.Cos( Time.time * 9F );
        var wobble = new Vector3( 1F + wx * 0.1F, 1F + wx * 0.1F, 1F );

        transform.localScale = wobble * _CurrentScale;
        _CurrentScale = Mathf.Lerp( _CurrentScale, _TargetScale, 0.01F );

        // Pop...?
        if( transform.position.magnitude > 6F ) // 6 is a magic number for the clipping radius
            Destroy( gameObject );
    }

    void EnableCollider()
    {
        Collider.enabled = true;
    }

    void OnTriggerEnter( Collider other )
    {
        // Pop!
        // Debug.Log( other.name );
        // Destroy( gameObject );
    }
}
