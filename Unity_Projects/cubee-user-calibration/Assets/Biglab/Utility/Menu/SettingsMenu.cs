using UnityEngine;

namespace Biglab
{
    public class SettingsMenu : MonoBehaviour
    {
        private SceneSelectionMenu _SceneSelectionMenu;

        void Start()
        {
            _SceneSelectionMenu = transform.parent.GetComponentInChildren<SceneSelectionMenu>();
        }

        /// <summary>
        /// Enables or disables multiplayer rendering.
        /// </summary>
        public void ToggleMultiplayer( bool enableMultiplayer )
        {
            // TODO: Andrew, Make this toggle multiplayer
            throw new System.NotImplementedException();
            _SceneSelectionMenu.RestartCurrentScene();
        }
    }
}