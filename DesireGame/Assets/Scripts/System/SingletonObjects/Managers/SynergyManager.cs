using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class SynergyManager : Singleton<SynergyManager>
    {
        private Dictionary<eSynergy, SynergyContainer> _synergyActivator;

        // �ش� ������ �״�� ����.
        //private Dictionary<eSynergy, Action<SynergyParameter>> _OnSynergyChanges;

        #region ������
        private SynergyManager() { }
        #endregion

        public override void Init()
        {
            base.Init();
            _synergyActivator = new();
            //_OnSynergyChanges = new();
        }

        public void Reset() => _synergyActivator.Clear();

        //public void SubscribeToChanges(eSynergy synergy, Action<SynergyParameter> trigging) => _OnSynergyChanges[synergy] += trigging;
        
        //public void UnsubscribeToChanges(eSynergy synergy, Action<SynergyParameter> trigging) => _OnSynergyChanges[synergy] -= trigging;

        /// <summary> ĳ���� ���� �ó��� ��� </summary>
        /// <param name="registrar"> ������ ���� </param>
        public void RegisterCharSynergy(CharLightWeightInfo registrar)
        {
            var mySynergies = registrar.synergyList;
            var otherSynergies = _synergyActivator.Keys.Except(mySynergies).ToList();

            foreach (var synergy in registrar.synergyList)
            {
                RegisterSynergy(registrar, synergy);
            }

            foreach(var other in otherSynergies)
            {
                foreach(var data in _synergyActivator[other].GetSynergyByLevel())
                {

                }
            }

        }

        /// <summary> �ó��� ��� ���� </summary>
        /// <param name="registrar"> ������ ���� </param>
        /// <param name="synergy"> ������ �ó��� </param>
        public void RegisterSynergy(CharLightWeightInfo registrar, eSynergy synergy)
        {
            if (synergy == eSynergy.None) return;

            if (!_synergyActivator.ContainsKey(synergy))
            {
                _synergyActivator.Add(synergy, new SynergyContainer(synergy));
            }
            _synergyActivator[synergy].Register(registrar);
        }

        public void DeleteSynergy(CharLightWeightInfo leaver, eSynergy synergy)
        {           
            if (synergy == eSynergy.None) return;

            if (_synergyActivator.ContainsKey(synergy))
            {
                _synergyActivator[synergy].Delete(leaver);
            }
            
            if (_synergyActivator[synergy].MemberCount == 0)
            {
                _synergyActivator.Remove(synergy);
            }
        }

        #region Test_Method
        // ���� �׽�Ʈ�� �ϴ� �뵵
        public void ShowCurrentSynergies()
        {
            StringBuilder view = new("���� �ó���\n");
            foreach(var value in _synergyActivator.Values)
            {
                view.AppendLine(value.ToString());
            }
            Debug.Log(view.ToString());
        }
        #endregion

    }

}