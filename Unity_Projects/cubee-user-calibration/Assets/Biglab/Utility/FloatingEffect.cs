using UnityEngine;

namespace Biglab
{
    /// <summary>
    /// A floating effect.
    /// 
    /// Note: This component causes an object to freeze its effective location to its location on Start().
    /// </summary>
    public class FloatingEffect : MonoBehaviour
    {
        private Quaternion BaseRotation;
        private Vector3 BasePosition;

        public float FloatTimeMultiplier = 0.5F;

        public float DriftingIntensity = 0.1F;

        public float WobbleIntensity = 0.3F;

        void Start()
        {
            BaseRotation = transform.rotation;
            BasePosition = transform.position;
        }

        void Update()
        {
            var scale = transform.lossyScale.x;
            var time = Time.time * FloatTimeMultiplier;

            // var xx = Mathf.Sin( BasePosition.x + time ) * DriftingIntensity * scale;
            // var yy = Mathf.Cos( BasePosition.y + time * 2F ) * DriftingIntensity * scale;
            // var zz = Mathf.Sin( BasePosition.z + time / 2F ) * DriftingIntensity * scale;
            // transform.position = BasePosition + new Vector3( xx, yy, zz );

            var ax = Mathf.Cos( BasePosition.x + time ) * 45 * WobbleIntensity * scale;
            var ay = Mathf.Sin( BasePosition.y + time * 2F ) * 45 * WobbleIntensity * scale;
            var az = Mathf.Sin( BasePosition.z + time / 2F ) * 45 * WobbleIntensity * scale;
            transform.rotation = BaseRotation * Quaternion.Euler( ax, ay, az );
        }
    }
}