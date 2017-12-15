using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TetrisPiece : MonoBehaviour
{
    public int Distribution = 10;

    private Transform[] Dots; 

    public IEnumerable<Transform> GetChildren()
    {
        if( Dots == null )
        {
            // Finds all dots
            var renderers = GetComponentsInChildren<MeshRenderer>();
            Dots = renderers.Select( x => x.transform ).ToArray();
        }

        return Dots;
    }

    public TetrisPiece CreateInstance( Transform parent )
    {
        var obj = Instantiate( gameObject, parent );
        return obj.GetComponent<TetrisPiece>();
    }
}
