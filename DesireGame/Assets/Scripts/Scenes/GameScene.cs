using System.Collections;
using UnityEngine;
using static Client.SystemEnum;


namespace Client
{
    /// <summary>
    /// Scene �ʱ�ȭ class
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

        [ContextMenu("����ü�� �������ƿ�")]
        private void TestProjectileShoot()
        {
            long skillIndex = DataManager.Instance.GetData<CharData>(200).skill1;
            TestChar.CharAction.CharAttackAction
                (new CharAttackParameter(TestEnemy, skillIndex));
        }

        [ContextMenu("������ �����غ��ƿ�")]
        public void TestAIInitialize()
        {
            CharManager.Instance.WakeAllCharAI();
        }
    }
}