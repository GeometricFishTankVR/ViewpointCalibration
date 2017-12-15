using UnityEngine;

namespace Biglab
{
    public class WebCamMaterial : MonoBehaviour
    // Author: Christopher Chamberlain - 2017
    /*
     * Not elegant enough, 
     * OnWebCamTextureUpdate needs to be rigged through WebCamSource.OnTextureUpdate in the inspector.
     */
    {
        [Tooltip( "The material to update with webcam information." )]
        public Material Material;

        [Tooltip( "Upon update, does this component update the materials main texture?" )]
        public bool UpdateMaterialAlbedo = true;

        [Tooltip( "Upon update, does this component update the materials emissive texture?" )]
        public bool UpdateMaterialEmissive = true;

        public void OnWebCamTextureUpdate( WebCamTexture texture )
        {
            // 
            if( UpdateMaterialAlbedo ) Material.SetTexture( "_MainTex", texture );
            else Material.SetTexture( "_MainTex", null );

            // 
            if( UpdateMaterialEmissive ) Material.SetTexture( "_EmissionMap", texture );
            else Material.SetTexture( "_EmissionMap", null );
        }
    }
}