using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu]
public class PlanetInfo : ScriptableObject
{
    [Tooltip( "Name of the celestial body in kilograms." )]
    public string Name;

    [Tooltip( "Type of the celestial body." )]
    public string Type = "Planet";

    [Tooltip( "Radius of the celestial body in kilometers." )]
    public float Radius;

    [Tooltip( "Mass of the celestial body in kilograms." )]
    public float Mass;

    [Tooltip( "Parent celestial body this celestial body orbits." )]
    public PlanetInfo OrbitParent;

    [Tooltip( "Distance this celestial orbits its parent." )]
    public float OrbitDistance;

    public float AvgDistanceToSun
    {
        get
        {
            // The sun should be the only one without an orbit parent
            if( OrbitParent == null ) return OrbitDistance;
            else
            {
                // Is this just the same as the orbit distance added up?
                var pDist = OrbitParent.AvgDistanceToSun;
                var d1 = pDist += OrbitDistance;
                var d2 = pDist -= OrbitDistance;
                return ( d1 + d2 ) / 2.0F;
            }
        }
    }
}
