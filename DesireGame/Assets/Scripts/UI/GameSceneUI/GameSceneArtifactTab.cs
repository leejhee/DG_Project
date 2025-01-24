using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Client
{
    public class GameSceneArtifactTab : MonoBehaviour
    {
        [SerializeField] Transform ArtifactPanel;

        private void Awake()
        {
            UIManager.Instance.SetCanvas(gameObject, false);
        }

        private void Start()
        {
            SetArtifact();
        }

        void SetArtifact()
        {
            // ��Ƽ��Ʈ ����Ʈ�� ��ȭ ���� �� ȣ��,
            // UI ��� ������Ʈ.
        }
    }
}