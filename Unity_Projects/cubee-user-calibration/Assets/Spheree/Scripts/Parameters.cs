using UnityEngine;
using UnityEngine.Assertions;
using System.Xml;

public class Parameters{
    public int NumberOfProjectors;
    public float Radius = 1;
    public Vector3 SpherePosition = new Vector3(0, 0, 0);

    /// <summary>
    /// Constructor, initializes all fields to zeros
    /// </summary>
    /// <param name="numberOfProjectors">Number of projectors in system</param>
    public Parameters(int numberOfProjectors, string path)
    {
        Assert.IsTrue(NumberOfProjectors >= 0, "Number of projectors cannot be negative.");
        Assert.IsTrue(Radius > 0, "Radius must be greater than 0.");
        NumberOfProjectors = numberOfProjectors;
        ReadSphereInfo(path);
    }
    

    void ReadSphereInfo(string path)
    {
        XmlReaderSettings settings = new XmlReaderSettings();
        XmlReader reader = XmlReader.Create(path, settings);
        reader.ReadToFollowing("sphere_pose");
        XmlReader subReader = reader.ReadSubtree();
        subReader.ReadToFollowing("data");
        string dataString = subReader.ReadElementContentAsString().Trim().Replace("\n", "").Replace("\r", "");
        int temp_len = 0;
        while(temp_len != dataString.Length)
        {
            temp_len = dataString.Length;
            dataString = dataString.Replace("  ", " ");
        }
        string[] stringArray = dataString.Split(' ');
        SpherePosition = new Vector3(float.Parse(stringArray[0]), float.Parse(stringArray[1]), float.Parse(stringArray[2]));
        Radius = float.Parse(stringArray[3]);
    }
}
