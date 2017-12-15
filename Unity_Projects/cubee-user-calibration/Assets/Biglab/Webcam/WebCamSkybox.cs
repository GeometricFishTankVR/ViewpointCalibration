using UnityEngine;

namespace Biglab
{
    public class WebCamSkybox : MonoBehaviour
    // Author: Christopher Chamberlain - 2017
    {
        private static readonly Vector3 POSITION = new Vector3( 10000, 0, 0 );

        private Cubemap SkyboxTexture;
        private Material SkyboxMaterial;

        // 
        private GameObject ArtificialSkyboxSphere;
        private Material ArtificialSkyboxMaterial;

        private void Awake()
        {
            // 
            SkyboxMaterial = new Material( Shader.Find( "Skybox/Cubemap" ) );
            ArtificialSkyboxMaterial = new Material( Shader.Find( "Sprites/Default" ) );

            // Create skybox texture
            SkyboxTexture = RenderCube.CreateTexture( 128 );
            SkyboxMaterial.SetTexture( "_Tex", SkyboxTexture );

            // Position a sphere wrapped with webcam texture
            ArtificialSkyboxSphere = GameObject.CreatePrimitive( PrimitiveType.Sphere );
            ArtificialSkyboxSphere.transform.hideFlags = HideFlags.HideInHierarchy;
            ArtificialSkyboxSphere.transform.position = POSITION;
            ArtificialSkyboxSphere.GetComponent<MeshRenderer>().material = ArtificialSkyboxMaterial;
            ArtificialSkyboxSphere.name = "Artificial Skybox";
        }

        private void OnDestroy()
        {
            Destroy( ArtificialSkyboxSphere );
            Destroy( SkyboxMaterial );
            Destroy( SkyboxTexture );
        }

        public void OnWebCamTextureUpdate( WebCamTexture texture )
        {
            //
            ArtificialSkyboxMaterial.SetTexture( "_MainTex", texture );

            //
            RenderSettings.skybox = SkyboxMaterial;
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
            RenderSettings.customReflection = SkyboxTexture;
            DynamicGI.UpdateEnvironment();

            // 
            //SkyboxTexture.DiscardContents();
            RenderCube.Render( POSITION, SkyboxTexture );
        }
    }
}