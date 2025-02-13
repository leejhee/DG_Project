using System.Collections.Generic;

namespace Client
{
    public class SynergyManager : Singleton<SynergyManager>
    {
        private Dictionary<SystemEnum.eSynergy, List<CharBase>> _synergyActivator;

        #region 생성자
        private SynergyManager() { }
        #endregion

        public override void Init()
        {
            base.Init();

        }

        public void RegisterCharSynergy()
        {

        }

        public void DeleteCharSynergy()
        {

        }

    }

    public class Synergy
    {
        // 시너지의 정보를 담은 데이터(활성화 단계 조건 필요, 활성화 시 효과 필요. 효과가 Function으로 가야한다.)
        private SynergyData     _synergyData;
        
        // 시너지가 
        private List<CharBase>  _synergyCharList;

        private List<int> _ActivationStandard;


    }
}