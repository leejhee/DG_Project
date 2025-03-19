using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Client
{
    public class CharMonster : CharBase
    {
        protected override SystemEnum.eCharType CharType => SystemEnum.eCharType.ENEMY;
        protected override void CharInit()
        {
            base.CharInit();
            CharManager.Instance.SetChar<CharMonster>(this);
        }

        public void Patrol()
        {
            //Patrol
        }

        public override void CharDistroy()
        {
            base.CharDistroy();
            DrawItems();
        }

        /// <summary>
        /// CharID�� EnemyID�� ����
        /// </summary>
        /// <param name="charID"></param>
        /// <returns></returns>
        long GetEnemyID(long charID)
        {
            var enemyDataList = DataManager.Instance.GetDataList<EnemyData>();
            foreach (var _enemyData in enemyDataList)
            {
                var enemyData = _enemyData as EnemyData;
                if (enemyData.CharID == charID) return enemyData.Index;
            }
            return -1;
        }

        void DrawItems()
        {
            List<long> dropList = ItemManager.Instance.SetDropTableList(GetEnemyID(Index));
            #region �׽�Ʈ�� �����
            StringBuilder sb = new();
            foreach (long id in dropList)
            {
                sb.Append($"{id} ");
            }
            Debug.Log($"{Index}�� ����� �� {dropList.Count}��, {sb.ToString()}");
            #endregion

        }

        void DropItemBeads()
        {
            // ���� ������ ������ �ڽ� ������ ������


        }
    }
}