using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace Client
{
    public class SkillMarkerShotProjectal : SkillTimeLineMarker
    {
        [Header("�߻��� ����ü")]
        [SerializeField] protected CharProjectile _projectile;
        [SerializeField] protected Vector3 _offSet;


        public override void MarkerAction()
        {
            Debug.Log("����!");

            
            CharProjectile projectile = GameObject.Instantiate<CharProjectile>(_projectile);
            
        }

        public override void SkillInitialize()
        {


            Debug.Log("����!");
        }
    }

}