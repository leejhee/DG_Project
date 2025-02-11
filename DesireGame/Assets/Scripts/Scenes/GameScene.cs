using System.Collections;
using UnityEngine;
using static Client.SystemEnum;


namespace Client
{
    /// <summary>
    /// Scene 초기화 class
    /// </summary>
    public class GameScene : MonoBehaviour
    {
        [SerializeField] GameObject GameSceneUIPrefab;

        [Header("Test")]
        [SerializeField] private CharBase TestChar;
        [SerializeField] private Vector3 testPlayerPoint;
        [SerializeField] private CharBase TestEnemy;
        [SerializeField] private Vector3 testEnemyPoint;

        private void Awake()
        {
            GameManager instance = GameManager.Instance;
        }

        private void Start()
        {
            TestChar = CharManager.Instance.CharGenerate
                (new CharParameter(SystemEnum.eScene.GameScene,
                testPlayerPoint,
                200));

            TestEnemy = CharManager.Instance.CharGenerate
                (new CharParameter(SystemEnum.eScene.GameScene,
                testEnemyPoint,
                100)); 
        }

        [ContextMenu("투사체를 날려보아요")]
        private void TestProjectileShoot()
        {
            long skillIndex = DataManager.Instance.GetData<CharData>(200).skill1;
            TestChar.CharAction.CharAttackAction
                (new CharAttackParameter(TestEnemy, skillIndex));
        }

        [ContextMenu("전투를 시작해보아요")]
        public void TestAIInitialize()
        {
            CharManager.Instance.WakeAllCharAI();
        }
    }
}