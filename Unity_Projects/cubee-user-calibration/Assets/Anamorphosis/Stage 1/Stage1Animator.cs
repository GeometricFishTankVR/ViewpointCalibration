using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Stage1Animator : MonoBehaviour {
    public Object[] Targets;
    public Transform[] TargetPads;
    public Transform[] ExitPads;
    public Transform Spawn;
    public TargetAligner Aligner;
    public Object StaticStickerParticle;
    public GameObject ViewrayTarget;
    private List<GameObject> spawnedTargets;
    private List<Vector3> oVSamples;
    private int targetIdx;
    private Vector3 oVStart;

    private bool isAnimating = false;
    #region monobehaviour
    void OnEnable()
    {
        Assert.IsNotNull(Targets, "Targets cannot be null.");
        Assert.IsNotNull(ViewrayTarget, "Viewray target cannot be null.");
        Assert.IsTrue(Targets.Length > 0, "Target length must be greater than 0.");
    }

    void Update () {
        if (Input.GetButtonUp("XboxOneMenu(Start)"))
        {
            StartStage1();
        }
        if (Input.GetButtonUp("XboxOneA") && !isAnimating && oVSamples.Count < Targets.Length)
        {
            oVSamples.Add(PersistentProjectStorage.Instance.CurrentViewpointCalibration.oV); // Save the oV sample
            Aligner.enabled = false; // Disable the target aligner so the target can be animated
            TransformTargetIntoSticker(TargetPads[targetIdx], spawnedTargets[targetIdx], OnStickerAnimationEnd); // Animate the target
        }

	}
    #endregion

    private void StartStage1()
    {
        targetIdx = 0;
        Aligner.enabled = false;
        oVStart = PersistentProjectStorage.Instance.CurrentViewpointCalibration.oV;
        oVSamples = new List<Vector3>(Targets.Length);
        spawnedTargets = new List<GameObject>(Targets.Length);
        spawnedTargets.Add(SpawnTarget(Spawn.position, Quaternion.identity, Targets[targetIdx]));
    }

    private void OnStickerAnimationEnd()
    {
        targetIdx++;
        if (targetIdx == Targets.Length)
        {
            // Completed
            Vector3 oVFinish = Vector3.zero;
            foreach(Vector3 oVSample in oVSamples)
            {
                oVFinish += oVSample;
            }
            oVFinish /= oVSamples.Count;
            PersistentProjectStorage.Instance.CurrentViewpointCalibration.oV = oVFinish;
            Debug.Log("Completed user-dependent oV fix. Changed from " + oVStart.ToString("f4") + " to " + oVFinish.ToString("f4"));
            StartCoroutine(FinishAnimation());
        }
        else
        {
            spawnedTargets.Add(SpawnTarget(Spawn.position, Quaternion.identity, Targets[targetIdx]));
            PersistentProjectStorage.Instance.CurrentViewpointCalibration.oV = oVStart; // Reset oV back to what it started as
        }
    }

    private GameObject SpawnTarget(Vector3 position, Quaternion rotation, Object target)
    {
        GameObject go = Instantiate(target, position, rotation) as GameObject;
        go.name = target.name;
        Aligner.Target = go;
        Aligner.enabled = true;
        return go;
    }

    private void TransformTargetIntoSticker(Transform grave, GameObject target, System.Action callback)
    {
        StartCoroutine(StickerAnimation(target, grave, callback));
    }

    IEnumerator FinishAnimation()
    {
        isAnimating = true;
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < spawnedTargets.Count; i++)
        {
            TargetPads[i].gameObject.SetActive(false);
            RotateAroundAxis script = spawnedTargets[i].AddComponent<RotateAroundAxis>();
            script.RotationAxis = spawnedTargets[i].transform.forward;
            script.RotationSpeed = 900;
            yield return new WaitForSeconds(0.25f);
        }

        for (int i=0; i < spawnedTargets.Count; i++)
        {
            TransformInterpolater script = spawnedTargets[i].AddComponent<TransformInterpolater>();
            script.Speed = 2;
            script.ChangeScale = false;
            script.Target = ExitPads[i];
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(2);
        foreach(GameObject go in spawnedTargets)
        {
            Destroy(go);
        }

        isAnimating = false;
        SendMessageUpwards("OnStage1Completed");
    }

    IEnumerator StickerAnimation(GameObject go, Transform grave, System.Action callback)
    {
        isAnimating = true;
        //GameObject particles = Instantiate(DynamicStickerParticle, go.transform) as GameObject;
        //particles.transform.localPosition = Vector3.zero;

        Transform outer = go.transform.FindChild("Outer");
        Transform inner = go.transform.FindChild("Inner");
        inner.localScale = outer.localScale * 1.01f;

        TransformInterpolater translateInnerScript = outer.gameObject.AddComponent<TransformInterpolater>();
        translateInnerScript.Target = go.transform;
        translateInnerScript.Speed = 20;
        TransformInterpolater translateOuterScript = inner.gameObject.AddComponent<TransformInterpolater>();
        translateOuterScript.Target = go.transform;
        translateOuterScript.Speed = 20;
        yield return new WaitForSeconds(0.5f);
        RotateAroundAxis rotate1 = go.AddComponent<RotateAroundAxis>();
        rotate1.RotationAxis = go.transform.TransformVector(Vector3.forward);
        rotate1.RotationSpeed = 900;
        //yield return new WaitForSeconds(1);
        //RotateAroundAxis rotate2 = go.AddComponent<RotateAroundAxis>();
        //rotate2.RotationAxis = go.transform.TransformVector(Vector3.right);
        //rotate2.RotationSpeed = 900;
        yield return new WaitForSeconds(1);
        RotateAroundAxis rotate3 = go.AddComponent<RotateAroundAxis>();
        rotate3.RotationAxis = go.transform.TransformVector(Vector3.up);
        rotate3.RotationSpeed = 900;
        yield return new WaitForSeconds(1);
        Destroy(rotate1);
        //Destroy(rotate2);
        Destroy(rotate3);
        
        // Finally, head to the rest position
        TransformInterpolater translateScript = go.AddComponent<TransformInterpolater>();
        translateScript.Target = grave;
        translateScript.Speed = 4;
        translateScript.ChangeScale = true;
        yield return new WaitForSeconds(2);
        //Destroy(particles);

        GameObject particles = Instantiate(StaticStickerParticle, go.transform) as GameObject;
        particles.transform.localPosition = Vector3.zero;

        isAnimating = false;
        callback();
    }


}
