using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    [System.Serializable]
    public class GameSave
    {
        public int credit;

        public List<CharFieldSave> CurrentFieldChar;

        public int maxCharCount;

        public List<CharItemData> CurrentTotalItem;

        public GameSave()
        {
            credit = 0;
            CurrentFieldChar = new List<CharFieldSave>();
            maxCharCount = 1; // �����ѹ� ��������...
            CurrentTotalItem = new List<CharItemData>();
            
        }

        // ������ ���� ��  ���� ��Ƽ��Ʈ �۾� ����.

        //////////////////////////////////////////////////////////////////////////
        /* �ļ��� �۾�
            StoreData.
        - ���� ����(�ϴ� �ļ���. ���� ��ȹ ���� �� �۾� ���)
            - ���� ����
            - ������ �÷��� �⹰ or ������ ��

            StageData.
        - ��(�� ��� ��������� �� ��ȹ ���� �� �۾� ���)
            - ���� �� ��
            - ���� ��
            - ���� ��ġ
            - ���� �� Ư�� ���� Ž�� ����
            - ex) 3��° ���� Ž�� �Ϸ�, 6��° ���� ��Ž�� ��
            - �̺�Ʈ �湮 ����
            - ex) 1ȸ�� �̺�Ʈ�� ��ȣ�ۿ� �߾��°�?
        */
    }
}