using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace Client
{
    public class SkillMarkerShotProjectal : SkillTimeLineMarker
    {
        [Header("발사할 투사체")]
        [SerializeField] protected ProjectileBase _projectile;
        [SerializeField] protected Vector3 _offSet;
        
        //만약 이걸 쓴다면 이 데이터들을 전송해줄 게 따로 필요하다.
        [Header("투사체에 반영할 시전자의 스탯과 비율")]
        [SerializeField] protected SystemEnum.eStats _statName;
        [SerializeField] protected float percent;

        public override void MarkerAction()
        {
            Debug.Log("시작!");
           
            ProjectileBase projectile = Instantiate(_projectile);
            projectile.InitProjectile(inputParam.ToStatPack(_statName, percent));        
        }

        // 이건 에디터 상에서 호출되는 함수. 런타임 작업 아니면 건들지 맙시다.
        public override void SkillInitialize()
        {
            Debug.Log("시작!");
        }
    }

}