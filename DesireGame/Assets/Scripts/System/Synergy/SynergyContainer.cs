using static Client.SystemEnum;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace Client
{
    public class SynergyContainer
    {
        private eSynergy mySynergy;

        private List<CharLightWeightInfo> _synergyMembers;

        private List<CharLightWeightInfo> _buffReceiverSignature;

        #region ������
        public SynergyContainer(eSynergy synergy)
        {
            _synergyMembers = new();
            _buffReceiverSignature = new();
            mySynergy = synergy;
        }

        public override string ToString()
        {
            return $"{mySynergy} �ó���, Members : {_synergyMembers.Count}, DistinctMembers : {DistinctMembers}";
        }

        #endregion

        #region Synergy Member Control

        public int MemberCount => _synergyMembers.Count;

        // �ش� �ó����� ���ԵǴ� ����� ��ϵ� �� ȣ��ȴ�.
        public void Register(CharLightWeightInfo registrar)
        {
            if(!_synergyMembers.Contains(registrar))
                _synergyMembers.Add(registrar);

            if (CheckSynergyChange())
                SetCurrentSynergy();
            else
                GetCurrentSynergyBuff();
        }

        // �ش� �ó����� ���ԵǴ� ����� Ż���� �� ȣ��ȴ�.
        public void Delete(CharLightWeightInfo leaver)
        {
            if (_synergyMembers.Contains(leaver))
                _synergyMembers.Remove(leaver);

            if (CheckSynergyChange())
                SetCurrentSynergy();
        }

        #endregion

        #region Synergy Data Getter
        private List<SynergyData> _currentSynergyBuff = new();
        public int DistinctMembers
        {
            get
            {
                var distinctList = _synergyMembers
                    .GroupBy(member => member.index)
                    .Select(g => g.First())
                    .ToList();
                return distinctList.Count;
            }
        }

        public List<SynergyData> GetSynergyByLevel()
        {
            var synergyInfo = DataManager.Instance.SynergyDataMap[mySynergy];
            var sortedKeys = new List<int>(synergyInfo.Keys);
            sortedKeys.Sort();

            int nowDistinct = DistinctMembers;

            // ������ 1�� �ó����� �ó��� �������� �ݵ�� �����ؾ� ��.
            // ������ ������ 1¥���� �ȸ�������. �״ϱ� Ȱ��ȭ�� ��������.
            int levelThreshold = 1;
            foreach (var threshold in sortedKeys)
            {
                if (nowDistinct < threshold)
                {
                    break;
                }
                levelThreshold = threshold;
            }

            if (!synergyInfo.ContainsKey(levelThreshold)) 
                return new List<SynergyData>();
            return synergyInfo[levelThreshold];
        }
        #endregion

        #region Synergy Buff Managing


        public Action OnSynergyChanges = null;
        
        // �ó����� ���ŵǾ�� �ϴ����� ���� üũ �Լ�
        public bool CheckSynergyChange()
        {
            var newSynergy = GetSynergyByLevel();
            if (_currentSynergyBuff == newSynergy)
                return false;
            else
            {
                _currentSynergyBuff = newSynergy;
                return true;
            }
        }

        // �ó��� ���� ��� �Լ�
        public void GetCurrentSynergyBuff()
        {

        }

        // �ó��� ���� �Լ�
        public void SetCurrentSynergy()
        {
            var newSynergyData = GetSynergyByLevel();
            if(newSynergyData != null)
            {                
                OnSynergyChanges?.Invoke();
                OnSynergyChanges = null;
                _currentSynergyBuff = newSynergyData;

                List<CharBase> casters = new();
                
                foreach(var synergyData in _currentSynergyBuff)
                {
                    // eSynergyRange�� ����, Caster�� ������ �Ʊ��� ������ �����Ѵ�.
                    if (synergyData.casterType == eSynergyRange.SELF)
                    {
                        foreach (var info in _synergyMembers)
                            casters.Add(info.SpecifyCharBase());                       
                    }
                    else if (synergyData.casterType == eSynergyRange.GLOBAL_ALLY)
                    {
                        casters = CharManager.Instance.GetOneSide(eCharType.ALLY);
                    }
                    else if(synergyData.casterType == eSynergyRange.GLOBAL_ENEMY)
                    {
                        casters = CharManager.Instance.GetOneSide(eCharType.ENEMY);
                    }

                    Debug.Log($"�ó��� ������ �����Ǿ����ϴ�. " +
                    $"Synergy {mySynergy}���� {DistinctMembers}�� �޼��Ͽ� {synergyData.Index}�� ���� " +
                    $"casters {casters.Count}���� ��");

                    foreach(var caster in casters)
                    {
                        Debug.Log(caster.name);
                    }

                    // ĳ���� ��ȸ�Ͽ� ���� caster�� ������ function�� ����� add�Ѵ�.
                    foreach (var caster in casters)
                    {
                        var funcData = DataManager.Instance.GetData<FunctionData>(synergyData.functionIndex);
                        if (funcData == null)
                        {
                            Debug.LogError("�߸� �ƴ��ݳ� �̳༮�� ������ Ȯ�� �ٽ��غ���.");
                            continue;
                        }

                        if (FunctionFactory.FunctionGenerate(new BuffParameter()
                        {
                            CastChar = caster,
                            TargetChar = null,
                            FunctionIndex = funcData.Index,
                            eFunctionType = funcData.function
                        }) is not SynergyFunction synergyFunc)
                        {
                            Debug.LogError("�� �ó��� ���� �ٸ� ��� �������� ���� ���ɷȾ�!");
                            continue;
                        }

                        // eSkillTargetType�� ����, Target���� ������ ������ ������ �����Ѵ�.
                        // �ó��� ���� ��� ���� �Ҵ�� function�� ��ü�Ǳ� ���� ������ �Ѵ�.
                        synergyFunc.DelayedInit(synergyData.skillTarget, OnSynergyChanges);
                        caster.FunctionInfo.AddFunction(synergyFunc);
                        Debug.Log($"{caster.GetID()}�� ĳ���� {caster.name}�� synergy Function {funcData.Index}�� function ����. " +
                                    $"��� : {funcData.function}");
                    }
                    


                }
            }
        }

        #endregion

    }
}