using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.IO;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class Quad_Controller : MonoBehaviour {
    public string CalibrationDataFolder;
    public int ProjectorNumber;
    public int ScreenPixelWidth;
    public int ScreenPixelHeight;
    public Material renderMaterial;

    #region monobehaviour
    void Start()
    {
        LoadSampleTexture();
    }
    #endregion

    // Set projector specific information as main texture for wach material
    public void LoadSampleTexture()
    {
        Renderer thisRenderer = GetComponent<Renderer>();
        Material mat = new Material(renderMaterial);
        thisRenderer.material = mat;
        mat.mainTexture = MakeProjectorTextures();
    }

    /// <summary>
    /// Creates a texture from the binary file in the CalibrationDataFolder folder
    /// Created texture is the same size as the binary file and format RGBAFloat
    /// </summary>
    /// <param name="projectorNumber"> The projector number</param>
    /// <returns> The name of the 4 channel texture</returns>
    Texture2D MakeProjectorTextures()
    {
        string calibrationDataPath = Path.GetFullPath(CalibrationDataFolder);
        Texture2D projectorTexture;

        // Read in binary files
        //byte[] pos = File.ReadAllBytes(Application.dataPath + "/" + "../CalibrationFiles/pro" + (proj_num + 1).ToString() + "pixel_.bin");
        //byte[] alpha = File.ReadAllBytes(Application.dataPath + "/" + "../CalibrationFiles/pro" + (proj_num + 1).ToString() + "pixela_.bin");
        string projectorName = "pro" + ProjectorNumber.ToString();
        string pixelPositionFileName = projectorName + "pixel_.bin";
        string pixelPositionFilePath = Path.GetFullPath(Path.Combine(calibrationDataPath, pixelPositionFileName));
        string pixelAlphaFileName = projectorName + "pixela_.bin";
        string pixelAlphaFilePath = Path.GetFullPath(Path.Combine(calibrationDataPath, pixelAlphaFileName));

        byte[] pos = File.ReadAllBytes(pixelPositionFilePath);
        byte[] alpha = File.ReadAllBytes(pixelAlphaFilePath);
        byte[] tex_bytes = new byte[pos.Length + alpha.Length];

        // Merge the two byte arrays using by taking 12 bytes (3 floats) from the position information for every 4 bytes (1 float) of the alpha information
        // Resulting byte array represents a 4 channel byte array where rgb represent xyz respectivley
        // QZ: swap y and z to fit in Unity coordinates
        for (int j = 0; j < tex_bytes.Length / 16; j++)
        {
            for (int k = 0; k < 4; k++) //x->x
            {
                tex_bytes[j * 16 + k] = pos[j * 12 + k];
            }
            for (int k = 4; k < 8; k++) //y->z
            {
                tex_bytes[j * 16 + 4 + k] = pos[j * 12 + k];
            }
            for (int k = 8; k < 12; k++)//z->y
            {
                tex_bytes[j * 16 - 4 + k] = pos[j * 12 + k];
            }
            for (int m = 0; m < 4; m++)//a->a
            {
                tex_bytes[j * 16 + 12 + m] = alpha[j * 4 + m];

            }
        }

        // Create texture out of byte array
        projectorTexture = new Texture2D(ScreenPixelWidth, ScreenPixelHeight, TextureFormat.RGBAFloat, false);
        projectorTexture.LoadRawTextureData(tex_bytes);
        projectorTexture.Apply();

        // Save to PNG (for debuging)
        byte[] projectorTexturePNG = projectorTexture.EncodeToPNG();
        string projectorTextureFileName = projectorName + "pixel_.png";
        string projectorTextureFilePath = Path.GetFullPath(Path.Combine(calibrationDataPath, projectorTextureFileName));
        File.WriteAllBytes(projectorTextureFilePath, projectorTexturePNG);

        return projectorTexture;
    }
}
