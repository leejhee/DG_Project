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
        
        public override void MarkerAction()
        {           
            Projectile projectile = GameObject.Instantiate(_projectile, inputParam.skillCaster.transform.position, Quaternion.identity);
            // ����� �׳� �ѹ��� �� ��°� �Ѵ�.
            // projectile�� ���ٴ� ������ ������ ���� ��ȣ ������ ������ �ָ� index �����ؼ� ��� �ϴ� ��...�?
            for(int idx = 0; idx < inputParam.skillTargets.Count; idx++)
                projectile.InitProjectile(inputParam.ToStatPack(_statName, percent, idx));        
        }

        // �̰� ������ �󿡼� ȣ��Ǵ� �Լ�. ��Ÿ�� �۾� �ƴϸ� �ǵ��� ���ô�.
        public override void SkillInitialize()
        {

        }
    }

}