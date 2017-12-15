using UnityEngine;

// Singleton desgin pattern
public class PersistentProjectStorage  {
    public Object SelectedHeadTracker;
    public bool IsHeadTracker6DoF;
    public float HeadTrackerToDisplayScaleFactor;
    public ViewpointCalibration CurrentViewpointCalibration;

    private PersistentProjectStorage() { }

    private static PersistentProjectStorage instance;
    public static PersistentProjectStorage Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new PersistentProjectStorage();
            }
            return instance;
        }

    }
}
