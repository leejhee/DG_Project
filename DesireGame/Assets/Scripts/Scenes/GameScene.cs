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

        [Header("Test(shootindex의 경우 편하게 하려고 임의로 이렇게 함)")]
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


            // 타일 시스템 결합
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

        [ContextMenu("투사체를 날려보아요")]
        private void TestProjectileShoot()
        {
            TestChar.CharAction.CharAttackAction
                (new CharAttackParameter(TestEnemy, projectileShootIndex, CharAI.eAttackMode.None));
        }
        
        [ContextMenu("전투를 시작해보아요")]
        public void TestAIInitialize()
        {
            // 임시
            StageManager.Instance.SetIsFinish(false); 
            CharManager.Instance.WakeAllCharAI();
        }

        [ContextMenu("시너지 조회")]
        public void TestShowSynergy()
        {
            SynergyManager.Instance.ShowCurrentSynergies();
        }


        public void TestMessageSystem(GameSceneMessageParam param)
        {
            Debug.Log($"GameScene 에서 수신 완료 메시지 내용은 - {param.message} - ");
        }
    }
}