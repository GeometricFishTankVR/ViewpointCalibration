using UnityEngine;

namespace Biglab
{
    public class PlayerManager : MonoBehaviour
    {
        public Transform PlayerOneTransform;

        public Transform PlayerTwoTransform;

        public Transform GetPlayerOneTranform()
        {
            return PlayerOneTransform;
        }

        public Transform GetPlayerTwoTransform()
        {
            return PlayerTwoTransform;
        }
    }

}