using System.Collections;
using UnityEngine;

/// <summary>
/// A tool to maniupulate the spheree viewport ( probably the cubee too ).
/// </summary>
public class ViewportManipulator : MonoBehaviour
// Author: Christopher Chamberlain - 2017
{
    [Tooltip( "The viewport object being manipulated" )]
    public Transform Viewport;

    private Coroutine zoomRoutine;

    /// <summary>
    /// Animates the viewport to the zoomed coordinate and scale via a coroutine.
    /// </summary>
    public void ZoomTo( Vector3 targetPosition, Vector3 targetScale, float duration )
    {
        // Stop previous
        if( zoomRoutine != null )
            StopCoroutine( zoomRoutine );

        // Begin new
        zoomRoutine = StartCoroutine( ZoomToCoroutine( targetPosition, targetScale, duration ) );
    }

    /// <summary>
    /// Jums the viewport to the zoomed coordinate and scale.
    /// </summary>
    public void MoveTo( Vector3 targetPosition, Vector3 targetScale, float duration )
    {
        // Stop previous
        if( zoomRoutine != null )
            StopCoroutine( zoomRoutine );

        // Jump to
        Viewport.localScale = targetScale;
        Viewport.position = targetPosition;
    }

    private IEnumerator ZoomToCoroutine( Vector3 targetPosition, Vector3 targetScale, float duration )
    {
        var initialScale = Viewport.localScale;
        var initialPosition = Viewport.position;

        float time = 0;
        while( time < duration )
        {
            time += Time.deltaTime;

            var factor = time / duration;

            // Interpolate position
            Viewport.localScale = Vector3.Lerp( initialScale, targetScale, factor );
            Viewport.position = Vector3.Lerp( initialPosition, targetPosition, factor );

            yield return new WaitForEndOfFrame();
        }

        zoomRoutine = null;
    }
}