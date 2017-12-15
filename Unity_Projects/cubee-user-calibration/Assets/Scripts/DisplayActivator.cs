using UnityEngine;

public class DisplayActivator : MonoBehaviour {
    public int NumberOfExtraDisplays;
    public int ScreenPixelWidth;
    public int ScreenPixelHeight;
    public int ScreenRefreshRate;

	// Use this for initialization
	void Start () {
        for(int displayIdx = 1; displayIdx < Display.displays.Length; displayIdx++)
        {
            if (displayIdx <= NumberOfExtraDisplays)
            {
                Display.displays[displayIdx].Activate(
                    ScreenPixelWidth, 
                    ScreenPixelHeight, 
                    ScreenRefreshRate);
            }
            else
            {
                Debug.Log("Not enough displays to activate.");
            }
        }
        
    }
}
