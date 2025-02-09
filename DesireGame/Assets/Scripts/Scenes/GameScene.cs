using System.Collections;
using UnityEngine;


namespace Client
{
    /// <summary>
    /// Scene √ ±‚»≠ class
    /// </summary>
    public class GameScene : MonoBehaviour
    {
        [SerializeField] GameObject GameSceneUIPrefab;

        [Header("Test")]
        private CharBase TestChar;
        private CharBase TestEnemy;

        private void Awake()
        {
            GameManager instance = GameManager.Instance;
        }

        private void Start()
        {
            TestChar = CharManager.Instance.CharGenerate
                (new CharParameter(SystemEnum.eScene.GameScene, 
                new Vector3(-5, 1, 0), 
                0));
        }

    }
}