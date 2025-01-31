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
            TestChar = CharManager.Instance.CharGenerate(new CharParameter(SystemEnum.eScene.GameScene, new Vector3(-5, 1, 0), 1));

            float radian = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float x = Mathf.Cos(radian);
            float z = Mathf.Sin(radian);
            Vector3 targetPosition = transform.position + new Vector3(x, 0f, z) * moveDistance;
            moveParameter = new CharMoveParameter(TestChar.transform.position + targetPosition);
        }
        private void FixedUpdate()
        {
            TestChar.CharAction.CharMoveAction(moveParameter);
        }



    }
}