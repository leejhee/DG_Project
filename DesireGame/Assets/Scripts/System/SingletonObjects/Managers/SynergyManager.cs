using System.Collections.Generic;

namespace Client
{
    public class SynergyManager : Singleton<SynergyManager>
    {
        private Dictionary<SystemEnum.eSynergy, List<CharBase>> _synergyActivator;

        #region ������
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
        // �ó����� ������ ���� ������(Ȱ��ȭ �ܰ� ���� �ʿ�, Ȱ��ȭ �� ȿ�� �ʿ�. ȿ���� Function���� �����Ѵ�.)
        private SynergyData     _synergyData;
        
        // �ó����� 
        private List<CharBase>  _synergyCharList;

        private List<int> _ActivationStandard;


    }
}