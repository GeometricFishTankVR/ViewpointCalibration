using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TextureAlbedoPingPong : MonoBehaviour {
    public Color AlbedoColor = Color.white;
    public float speed = 7;

    private List<Color> defaultColors;
    private Material[] materials;
    private float[] ts;
    private List<Color> targetColors;
    #region monobehaviour
    void Start()
    {
        materials = GetComponent<Renderer>().materials;
        defaultColors = new List<Color>(materials.Length);
        targetColors = new List<Color>(materials.Length);
        ts = new float[materials.Length];
        for(int i = 0; i < materials.Length; i++)
        {
            defaultColors.Add(materials[i].color);
            targetColors.Add(AlbedoColor);
            ts[i] = 0;
        }
    }
    void Update () {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = Color.Lerp(materials[i].color, targetColors[i], ts[i] * speed);
            ts[i] += Time.deltaTime;
            if (materials[i].color == targetColors[i])
            {
                ts[i] = 0;
                if (targetColors[i] == defaultColors[i])
                {
                    targetColors[i] = AlbedoColor;
                }
                else
                {
                    targetColors[i] = defaultColors[i];
                }
            }
        }
        
	}
    #endregion
}
