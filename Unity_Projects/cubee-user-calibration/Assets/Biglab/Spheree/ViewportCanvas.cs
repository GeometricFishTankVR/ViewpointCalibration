using Biglab;
using UnityEngine;

public class ViewportCanvas : MonoBehaviour
{ 
    public PlayerManager PlayerManager;

    [Tooltip( "The viewport ( ie, spheree ) transform." )]
    public Transform Viewport;

    private Transform Transform;

    private void Start()
    {
        Transform = GetComponent<Transform>();
    }

    void LateUpdate()
    {
        // 
        var player = PlayerManager.GetPlayerOneTranform();
        var dir = Vector3.Normalize( player.position - Viewport.position );

        // Place tangent plane at radius
        Transform.position = Viewport.position + ( dir * Viewport.lossyScale.x * 0.5F ) * ( 4F / 5F );
        Transform.rotation = Quaternion.LookRotation( -dir );
    }
}
