using UnityEngine;
using UnityEditor;

/// <summary>
/// Provides debug util.
/// </summary>
public static class DebugDrawing
{
    /// <summary>
    /// Using <see cref="Handles"/>, draws a wireframe hemisphere aligned to the <see cref="Vector3.up"/> axis.
    /// </summary>
    public static void DrawHemisphere( Vector3 center, float radius, Color color )
    {
        Handles.color = color;

        // 'Cage'
        for( float a = 0; a < 360; a += 60 )
        {
            var vec = Quaternion.AngleAxis( a, Vector3.up ) * Vector3.forward;
            Handles.DrawWireArc( Vector3.zero, vec, Vector3.up, 90, radius );
        }

        // 'Floor'
        Handles.DrawWireArc( Vector3.zero, Vector3.up, Vector3.forward, 360, radius );
    }

    /// <summary>
    /// Using <see cref="Handles"/>, draws a wireframe sphere aligned to the <see cref="Vector3.up"/> axis.
    /// </summary>
    public static void DrawSphere( Vector3 center, float radius, Color color )
    {
        Handles.color = color;

        // 'Cage'
        for( float a = 0; a < 360; a += 60 )
        {
            var vec = Quaternion.AngleAxis( a, Vector3.up ) * Vector3.forward;
            Handles.DrawWireArc( Vector3.zero, vec, Vector3.up, 180, radius );
        }

        // 'Floor'
        Handles.DrawWireArc( Vector3.zero, Vector3.up, Vector3.forward, 360, radius );
    }

    /// <summary>
    /// Using <see cref="Handles"/>, draws a wireframe 'arc cage' aligned to the <see cref="Vector3.up"/> axis.
    /// </summary>
    public static void DrawArcCage( Vector3 center, float radius, float startAngle, float endAngle, Color color )
    {
        Handles.color = color;

        var midAngle = ( endAngle + startAngle ) / 2F;

        var topRot = Quaternion.AngleAxis( startAngle, Vector3.forward );
        var botRot = Quaternion.AngleAxis( endAngle, Vector3.forward );
        var midRot = Quaternion.AngleAxis( midAngle, Vector3.forward );

        // 'Cage Bars' 
        for( float a = 0; a < 360; a += ( 360F / 8 ) )
        {
            // 
            var spin = Quaternion.AngleAxis( a, Vector3.up );

            var nor = spin * Vector3.forward;
            var vec = spin * topRot * Vector3.up;

            Handles.DrawWireArc( center + Vector3.zero, nor, vec, endAngle - startAngle, radius );
        }

        // 'Cage Caps'
        var t = topRot * Vector3.up * radius;
        var b = botRot * Vector3.up * radius;
        var m = midRot * Vector3.up * radius;
        Handles.DrawWireArc( center + Vector3.up * t.y, Vector3.up, Vector3.forward, 360, radius * Mathf.Sin( startAngle * Mathf.Deg2Rad ) );
        Handles.DrawWireArc( center + Vector3.up * b.y, Vector3.up, Vector3.forward, 360, radius * Mathf.Sin( endAngle * Mathf.Deg2Rad ) );
        Handles.DrawWireArc( center + Vector3.up * m.y, Vector3.up, Vector3.forward, 360, radius * Mathf.Sin( midAngle * Mathf.Deg2Rad ) );
    }

    /// <summary>
    /// Using <see cref="Handles"/>, draws a wireframe box aligned to the <see cref="Vector3.up"/> axis.
    /// </summary>
    public static void DrawWireBox( Vector3 min, Vector3 max, Color color )
    {
        var offset = Vector3.one * 0.5F;

        Handles.color = color;

        var c = ( min + max ) / 2F;
        Handles.DrawWireCube( c - offset, max - min );
    }
}