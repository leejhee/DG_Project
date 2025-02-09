using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace Client
{
    public class SkillMarkerShotProjectal : SkillTimeLineMarker
    {
        [Header("�߻��� ����ü")]
        [SerializeField] protected ProjectileBase _projectile;
        [SerializeField] protected Vector3 _offSet;
        
        //���� �̰� ���ٸ� �� �����͵��� �������� �� ���� �ʿ��ϴ�.
        [Header("����ü�� �ݿ��� �������� ���Ȱ� ����")]
        [SerializeField] protected SystemEnum.eStats _statName;
        [SerializeField] protected float percent;

        public override void MarkerAction()
        {
            Debug.Log("����!");
           
            ProjectileBase projectile = Instantiate(_projectile);
            projectile.InitProjectile(inputParam.ToStatPack(_statName, percent));        
        }

        // �̰� ������ �󿡼� ȣ��Ǵ� �Լ�. ��Ÿ�� �۾� �ƴϸ� �ǵ��� ���ô�.
        public override void SkillInitialize()
        {
            Debug.Log("����!");
        }
    }

}