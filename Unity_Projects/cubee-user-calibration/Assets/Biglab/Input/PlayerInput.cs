using Malee;
using UnityEngine;

namespace Biglab.Input
{
    public class PlayerInput : MonoBehaviour
    {
        private CustomInput Input;

        [Reorderable, SerializeField]
        private StringList Buttons;

        [Reorderable, SerializeField]
        private StringList Axes;

        [System.Serializable]
        private class StringList : ReorderableArray<string> { }

        void Start()
        {
            Input = FindObjectOfType<CustomInput>();
            throw new System.NotImplementedException();
        }

        void Update()
        {
            // TODO: Extract each input value, and keep in local map?
        }
    }
}