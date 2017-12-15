using UnityEngine;
using UnityEngine.Assertions;
using System.IO;

public class SphereeFactory : MonoBehaviour {

    public Object ProjectorPrefab;
    public Transform Spheree;
    public int ScreenPixelWidth;
    public int ScreenPixelHeight;
    public int ScreenRefreshRate;
    public string CalibrationDataFolder;
    public string ConfigurationFile;
    public float SphereRadius;
    public Matrix4x4 SphereTransformation; 
    public Vector4 SphereCenter;
    public Material defaultMaterial;

    private Material sphereeMaterial;

    #region monobehaviour
    void OnEnable()
    {
        Assert.IsNotNull(ProjectorPrefab, "Projector object cannot be null. Please specify an object in the Editor.");
        Assert.IsNotNull(Spheree, "Spheree cannot be null. Please specify an object in the Editor.");
        Assert.IsTrue(ScreenPixelWidth > 0, "Screen width must be greater than 0.");
        Assert.IsTrue(ScreenPixelHeight > 0, "Screen height must be greater than 0.");
        Assert.IsTrue(ScreenRefreshRate > 0, "Screen refresh rate must be greater than 0.");
        Assert.IsTrue(SphereRadius > 0, "Sphere radius must be greater than 0.");
        Assert.IsFalse(ConfigurationFile == "", "Configuration File must be specified. Please specify in the Editor");


        SphereeMaterialController sphereeMaterialController = FindObjectOfType<SphereeMaterialController>();
        if (sphereeMaterialController != null)
        {
            sphereeMaterial = sphereeMaterialController.sphereeMaterial;
        }else
        {
            sphereeMaterial = defaultMaterial;
        }
        LoadConfiguration();
    }

    void Start()
    {
        //Shader.SetGlobalMatrix("sphere_transform", SphereTransformation);
        Shader.SetGlobalVector("sphere_center", SphereCenter);
    }
    #endregion

    public void LoadConfiguration()
    {
        string calibrationDataPath = Path.GetFullPath(CalibrationDataFolder);
        string parametersFilePath = Path.GetFullPath(Path.Combine(calibrationDataPath, ConfigurationFile));
        string[] binFiles = Directory.GetFiles(calibrationDataPath, "pro?pixel_.bin");
        int numberOfProjectors = binFiles.Length;
        Parameters sphereeParameters = new Parameters(numberOfProjectors, parametersFilePath);
        //UpdateGameObjects(sphereeParameters);
        UpdateGameObjects(sphereeParameters.NumberOfProjectors);
    }

    
    /// <summary>
    /// Creates projectors from normalized calibration data: sphere-centered coordinates with radius of 1
    /// </summary>
    void UpdateGameObjects(int numberOfProjectors)
    {
        SphereTransformation = Matrix4x4.identity;
        SphereCenter = new Vector4(0,0,0,1);

        Transform projectorsTransform = transform.FindChild("Projectors");
        if(projectorsTransform != null)
        {
            foreach (Transform child in projectorsTransform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        GameObject projectors = projectorsTransform.gameObject;
        projectors.transform.parent = transform;
        projectors.transform.localScale = Vector3.one;
        projectors.transform.localPosition = Vector3.zero;
        projectors.transform.localRotation = Quaternion.identity;

        // Loop through each projector 
        for (int i = 0; i < numberOfProjectors; i++)
        {
            // Instantiate projectrs side by side
            GameObject projector = (GameObject)Instantiate(ProjectorPrefab, Vector3.zero, Quaternion.identity);
            projector.transform.parent = projectors.transform;
            projector.transform.localScale = Vector3.one;
            projector.transform.localPosition = new Vector3(i, 0, 0);
            projector.name = "Projector " + (i + 1).ToString();
            
            // Set the projection matrix of the projector 
            Camera projectorCamera = projector.GetComponent<Camera>();
            projectorCamera.orthographic = true;
            projectorCamera.targetDisplay = i + 1;

            // Set main sample texture for each projector (each pixel is 3D location + alpha)
            GameObject quad = projector.transform.Find("Quad").gameObject;
            Quad_Controller qc = quad.GetComponent<Quad_Controller>();
            qc.CalibrationDataFolder = CalibrationDataFolder;
            qc.ProjectorNumber = i + 1;
            qc.ScreenPixelHeight = ScreenPixelHeight;
            qc.ScreenPixelWidth = ScreenPixelWidth;
            qc.renderMaterial = sphereeMaterial;
            quad.SetActive(true);
            qc.LoadSampleTexture();
            

            SphereeProjector projectorScript = projector.GetComponent<SphereeProjector>();
            projectorScript.ScreenPixelHeight = ScreenPixelHeight;
            projectorScript.ScreenPixelWidth = ScreenPixelWidth;
            projectorScript.ScreenRefreshRate = ScreenRefreshRate;
            projectorScript.Spheree = GetComponent<Transform>(); ;
        }
 
    }


   
}
