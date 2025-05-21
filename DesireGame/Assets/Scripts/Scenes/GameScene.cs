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
        [SerializeField] private int testPlayerTile;
        [SerializeField] private CharBase TestEnemy;
        [SerializeField] private int testEnemyTile;
        [SerializeField] private long projectileShootIndex;

        private void Awake()
        {
            GameManager instance = GameManager.Instance;
            StageManager.Instance.Init();
        }

        private void Start()
        {
            UIManager.Instance.ShowUI(GameSceneUIPrefab);

            // 타일 시스템 결합
            TestChar = CharManager.Instance.CharGenerate
                (new CharTileParameter(SystemEnum.eScene.GameScene,
                    testPlayerTile,
                700));
            
            TestEnemy = CharManager.Instance.CharGenerate
                (new CharTileParameter(SystemEnum.eScene.GameScene,
                    testEnemyTile,
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
            StageManager.Instance.StartCombat();
        }

        [ContextMenu("시너지 조회")]
        public void TestShowSynergy()
        {
            SynergyManager.Instance.ShowCurrentSynergies();
        }

        [ContextMenu("애니메이션 테스트")]
        public void TestAnimation()
        {
            TestChar.CharAnim.PlayAnimation(state: PlayerState.ATTACK);
            StartCoroutine(playafter10frame());
        }

        private IEnumerator playafter10frame()
        {
            yield return new WaitForEndOfFrame();
            TestEnemy.CharAnim.PlayAnimation(PlayerState.DAMAGED);
        }
        
        public void TestMessageSystem(GameSceneMessageParam param)
        {
            Debug.Log($"GameScene 에서 수신 완료 메시지 내용은 - {param.message} - ");
        }
    }
}