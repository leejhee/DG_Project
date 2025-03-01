using UnityEngine;

namespace Client
{
    // ��ü ���ۻ� ���� ��� Ÿ�̹��� ���� �־ ��Ŀ�� ó����
    public class SkillMarkerManaExchanger : SkillTimeLineMarker
    {
        private enum choice { Earn, Pay }

        [SerializeField] 
        choice Choice;

        [SerializeField, Header("Pay�� ���, �׳� �μŵ� �˴ϴ�. �˾Ƽ� ����")] 
        int amount = 5;

        public override void MarkerAction()
        {
            base.MarkerAction();
           
            var stat = skillParam.skillCaster.CharStat;
            if (Choice == choice.Earn)
            {
                stat.GainMana(amount, true);
                Debug.Log(@$"{skillParam.skillCaster.GetID()}�� ĳ���Ϳ��� ��Ÿ�� {amount}���� ȹ��
���� ���� {stat.GetStat(SystemEnum.eStats.N_MANA)}");
            }
            else if(Choice == choice.Pay)
            {
                amount = (int)stat.GetStat(SystemEnum.eStats.MAX_MANA);
                stat.GainMana(amount, false);
                Debug.Log(@$"{skillParam.skillCaster.GetID()}�� ĳ���Ϳ��� ��ų�� {amount}���� �Һ�
���� ���� {stat.GetStat(SystemEnum.eStats.N_MANA)}");
            }

        }

    }
}