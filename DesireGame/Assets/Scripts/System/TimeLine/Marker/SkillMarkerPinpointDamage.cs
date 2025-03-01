using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Client
{
    /// <summary>
    /// ��ų Ÿ�Ӷ��ο� ��Ŀ
    /// </summary>
    public class SkillMarkerPinpointDamage : SkillTimeLineMarker
    {

        [Header("����ü�� �ݿ��� �������� ���Ȱ� ����")]
        [SerializeField] protected SystemEnum.eStats _statName;
        [SerializeField] protected float percent;

        public override void MarkerAction()
        {
            if(_statName == default || percent == default)
            {
                Debug.Log("������ �������� �ʾҳ���? Ȯ���� �ּ���.");
                return;
            }

            // ������Ű�� ���ο��� ����ϵ��� �ұ�...?
            for (int idx = 0; idx < skillParam.skillTargets.Count; idx++)
            {
                var casterStat = skillParam.skillCaster.CharStat;
                var target = skillParam.skillTargets[idx];
                if (target == null || casterStat is null) continue;
                var calculated = casterStat.GetStat(_statName) * 
                                (percent / SystemConst.PER_CENT);

                DamageParameter param = casterStat.SendDamage(calculated, casterStat.DamageType);
                target.CharStat.ReceiveDamage(param);
            }
        }

        // �̰� ������ �󿡼� ȣ��Ǵ� �Լ�. ��Ÿ�� �۾� �ƴϸ� �ǵ��� ���ô�.
        public override void SkillInitialize()
        {

        }

    }



}