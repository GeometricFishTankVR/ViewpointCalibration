using UnityEngine;

public static class VectorUtil
// Author: Christopher Chamberlain - 2017
{
    #region Vector2 Extensions

    /// <summary>
    /// Multiplies vector A with B ( per component ).
    /// </summary>
    public static Vector2 mul( this Vector2 a, Vector2 b )
    {
        return new Vector2
        {
            x = a.x * b.x,
            y = a.y * b.y
        };
    }

    /// <summary>
    /// Divides vector A with B ( per component ).
    /// </summary>
    public static Vector2 div( this Vector2 a, Vector2 b )
    {
        return new Vector2
        {
            x = a.x / b.x,
            y = a.y / b.y
        };
    }

    /// <summary>
    /// Converts (x,y) -> (x,0,y)
    /// </summary>
    public static Vector3 AsVector3( this Vector2 @this )
    {
        return new Vector3
        {
            x = @this.x,
            y = 0,
            z = @this.y
        };
    }

    #endregion

    #region Vector3 Extensions

    /// <summary>
    /// Multiplies vector A with B ( per component ).
    /// </summary>
    public static Vector3 mul( this Vector3 a, Vector3 b )
    {
        return new Vector3
        {
            x = a.x * b.x,
            y = a.y * b.y,
            z = a.z * b.z,
        };
    }

    /// <summary>
    /// Divides vector A with B ( per component ).
    /// </summary>
    public static Vector3 div( this Vector3 a, Vector3 b )
    {
        return new Vector3
        {
            x = a.x / b.x,
            y = a.y / b.y,
            z = a.z / b.z,
        };
    }

    /// <summary>
    /// Converts (x,y,z) -> (x,z)
    /// </summary>
    public static Vector2 AsVector2( this Vector3 @this )
    {
        return new Vector2
        {
            x = @this.x,
            y = @this.z
        };
    }

    #endregion

    #region Vector4 Extensions

    /// <summary>
    /// Multiplies vector A with B ( per component ).
    /// </summary>
    public static Vector4 mul( this Vector4 a, Vector4 b )
    {
        return new Vector4
        {
            x = a.x * b.x,
            y = a.y * b.y,
            z = a.z * b.z,
            w = a.w * b.w,
        };
    }

    /// <summary>
    /// Divides vector A with B ( per component ).
    /// </summary>
    public static Vector4 div( this Vector4 a, Vector4 b )
    {
        return new Vector4
        {
            x = a.x / b.x,
            y = a.y / b.y,
            z = a.z / b.z,
            w = a.w / b.w,
        };
    }

    #endregion
}