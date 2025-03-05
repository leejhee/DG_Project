using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace Client
{
    public class SkillMarkerShotProjectal : SkillTimeLineMarker
    {
        [Header("�߻��� ����ü")]
        [SerializeField] protected Projectile _projectile;
        [SerializeField] protected Vector3 _offSet;
        
        //���� �̰� ���ٸ� �� �����͵��� �������� �� ���� �ʿ��ϴ�.
        [Header("����ü�� �ݿ��� �������� ���Ȱ� ����")]
        [SerializeField] protected SystemEnum.eStats _statName;
        [SerializeField] protected float percent;

        [SerializeField] protected List<long> indices;
        
        public override void MarkerAction()
        {                      
            // ����� �׳� �ѹ��� �� ��°� �Ѵ�.
            for(int idx = 0; idx < skillParam.skillTargets.Count; idx++)
            {
                Projectile projectile = GameObject.Instantiate(_projectile, skillParam.skillCaster.transform.position, Quaternion.identity);
                projectile.InitProjectile(skillParam.ToStatPack(_statName, percent, idx));
            }
        }

        // �̰� ������ �󿡼� ȣ��Ǵ� �Լ�. ��Ÿ�� �۾� �ƴϸ� �ǵ��� ���ô�.
        public override void SkillInitialize()
        {

        }
    }

}