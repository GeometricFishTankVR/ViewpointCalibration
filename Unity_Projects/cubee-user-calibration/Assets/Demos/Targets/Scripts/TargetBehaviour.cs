using Biglab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehaviour : MonoBehaviour
{
    [ReadOnly]
    public TargetState State;

    public float BirthTime = 0.5F;

    public float BreakTime = 0.5F;

    public float LifeTime = 2F;

    public GameObject SparksPrefab;

    public CsvTableWriter CsvWriter;

    // 
    private Coroutine StateCoroutine;

    // 
    private MeshRenderer Renderer;
    private Material Material { get { return Renderer.material; } }

    void Start()
    {
        // 
        Renderer = GetComponent<MeshRenderer>();

        // Clone material
        Renderer.material = new Material( Renderer.material );

        //
        GotoState( TargetState.Birth );
    }

    private void GotoState( TargetState state )
    {
        State = state;
        // Debug.LogFormat( "Going to state: {0}", State );

        // Stop previous routine
        if( StateCoroutine != null )
            StopCoroutine( StateCoroutine );

        //
        Func<IEnumerator> coroutine = null;
        switch( state )
        {
            case TargetState.Birth:
                coroutine = BirthState;
                break;

            case TargetState.Normal:
                coroutine = NormalState;
                break;

            case TargetState.Break:
                coroutine = BreakState;
                break;
        }

        // Begin new routine
        Debug.Assert( coroutine != null );
        StateCoroutine = StartCoroutine( coroutine() );
    }

    #region State Coroutines

    private IEnumerator BirthState()
    {
        var baseRotation = transform.localRotation;

        var time = 0F;
        while( time < BirthTime )
        {
            var p = time / BirthTime;

            // Fade in
            var color = Renderer.material.color;
            color.a = Easings.CircularEaseInOut( p );
            Renderer.material.color = color;

            // Spin
            var angle = Easings.CubicEaseOut( p ) * 180F * 2F;
            var r = Quaternion.AngleAxis( angle, Vector3.right );
            transform.localRotation = baseRotation * r;
            transform.localScale = Vector3.one * Easings.CircularEaseInOut( p );

            // 
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //
        transform.localRotation = baseRotation;
        Renderer.material.color = Color.white;

        // Change state
        GotoState( TargetState.Normal );
    }

    private IEnumerator NormalState()
    {
        var time = 0F;

        while( time < LifeTime )
        {
            // 
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Darken, to show waste
        Material.color = new Color( 0.33F, 0.33F, 0.33F );

        // Lost the target.
        // CsvWriter.SetField( "EventType", HitType.LostTarget );

        // Switch to the break animation
        GotoState( TargetState.Break );
    }

    private IEnumerator BreakState()
    {
        var baseRotation = transform.localRotation;

        // Disable collider
        var collider = GetComponent<Collider>();
        collider.enabled = false; // 

        var time = 0F;
        while( time < BreakTime )
        {
            var p = 1F - ( time / BreakTime );

            // Fade in
            var color = Renderer.material.color;
            color.a = Easings.CircularEaseInOut( p );
            Renderer.material.color = color;

            // Spin
            var angle = Easings.QuarticEaseIn( p ) * 180F * 1F;
            var r = Quaternion.AngleAxis( angle, Vector3.right );
            transform.localRotation = baseRotation * r;

            // 
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Kill this game object
        Destroy( gameObject );
    }

    #endregion

    public bool DetectHit( Ray ray, RaycastHit info )
    {
        // Not already in the break state
        if( State != TargetState.Break )
        {
            // If facing the right way
            if( Vector3.Dot( ray.direction, transform.forward ) <= 0 )
            {
                // Make Sparks
                var sparks = Instantiate( SparksPrefab );
                sparks.transform.localScale = transform.lossyScale * 2F;
                sparks.transform.rotation = Quaternion.LookRotation( info.normal );
                sparks.transform.position = info.point;

                // 
                GotoState( TargetState.Break );
                return true;
            }
        }

        // 
        return false;
    }

    public enum TargetState
    {
        Birth,
        Normal,
        Break
    }
}
