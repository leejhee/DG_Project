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
        /// CharID를 EnemyID로 변경
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
            #region 테스트용 디버그
            StringBuilder sb = new();
            foreach (long id in dropList)
            {
                sb.Append($"{id} ");
            }
            Debug.Log($"{Index}가 드랍할 건 {dropList.Count}개, {sb.ToString()}");
            #endregion

        }

        void DropItemBeads()
        {
            // 최종 결정된 아이템 박스 프리팹 떨구기


        }
    }
}