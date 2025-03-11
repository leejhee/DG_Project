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

        [Header("Test(shootindex�� ��� ���ϰ� �Ϸ��� ���Ƿ� �̷��� ��)")]
        [SerializeField] private CharBase TestChar;
        [SerializeField] private Vector3 testPlayerPoint;
        [SerializeField] private CharBase TestEnemy;
        [SerializeField] private Vector3 testEnemyPoint;
        [SerializeField] private long projectileShootIndex;

        private void Awake()
        {
            GameManager instance = GameManager.Instance;
            StageManager.Instance.Init();
        }

        private void Start()
        {


            // Ÿ�� �ý��� ����
            TestChar = CharManager.Instance.CharGenerate
                (new CharTileParameter(SystemEnum.eScene.GameScene,
                15,
                200));
            
            TestEnemy = CharManager.Instance.CharGenerate
                (new CharTileParameter(SystemEnum.eScene.GameScene,
                35,
                10000));

            MessageManager.SubscribeMessage<GameSceneMessageParam>(this, TestMessageSystem);
        }

        [ContextMenu("����ü�� �������ƿ�")]
        private void TestProjectileShoot()
        {
            TestChar.CharAction.CharAttackAction
                (new CharAttackParameter(TestEnemy, projectileShootIndex, CharAI.eAttackMode.None));
        }
        
        [ContextMenu("������ �����غ��ƿ�")]
        public void TestAIInitialize()
        {
            // �ӽ�
            StageManager.Instance.SetIsFinish(false); 
            CharManager.Instance.WakeAllCharAI();
        }

        [ContextMenu("�ó��� ��ȸ")]
        public void TestShowSynergy()
        {
            SynergyManager.Instance.ShowCurrentSynergies();
        }


        public void TestMessageSystem(GameSceneMessageParam param)
        {
            Debug.Log($"GameScene ���� ���� �Ϸ� �޽��� ������ - {param.message} - ");
        }
    }
}