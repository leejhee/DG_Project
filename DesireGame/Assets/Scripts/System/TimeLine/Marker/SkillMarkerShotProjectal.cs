using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace Client
{
    public class SkillMarkerShotProjectal : SkillTimeLineMarker
    {
        [Header("�߻��� ����ü")]
        [SerializeField] protected CharProjectal _projectal;
        [SerializeField] protected Vector3 _offSet;

        public override void MarkerAction()
        {
            Debug.Log("����!");

            CharProjectal projectal = GameObject.Instantiate<CharProjectal>(_projectal);

        }
        public override void SkillInitialize()
        {


            Debug.Log("����!");
        }
    }

}