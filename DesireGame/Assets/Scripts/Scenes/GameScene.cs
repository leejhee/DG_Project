using UnityEngine;


namespace Client
{
    /// <summary>
    /// Scene �ʱ�ȭ class
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
            //UIManager.Instance.ShowUI(GameSceneUIPrefab);
        }
    }
}