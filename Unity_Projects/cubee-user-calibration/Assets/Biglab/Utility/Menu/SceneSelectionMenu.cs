using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Biglab
{
    [RequireComponent( typeof( SceneList ) )]
    public class SceneSelectionMenu : MonoBehaviour
    {
        public RectTransform Content;

        public GameObject ButtonPrefab;

        private SceneList _SceneList;

        void Awake()
        {
            _SceneList = GetComponent<SceneList>();

            var buttonRect = ButtonPrefab.GetComponent<RectTransform>();
            var buttonSpacing = buttonRect.rect.xMin;
            var buttonHeight = buttonRect.sizeDelta.y;

            var height = 0F;
            var number = 0;

            Selectable prev = null;
            foreach( var scene in _SceneList.Scenes )
            {
                var goButton = Instantiate( ButtonPrefab );
                goButton.transform.SetParent( Content, false );
                goButton.transform.localPosition += new Vector3( 0, -height, 0 );
                goButton.name = string.Format( "Button ( {0} )", scene.Scene.Path );

                // 
                height += buttonHeight + buttonSpacing;
                number++;

                // Configure button
                goButton.GetComponentInChildren<Text>().text = scene.Title;
                var button = goButton.GetComponentInChildren<Button>();
                button.onClick.AddListener( () =>
                {
                    SceneManager.LoadScene( scene.Scene.Path, LoadSceneMode.Single );
                } );

                // Adjust navigation
                LinkNavigation( prev, button );
                prev = button;
            }

            // 
            Content.sizeDelta = new Vector2( Content.sizeDelta.x, height );
        }

        private void LinkNavigation( Selectable prev, Selectable next )
        {
            if( prev != null )
            {
                // Previous element goes to next
                var prevNav = prev.navigation;
                prevNav.selectOnDown = next;
                prev.navigation = prevNav;
            }

            // Next element goes to previous
            var nextNav = next.navigation;
            nextNav.selectOnUp = prev;
            next.navigation = nextNav;
        }

        public void GotoScene()
        {
            var currentScene = SceneManager.GetActiveScene();
            if( currentScene.buildIndex >= 0 ) SceneManager.LoadScene( currentScene.buildIndex );
            else SceneManager.LoadScene( currentScene.path );
        }

        public void RestartCurrentScene()
        {
            var currentScene = SceneManager.GetActiveScene();
            if( currentScene.buildIndex >= 0 ) SceneManager.LoadScene( currentScene.buildIndex );
            else SceneManager.LoadScene( currentScene.path );
        }

        void Update()
        {

        }
    }
}