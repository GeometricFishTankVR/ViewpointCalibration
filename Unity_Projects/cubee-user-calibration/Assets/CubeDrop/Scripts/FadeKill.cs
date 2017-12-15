using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeKill : MonoBehaviour
{
    public float TimeToLive;
    public float TimeVariance;

    private float _LifeTime;
    private MeshRenderer MeshRenderer;

    void Start()
    {
        // 
        MeshRenderer = GetComponent<MeshRenderer>();

        // 
        _LifeTime = TimeToLive + Random.Range( 0, TimeVariance );
    }

    void Update()
    {
        _LifeTime -= Time.smoothDeltaTime;

        // Death
        if( _LifeTime < 0 ) Destroy( gameObject );
        // Fade
        else if( MeshRenderer != null )
        {
            foreach( var m in MeshRenderer.materials )
            {
                var c = m.color;
                c.a = _LifeTime / TimeToLive;
                m.color = c;
            }
        }
    }
}
