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

        CharBase TestChar;
        float moveDistance = 5f;
        CharMoveParameter moveParameter;

        private void Awake()
        {
            GameManager instance = GameManager.Instance;
        }

        private void Start()
        {
            //UIManager.Instance.ShowUI(GameSceneUIPrefab);
            TestChar = CharManager.Instance.CharGenerate(new CharParameter(SystemEnum.eScene.GameScene, new Vector3(-5, 1, 0), 0));

            //TestChar.CharSKillInfo.PlaySkill(TestChar.)
        }
        private void FixedUpdate()
        {
            //TestChar.CharAction.CharMoveAction(moveParameter); 
        }



    }
}