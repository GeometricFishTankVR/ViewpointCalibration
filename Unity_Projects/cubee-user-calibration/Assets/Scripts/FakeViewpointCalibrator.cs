using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FakeViewpointCalibrator : MonoBehaviour
{
    public int NumberOfSamples;
    public float MinimumCalibrationDistance;
    public float MaximumCalibrationDistance;
    public Transform DisplayCenter;
    public Transform Display;
    public GameObject Pattern;
    public Transform ViewInDisplay;
    public int RandomSeed = 4;

    private List<Vector3> GeneratedCalibrationPositions;
    private Vector3 currentCalibrationPosition;

    #region monobehaviour
    void OnEnable()
    {
        Assert.IsNotNull(DisplayCenter, "Display origin cannot be null. Please specify in the Editor.");
        Assert.IsNotNull(Pattern, "Pattern cannot be null. Please specify in the Editor.");
        Assert.IsNotNull(ViewInDisplay, "View cannot be null. Please specify in the Editor.");
        Assert.IsTrue(NumberOfSamples > 3, "The number of samples must be greater than 3.");
        Assert.IsTrue(MinimumCalibrationDistance > 0, "The minimum calibration distance must be greater than 0.");
        Assert.IsTrue(MaximumCalibrationDistance > 0, "The maximum calibration distance must be greater than 0.");
        Assert.IsTrue(MaximumCalibrationDistance > MinimumCalibrationDistance, "The maximum calibration distance must be greater than the minimum calibration distance.");
    }

    void Start()
    {
        GeneratedCalibrationPositions = new List<Vector3> { Vector3.down };
        currentCalibrationPosition = GeneratedCalibrationPositions[0];
        Pattern.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonUp("XboxOneMenu(Start)"))
        {
            StartCalibration();
        }
        if (Input.GetButtonUp("XboxOneA"))
        {
            RecordCurrentAndGoToNext();
        }
        else if (Input.GetButtonUp("XboxOneB"))
        {
            DeleteCurrentAndGoToPrev();
        }
        if (Pattern.activeSelf)
        {
            Pattern.transform.rotation = Quaternion.LookRotation(-currentCalibrationPosition);
            Pattern.transform.position = DisplayCenter.position;
            ViewInDisplay.localPosition = Vector3.Lerp(ViewInDisplay.localPosition, currentCalibrationPosition, Time.deltaTime * 20);
        }
    }
    #endregion

    public void StartCalibration()
    {
        Pattern.SetActive(true);
        GeneratedCalibrationPositions = GenerateCalibrationPositions(RandomSeed);
        GeneratedCalibrationPositions.Add(Vector3.down);
        currentCalibrationPosition = GeneratedCalibrationPositions[0];
    }

    public Vector3 NextCalibrationPosition(Vector3 current, out bool moved)
    {
        int index = GeneratedCalibrationPositions.IndexOf(current);

        if (index < GeneratedCalibrationPositions.Count - 1)
        {
            moved = true;
            return GeneratedCalibrationPositions[index + 1];
        }
        else
        {
            moved = false;
            return current;
        }
    }

    public Vector3 PreviousCalibrationPosition(Vector3 current, out bool moved)
    {
        int index = GeneratedCalibrationPositions.IndexOf(current);

        if (index >= 1)
        {
            moved = true;
            return GeneratedCalibrationPositions[index - 1];
        }
        else
        {
            moved = false;
            return current;
        }
    }

    public void RecordCurrentAndGoToNext()
    {
        bool moved;
        Vector3 nextCalibrationPosition = NextCalibrationPosition(currentCalibrationPosition, out moved);
        if (moved)
        {
            currentCalibrationPosition = nextCalibrationPosition;
        }
    }

    public void DeleteCurrentAndGoToPrev()
    {
        bool moved;
        Vector3 prevCalibrationPosition = PreviousCalibrationPosition(currentCalibrationPosition, out moved);
        if (moved)
        {
            currentCalibrationPosition = prevCalibrationPosition;
        }
    }

    public List<Vector3> GenerateCalibrationPositions(int seed)
    {
        Random.InitState(seed);

        List<float> theta = new List<float>(NumberOfSamples);
        List<float> phi = new List<float>(NumberOfSamples);
        List<float> radius = new List<float>(NumberOfSamples);

        int samplesPerQuadrant = NumberOfSamples / 4;
        for (int i = 0; i < NumberOfSamples; i++)
        {
            int quadrant = i / samplesPerQuadrant;
            // Generate restricted random spherical coordinates
            theta.Add((((i % samplesPerQuadrant) + 1.0f) / samplesPerQuadrant) * 0.4f + 0.4f + (quadrant * Mathf.PI / 2));
            phi.Add(Random.Range(Mathf.PI / 2.5f, Mathf.PI / 6));
            radius.Add(Random.Range(MinimumCalibrationDistance, MaximumCalibrationDistance));
        }

        // Sort theta coordinate
        theta.Sort();

        // Convert to cartesian and add to list
        List<Vector3> positions = new List<Vector3>(NumberOfSamples);
        for (int i = 0; i < NumberOfSamples; i++)
        {
            Vector3 position = new Vector3(Mathf.Cos(theta[i]) * Mathf.Sin(phi[i]), Mathf.Cos(phi[i]), Mathf.Sin(theta[i]) * Mathf.Sin(phi[i])) * radius[i];
            positions.Add(position);
        }
        return positions;
    }
}
