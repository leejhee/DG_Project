using System;
using UnityEngine;

namespace Client
{
    public class GameSceneSynergyTab : MonoBehaviour
    {
        //TODO : Depth 관리할지 선택하기
        [SerializeField] private GameObject synergyUnitPrefab;
        [SerializeField] private Transform teamAGridTransform;
        [SerializeField] private Transform teamBGridTransform;
        private void Awake()
        {
            MessageManager.SubscribeMessage<OnSynergyChange>(this, SynergyUpdate);
        }

        private void SynergyUpdate(OnSynergyChange change)
        {
            throw new NotImplementedException();
        }
    }
}