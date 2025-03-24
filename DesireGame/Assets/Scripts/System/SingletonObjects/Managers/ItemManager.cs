using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    public class ItemManager : Singleton<ItemManager>
    {
        #region ������
        ItemManager() { }
        #endregion
        // ���� ID ���� 
        private long _nextID = 0;
        private List<GameObject> itemBeads         = new(); // �ʵ忡 �ִ� ������ ����
        private List<int>        cumulativeWeights = new(); // ���꽺�� - ����ġ ���� ���� ����
        public List<int> CumulativeWeights => cumulativeWeights;
        public override void Init()
        {
            base.Init();
            AccumulateWeights();
        }
        public long GetNextID() => _nextID++;

        public void RegisterItem(GameObject item)
        {
            if (!itemBeads.Contains(item))
            {
                itemBeads.Add(item);
            }
        }

        public void DeactivateItem(GameObject item)
        {
            item.SetActive(false);
        }

        public bool AreAllItemsCollected()
        {
            foreach (GameObject item in itemBeads)
            {
                if (item.activeSelf) return false;
            }
            return true;
        }

        public void CleanupItems()
        {
            foreach (GameObject item in itemBeads)
            {
                if (!item.activeSelf)
                {
                    GameObject.Destroy(item);
                }
            }
            itemBeads.Clear();
        }
        #region ���� ���� ���� ��Ʈ
        /// <summary>
        /// ����ġ ���� �� �̸� ��� - binary search��
        /// </summary>
        public void AccumulateWeights()
        {
            cumulativeWeights.Clear(); // ���� ������ �ʱ�ȭ
            int sum = 0;

            var ItemSubStatDataList = DataManager.Instance.GetDataList<ItemSubStatData>();
            if (ItemSubStatDataList == null)
            {
                Debug.LogWarning("ItemSubStatDataList �� ã�� ����");
            }

            foreach(var _itemSubStatData in ItemSubStatDataList)
            {
                var itemSubStatData = _itemSubStatData as ItemSubStatData;
                int proWeight = itemSubStatData.proWeight;
                sum += proWeight;
                cumulativeWeights.Add(sum);
            }
        }

        /// <summary>
        /// ���� ������ ���ϱ�
        /// </summary>
        /// <param name="itemSubStatData"></param>
        /// <returns></returns>
        public int SetStatIncrease(ItemSubStatData itemSubStatData)
        {
            return Random.Range(itemSubStatData.min, itemSubStatData.max + 1);
        }

        /// <summary>
        /// ���� ���� ���� ��í
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<(SystemEnum.eStats eStat, int increase)> GenerateSubStats(int count)
        {

            List<(SystemEnum.eStats eStat, int increase)> list = new();
            if (count == 0) return list;

            var ItemSubStatDataList = DataManager.Instance.GetDataList<ItemSubStatData>();
            if (ItemSubStatDataList == null)
            {
                Debug.LogWarning("ItemSubStatDataList �� ã�� ����");
            }

            if (cumulativeWeights.Count == 0) AccumulateWeights();

            for (int i = 0; i < count; i++)
            {
                float randomValue = Random.Range(0, cumulativeWeights[^1]);
                int index = cumulativeWeights.FindIndex(x => x > randomValue);

                var _itemSubStatData = ItemSubStatDataList[index];
                var itemSubStatData = _itemSubStatData as ItemSubStatData;
                list.Add((itemSubStatData.subStats, SetStatIncrease(itemSubStatData)));
                Debug.Log($"{i} ���� ���� : {list[i].eStat}, ������ : {list[i].increase}");
            }
            return list;
        }
        #endregion

        // ��ġ �ű� ����..
        public List<long> SetDropTableList(long enemyID)
        {
            // 1. ���� ���� ��� ���̺� �׷� ���̵� ������ ���̺� ���� ��������
            EnemyData enemyData = DataManager.Instance.GetData<EnemyData>(enemyID);
            DropTableGroup dropTableGroup = DataManager.Instance.GetData<DropTableGroup>(enemyData.dropTableGroupID);

            // 2. Ȯ�� ��� ���̺� ���̵�� �׳� ����Ʈ�� �ֱ�
            List<long> dropTableIDList = new List<long>(dropTableGroup.fixDrop);

            // 3-1. ���� ��� ����Ʈ���� ����ġ �ݿ��ؼ� � ������ ����
            List<int> cumulative = new List<int>()
            {
                dropTableGroup.noItem,
                dropTableGroup.noItem + dropTableGroup.oneItem,
                dropTableGroup.noItem + dropTableGroup.oneItem + dropTableGroup.twoItem,
                dropTableGroup.noItem + dropTableGroup.oneItem + dropTableGroup.twoItem + dropTableGroup.threeItem
            }; // ����

            float randomValue = Random.Range(0f, cumulative[^1]);
            int itemQuantity = cumulative.FindIndex(x => x > randomValue);

            // 3-2. ���� ��� ����Ʈ�� ���̵�� ������ ������̺� ���� ��������, Ȯ�� ����ġ(in DropTable) ����� ���� ���� ����ġ ����Ʈ �����
            List<DropTable> _dropTableList = new();
            List<int> cumulative2 = new();
            int sum = 0;
            foreach (long id in dropTableGroup.ranDrop)
            {
                DropTable dropTable = DataManager.Instance.GetData<DropTable>(id);
                sum += dropTable.prob;
                _dropTableList.Add(dropTable);
                cumulative2.Add(sum);
            }

            // 3-3. 3-1���� ���� ������ ������ŭ ������� ����Ʈ���� ������̺� id ��������
            for (int i = 0; i < itemQuantity; i++)
            {
                float rand = Random.Range(0f, cumulative2[^1]);
                int id = cumulative2.FindIndex(x => x > rand);

                long randDroptableID = dropTableGroup.ranDrop[id];
                dropTableIDList.Add(randDroptableID);
            }

            return dropTableIDList;
        }


        public void DropItemBeads(Vector3 position, List<long> dropList)
        {
            foreach (int dropIndex in dropList)
            {
                DropTable dropTable = DataManager.Instance.GetData<DropTable>(dropIndex);
                if (dropTable == null)
                {
                    Debug.LogError($"DropTable ID {dropIndex} ���� ����");
                    return;
                }

                Item item = new Item(dropTable.itemID);
                GameObject beadPrefab = ObjectManager.Instance.Instantiate($"Item/ItemBead");

                if (beadPrefab != null)
                {
                    // �ڽ� �������� �ν��Ͻ�ȭ�ϰ� ������ ���� �ֱ�
                    Vector3 randPos = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
                    GameObject itemBead = GetItemBeadwithColor(beadPrefab, dropTable.beadColor);
                    itemBead.transform.position = position + randPos;
                    itemBead.GetComponent<ItemBead>().WrapItem(item, dropTable.amount);

                }
            }
        }
        GameObject GetItemBeadwithColor(GameObject bead, string hexCode)
        {
            bead.GetComponent<Renderer>().material.color = Util.GetHexColor(hexCode);
            return bead;
        }

    }
}