using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace Client
{
    public class SkillMarkerShotProjectal : SkillTimeLineMarker
    {
        [Header("발사할 투사체")]
        [SerializeField] protected Projectile _projectile;
        [SerializeField] protected Vector3 _offSet;
        
        //만약 이걸 쓴다면 이 데이터들을 전송해줄 게 따로 필요하다.
        [Header("투사체에 반영할 시전자의 스탯과 비율")]
        [SerializeField] protected SystemEnum.eStats _statName;
        [SerializeField] protected float percent;

        [SerializeField] protected List<long> indices;
        
        public override void MarkerAction()
        {                      
            // 현재는 그냥 한번에 빵 쏘는거 한다.
            for(int idx = 0; idx < skillParam.skillTargets.Count; idx++)
            {
                Projectile projectile = GameObject.Instantiate(_projectile, skillParam.skillCaster.transform.position, Quaternion.identity);
                projectile.InitProjectile(skillParam.ToStatPack(_statName, percent, idx));
                projectile.InjectProjectileFunction(indices);            
            }
        }

        // 이건 에디터 상에서 호출되는 함수. 런타임 작업 아니면 건들지 맙시다.
        public override void SkillInitialize()
        {

        }
    }

}