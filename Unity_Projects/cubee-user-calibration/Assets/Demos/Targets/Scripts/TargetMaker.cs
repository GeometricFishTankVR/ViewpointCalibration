using UnityEngine;

/// <summary>
/// A component with the ability to spawn targets.
/// </summary>
public class TargetMaker : MonoBehaviour
{
    [Tooltip( "Prefab to spawn as the targets." )]
    public GameObject TargetPrefab;

    /// <summary>
    /// Instantiates a target at the given position and looking along the given direction.
    /// </summary>
    public GameObject SpawnTarget( Vector3 position, Vector3 direction )
    {
        return SpawnTarget( position, Quaternion.LookRotation( direction ) );
    }

    /// <summary>
    /// Instantiates a target at the given position and orientation.
    /// </summary>
    public GameObject SpawnTarget( Vector3 position, Quaternion rotation )
    {
        // Create the target
        var target = Instantiate( TargetPrefab, transform );

        // Position and orient the target
        var target_Transform = target.GetComponent<Transform>();
        target_Transform.rotation = rotation;
        target_Transform.position = position; 

        // 
        return target;
    }
}
