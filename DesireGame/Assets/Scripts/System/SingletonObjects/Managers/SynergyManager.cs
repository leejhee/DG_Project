using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class SynergyManager : Singleton<SynergyManager>
    {
        private Dictionary<eSynergy, SynergyContainer> _synergyActivator;
        public ReadOnlyCollection<CharLightWeightInfo> GetInfo(eSynergy synergy)
        {
            if(_synergyActivator.ContainsKey(synergy))
                return _synergyActivator[synergy].SynergyMembers;
            return null;
        }
        
        #region 생성자
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

        /// <summary> 캐릭터 단위 시너지 등록 </summary>
        /// <param name="registrar"> 가입자 정보 </param>
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
                _synergyActivator[other].GuestRegister(registrar);
            }

        }
        

        public void DeleteCharSynergy(CharLightWeightInfo leaver)
        {
            var mySynergies = leaver.synergyList;
            var otherSynergies = _synergyActivator.Keys.Except(mySynergies).ToList();

            foreach (var synergy in leaver.synergyList)
            {
                DeleteSynergy(leaver, synergy);
            }

            foreach (var other in otherSynergies)
            {
                _synergyActivator[other].GuestDelete(leaver);
            }

        }


        /// <summary> 시너지 등록 단위 </summary>
        /// <param name="registrar"> 가입자 정보 </param>
        /// <param name="synergy"> 가입할 시너지 </param>
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
        // 씬상 테스트만 하는 용도
        public void ShowCurrentSynergies()
        {
            StringBuilder view = new("현재 시너지\n");
            foreach(var value in _synergyActivator.Values)
            {
                view.AppendLine(value.ToString());
            }
            Debug.Log(view.ToString());
        }
        #endregion

    }

}