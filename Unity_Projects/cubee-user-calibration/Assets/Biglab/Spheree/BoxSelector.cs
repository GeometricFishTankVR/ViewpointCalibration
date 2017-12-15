using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof( Image ) )]
public class BoxSelector : MonoBehaviour
{
    private Image Image;

    private Canvas Canvas;

    [Tooltip( "A transform representing where the viewer/eye is." )]
    public Transform Viewpoint;

    [SerializeField, Tooltip( "The renderer the selector will highlight. May be null." )]
    private MeshRenderer SelectedRenderer;

    void Start()
    {
        Canvas = GetComponentInParent<Canvas>();
        Image = GetComponent<Image>();
    }

    void Update()
    {
        // Disable image if no renderer is given
        Image.enabled = SelectedRenderer != null;

        // If we have a renderer and viewpoint specified
        if( SelectedRenderer != null )
        {
            // Get canvas plane information 
            var plane = new Plane( Canvas.transform.forward, Canvas.transform.position );

            // Find min, max points within canvas space
            var min = Vector3.one * float.MaxValue;
            var max = Vector3.one * float.MinValue;
            foreach( var corner in GetLocalCornerPoints( SelectedRenderer ) )
            {
                // Transform each local aabb point into an obb point
                var intersect = RaycastPlane( SelectedRenderer.transform.TransformPoint( corner ), ref plane );
                var canvasPoint = Canvas.transform.InverseTransformPoint( intersect ); // Transform to canvas space

                // Find extreme canvas space points
                min = Vector3.Min( min, canvasPoint );
                max = Vector3.Max( max, canvasPoint );
            }

            // Move box to fit min-max extreme canvas points
            Image.rectTransform.sizeDelta = max - min;
            Image.rectTransform.localPosition = ( max + min ) * 0.5F;
        }
    }

    /// <summary>
    /// Gets the selected game object.
    /// </summary>
    public GameObject GetSelectedObject()
    {
        if( SelectedRenderer == null ) return null;
        else return SelectedRenderer.gameObject;
    }

    /// <summary>
    /// Selects a game object. 
    /// A null value will hide selection.
    /// </summary>
    public void SelectObject( GameObject obj )
    {
        if( obj == null )
        {
            Image.CrossFadeAlpha( 0F, 0.2F, false );
            SelectedRenderer = null;
        }
        else
        {
            var renderer = obj.GetComponent<MeshRenderer>();
            if( SelectedRenderer != renderer )
            {
                SelectedRenderer = renderer;
                Image.CrossFadeAlpha( 0F, 0F, false ); // Force no alpha
                Image.CrossFadeAlpha( 1F, 0.2F, false ); // Fade in
            }
        }
    }

    #region Raycasting

    IEnumerable<Vector3> GetLocalCornerPoints( MeshRenderer renderer )
    {
        var mesh = renderer.GetComponent<MeshFilter>();

        var b = mesh.mesh.bounds;
        var e = b.extents;

        yield return b.center + new Vector3( +e.x, +e.y, +e.z );
        yield return b.center + new Vector3( -e.x, +e.y, +e.z );
        yield return b.center + new Vector3( -e.x, -e.y, +e.z );
        yield return b.center + new Vector3( +e.x, -e.y, +e.z );
        yield return b.center + new Vector3( +e.x, +e.y, -e.z );
        yield return b.center + new Vector3( -e.x, +e.y, -e.z );
        yield return b.center + new Vector3( -e.x, -e.y, -e.z );
        yield return b.center + new Vector3( +e.x, -e.y, -e.z );
    }

    Vector3 RaycastPlane( Vector3 position, ref Plane plane )
    {
        // Normalized vector between view and point
        var dir = ( GetViewpointPosition() - position ).normalized;

        float dist;
        var ray = new Ray( position, dir );
        if( plane.Raycast( ray, out dist ) )
            return ray.GetPoint( dist );

        return position;
    }

    Vector3 GetViewpointPosition()
    {
        if( Viewpoint == null ) return Camera.main.transform.position;
        else return Viewpoint.position;
    }

    #endregion
}
