using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Client
{
    public class StageManager : Singleton<StageManager>
    {
        private StageManager() 
        {
            
        }
        
        // ���� ��������
        public int Stage { get; private set; }
        // �������� ���� ���� ����
        public bool CanStartStage { get; private set; } = true;

        public override void Init()
        {
            base.Init();
            CharManager.Instance.OnCharTypeEmpty += CheckWinCondition;
            Stage = 0;
        }

        public void StartStage(int stageNum)
        {
            if (CanStartStage == false)
                return;

            if (DataManager.Instance.MonsterSpawnStageMap.ContainsKey(stageNum) == false)
                return;
            var StageMonsterList = DataManager.Instance.MonsterSpawnStageMap[stageNum];

            foreach (var monsterInfo in StageMonsterList)
            {
                CharBase charMoster = CharManager.Instance.CharGenerate(monsterInfo.MonsterID);
                var tileObj = TileManager.Instance.GetTile(monsterInfo.PositionIndex);
            }
        }

        /// <summary>
        /// �̰� ȣ���� ������ ���� ���������� ������ ��� ��
        /// </summary>
        public void MoveToNextStage()
        {
            if(TryGetNextStage(Stage))
            {
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
            Debug.Log($"{charType} Ÿ���� ��� ĳ���Ͱ� ���ŵǾ����ϴ�.");

            if (charType == typeof(CharPlayer))
            {
                Debug.Log("��� �÷��̾ �׾����ϴ�. ����..");
                // ���� ���� ó��
            }
            else if (charType == typeof(CharMonster))
            {
                Debug.Log("��� ���� ��������ϴ�. �̰��!!");
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
            var stageList = DataManager.Instance.MonsterSpawnStageMap[nextStage];
            foreach (var stage in stageList)
            {
                CharBase charMonster = CharManager.Instance.CharGenerate(stage.MonsterID);
                TileManager.Instance.SetChar(stage.PositionIndex, charMonster);
            }
            return true;

        }
    }
}