using static Client.SystemEnum;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using System.Collections.ObjectModel;

namespace Client
{
    public class SynergyContainer
    {
        private readonly eSynergy mySynergy;

        private List<CharLightWeightInfo> _synergyMembers;
        public ReadOnlyCollection<CharLightWeightInfo> SynergyMembers => _synergyMembers.AsReadOnly();

        #region ������
        public SynergyContainer(eSynergy synergy)
        {
            _synergyMembers = new();
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
                GetCurrentSynergyBuff(registrar);
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

        private Queue<SynergyBuffRecord> synergyBuffRecords = new();
        
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

        // ���� ����Ǵ� �۷ι� �ó��� ������ ��� �Լ�
        public void GetCurrentSynergyBuff(CharLightWeightInfo receiver)
        {
            var caster = receiver.SpecifyCharBase();
            if (caster == null) return;

            foreach(var synergyData in _currentSynergyBuff)
            {
                if(synergyData.casterType == eSynergyRange.GLOBAL_ALLY)
                {
                    GetBuff(caster, synergyData);
                }
            }

        }

        // �ó��� ���� ��� �Լ�
        public void GetBuff(CharBase caster, SynergyData data)
        {
            #region GETTING PARAMETERS
            var funcData = DataManager.Instance.GetData<FunctionData>(data.functionIndex);
            if (funcData == null)
            {
                Debug.LogError("�߸� �ƴ��ݳ� �̳༮�� ������ Ȯ�� �ٽ��غ���.");
                return;
            }
            
            var IntendedTargets = TargetStrategyFactory.CreateTargetStrategy
                (new TargettingStrategyParameter() { Caster = caster, type = data.skillTarget }).GetTargets();

            CharBase target = null;
            if (IntendedTargets.Count == 0)
                Debug.Log($"���� Ÿ���� Ÿ���� Ÿ�� {data.skillTarget}�� ���� �������� �ʽ��ϴ�.");
            else
                target = IntendedTargets[0];
            #endregion

            #region ADD BUFF AND RECORD
            //////////Add Buff///////////
            var synergyFunction = FunctionFactory.FunctionGenerate(new BuffParameter()
            {
                CastChar = caster,
                TargetChar = target,
                eFunctionType = funcData.function,
                FunctionIndex = funcData.Index
            });

            //TODO : FunctionInfo�� '���� ����'�� ���� ���� öȸ�� ��. �ó��������̳ʿ��� å�� ����.
            //TODO : SynergyContainer���� Distributer�� ��ü �и����� �����غ� ��
            caster.FunctionInfo.AddFunction(synergyFunction, data.buffTriggerTime);
            Debug.Log($"{caster.GetID()}�� ĳ���� {caster.name}�� synergy Function {funcData.Index}�� function ����. " +
                                    $"��� : {funcData.function}");
            //////////Add Record/////////
            synergyBuffRecords.Enqueue(new SynergyBuffRecord(caster, synergyFunction));
            #endregion
        }

        // �ó��� ���� �Լ�
        public void SetCurrentSynergy()
        {
            // Reset Record
            while(synergyBuffRecords.Count > 0)
            {
                var record = synergyBuffRecords.Dequeue();
                if (record.Caster) // ���� �ȾƼ� ������ Destroy�Ǹ� null�� �Ŷ�
                    record.KillSynergyBuff();
            }

            List<CharBase> casters = new();            
            foreach(var synergyData in _currentSynergyBuff)
            {
                // eSynergyRange�� ����, Caster�� ������ �Ʊ��� ������ �����Ѵ�.
                switch (synergyData.casterType)
                {
                    case eSynergyRange.ONCE:                        
                        break;
                    case eSynergyRange.SELF:
                        foreach (var info in _synergyMembers)
                            casters.Add(info.SpecifyCharBase());
                        break;
                    case eSynergyRange.GLOBAL_ALLY:
                        casters = CharManager.Instance.GetOneSide(eCharType.ALLY);
                        break;
                    case eSynergyRange.GLOBAL_ENEMY:
                        casters = CharManager.Instance.GetOneSide(eCharType.ENEMY);
                        break;
                    default: break;
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
                    GetBuff(caster, synergyData);                       
                }
            }            
        }

        #endregion

    }

    public class SynergyBuffRecord
    {
        public CharBase Caster { get; private set; }
        public FunctionBase BuffFunction { get; private set; }

        public SynergyBuffRecord(CharBase caster, FunctionBase buffFunction)
        {
            Caster = caster;
            BuffFunction = buffFunction;
        }

        public void KillSynergyBuff()
        {
            Caster.FunctionInfo.KillFunction(BuffFunction);
            Debug.Log($"{Caster.name}�� ���� ���� : {BuffFunction.functionType}");
        }

    }


}