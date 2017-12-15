using UnityEngine;

using Object = UnityEngine.Object;

namespace Biglab
{
    public static class RenderCube
    // Author: Christopher Chamberlain - 2017
    {
        /// <summary>
        /// Creates a cubic render texture.
        /// </summary> 
        public static Cubemap CreateTexture( int size )
        {
            return new Cubemap( size, TextureFormat.ARGB32, true );
        }

        /// <summary>
        /// Renders what the world looks like at the given position to the specified cubemap.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="cubemap"></param>
        public static void Render( Vector3 position, Cubemap cubemap )
        {
            var cameraObject = new GameObject( "Cubemap Camera" );
            var camera = cameraObject.AddComponent<Camera>();
            camera.transform.position = position;
            camera.clearFlags = CameraClearFlags.Color | CameraClearFlags.Depth;
            camera.cameraType = CameraType.Game;
            camera.nearClipPlane = 0.001F;
            camera.farClipPlane = 2F;
            camera.fieldOfView = 90;

            // 
            camera.RenderToCubemap( cubemap );

            // 
            Object.Destroy( cameraObject );

        }
    }
}