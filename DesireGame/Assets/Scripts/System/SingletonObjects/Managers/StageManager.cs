using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Client
{
    public class StageManager : Singleton<StageManager>
    {
        private StageManager() { }
        
        // ���� ��������
        public int Stage { get; private set; }
        // �������� ���� ���� ����
        public bool CanStartStage { get; private set; } = true;
        public bool IsStageFinished { get; private set; } = false;

        public override void Init()
        {
            base.Init();
            CharManager.Instance.OnCharTypeEmpty += CheckWinCondition;
            Stage = 1;
        }

        public void StartStage(int stageNum)
        {
            if (CanStartStage == false)
                return;

            if (DataManager.Instance.MonsterSpawnStageMap.ContainsKey(stageNum) == false)
                return;

            var stageList = DataManager.Instance.MonsterSpawnStageMap[stageNum];
            foreach (var stage in stageList)
            {
                CharBase charMonster = CharManager.Instance.CharGenerate(stage.MonsterID);
                TileManager.Instance.SetChar(stage.PositionIndex, charMonster);
            }

            TileManager.Instance.SwitchTileCombatmode(false);

        }

        public void StartCombat()
        {
            SetIsFinish(false);
            TileManager.Instance.SwitchTileCombatmode(true);
            CharManager.Instance.WakeAllCharAI();
        }

        /// <summary>
        /// �̰� ȣ���� ������ ���� ���������� ������ ��� ��
        /// </summary>
        public void MoveToNextStage()
        {
            if(TryGetNextStage(Stage))
            {
                CharManager.Instance.ReturnToOriginPos();
                StartStage(++Stage);
            }
            else
            {
                Debug.LogError("���� �������� ����");
            }
        }

        /// <summary>
        /// ���� ����
        /// </summary>
        public void CheckWinCondition(Type charType)
        {
            // ���� ������ �ѹ��� üũ�ϵ���
            if (!IsStageFinished) IsStageFinished = true;
            else return;

            Debug.Log($"{charType} Ÿ���� ��� ĳ���Ͱ� ���ŵǾ����ϴ�.");

            if (charType == typeof(CharPlayer))
            {
                Debug.Log("��� �÷��̾ �׾����ϴ�. ����..");
                // ���� ���� ó��
            }
            else if (charType == typeof(CharMonster))
            {
                Debug.Log("��� ���� ��������ϴ�. �̰��!!");
                CharManager.Instance.CopyFieldPlayerID();
                MoveToNextStage();
            }
        }

        /// <summary>
        /// ���� �������� ���� ��������
        /// </summary>
        public bool TryGetNextStage(int _stage)
        {
            CharManager.Instance.ClearAllChar();

            int nextStage = _stage + 1;
            if (DataManager.Instance.MonsterSpawnStageMap.ContainsKey(nextStage) == false)
            {
                return false;
            }

            return true;

        }

        public void SetIsFinish(bool isFinish)
        {
            IsStageFinished = isFinish;
        }
    }
}