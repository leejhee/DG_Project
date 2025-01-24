using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class GameSceneMap : MonoBehaviour
    {
        [SerializeField] GameObject MapHUD;

        private void Awake()
        {
            UIManager.Instance.SetCanvas(gameObject, false);
        }

        // �ΰ��� ������ ���� �� UI �����ϵ��� �ۼ��� ��.
        // �Ƹ� ī�޶� ���� �ʿ��Ұ���.
    }
}