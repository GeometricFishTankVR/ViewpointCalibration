using UnityEngine;
using UnityEngine.Assertions;

public class TargetAligner : MonoBehaviour {
    public GameObject Target;
    private GameObject Outer;
    private GameObject Inner;
    public GameObject vD;
    public float depthDisplacement;

    #region monobehaviour
    void OnEnable()
    {
        Assert.IsNotNull(Outer, "Outer cannot be null.");
        Assert.IsNotNull(Inner, "Inner cannot be null.");
        Assert.IsNotNull(vD, "vD cannot be null");
        Outer = Target.transform.FindChild("Outer").gameObject;
        Inner = Target.transform.FindChild("Inner").gameObject;
    }

    void Update()
    {
        if (Outer != null && Inner != null)
        {
            Target.transform.rotation = vD.transform.rotation;
            //Target.transform.rotation = Quaternion.LookRotation(vD.transform.position - Target.transform.position, Vector3.up);
            //float distanceToTarget = Vector3.Distance(Target.transform.position, vD.transform.position);
            Outer.transform.position = Target.transform.TransformPoint(Vector3.forward * depthDisplacement);
            Inner.transform.position = Target.transform.TransformPoint(Vector3.back * depthDisplacement);

            float distanceToOuter = Vector3.Distance(Outer.transform.position, vD.transform.position);
            float distanceToInner = Vector3.Distance(Inner.transform.position, vD.transform.position);

            float perspectiveScaleFactor = (distanceToInner / distanceToOuter) * 1.01f;
            Inner.transform.localScale = Outer.transform.localScale * perspectiveScaleFactor;
            Inner.transform.rotation = Target.transform.rotation;// Quaternion.LookRotation(vD.transform.position - Inner.transform.position, Target.transform.up);//vD.transform.rotation;
            Outer.transform.rotation = Target.transform.rotation;// Quaternion.LookRotation(vD.transform.position - Outer.transform.position, Target.transform.up);//vD.transform.rotation;
        }
    }
    #endregion
}
