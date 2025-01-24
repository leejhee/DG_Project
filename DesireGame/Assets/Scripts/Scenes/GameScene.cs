using UnityEngine;


namespace Client
{
    /// <summary>
    /// Scene √ ±‚»≠ class
    /// </summary>
    public class GameScene : MonoBehaviour
    {
        [SerializeField] GameObject GameSceneUIPrefab;

        private void Awake()
        {
            GameManager instance = GameManager.Instance;
        }

        private void Start()
        {
            //UIManager.Instance.ShowSceneUI<UI_GameScene>();
            UIManager.Instance.ShowUI(GameSceneUIPrefab);
        }
    }
}