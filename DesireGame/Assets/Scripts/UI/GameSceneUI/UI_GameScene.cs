using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace Client
{
    // GameScene �ȿ� �� UI ��ü.
    // [TODO] : UI ������ ��ü�� ���� Ÿ�� ���� ������ ��.(ĵ���� �������� ����. setcanvas �ᵵ �ȴ�.)
    // [TODO] : �ش� Ÿ�Կ� ���� �ػ� ���� ������ ��.
    public class UI_GameScene : MonoBehaviour
    {
        // GameScene �ȿ� ���� UI ������ ���� �־��ٰ�.
        [SerializeField] List<GameObject> Prefabs = new List<GameObject>();

        private void Awake()
        {
            foreach(GameObject prefab in Prefabs)
            {
                ObjectManager.Instance.Instantiate(prefab, transform);
            }
        }

    }
}