using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using Client;



namespace Client
{

    [CreateAssetMenu(fileName = "GoogleSheet", menuName = "Scriptable Object/GoogleSheet", order = int.MaxValue)]

    public class GoogleSheet : ScriptableObject
    {
        public string associatedSheet = "";
        public string associatedDataWorksheet = "";
        //public string associatedData2Worksheet = "";

        enum DataCell
        {
            ��,
            ��,
            ��,
            ��,

        }
        enum Data2Cell
        {

        }
        /// <summary>
        /// ���� ������ ���� �Լ�
        /// �����͸� �ٸ� �ڷᱸ���� �ٲٰ� ����
        /// </summary>
        /// <param name="line"> \t ���� Cell�� ������ �� �� </param>
        internal void GetData(string line)
        {
            string[] tmp = line.Split('\t');

            for (int i = 0; i < tmp.Length; i++)
            {
                //Debug.Log($"{(DataCell)i} : {int.Parse(tmp[i])}");
            }

        }

    }
}