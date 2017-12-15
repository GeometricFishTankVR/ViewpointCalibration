using UnityEditor;
using UnityEngine;

[CustomEditor( typeof( TargetRandomizer ) )]
class TargetRandomizerEditor : Editor
{
    private void OnSceneGUI()
    {
        var obj = target as TargetRandomizer;
        if( obj == null ) return;

        //
        DebugDrawing.DrawArcCage( obj.transform.position, obj.SpawnRadius, obj.SpawnAngleRange.min, obj.SpawnAngleRange.max, Color.red );
    }
}
