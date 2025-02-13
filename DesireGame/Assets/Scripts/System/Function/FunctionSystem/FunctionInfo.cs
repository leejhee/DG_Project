using static Client.SystemEnum;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class FunctionInfo
    {
        private Dictionary<eFunction, List<FunctionBase>> _functionBaseDic = new(); // ��� 

        private Queue<FunctionBase> _functionReadyQueue = new();

        private Queue<FunctionBase> _functionKillQueue = new();

        public void Init()
        {
            for (eFunction i = 0; i < eFunction.MaxCount; i++)
            {
                _functionBaseDic[i] = new List<FunctionBase>();
            }
        }

        public void UpdateFunctionDic()
        {          
            // �غ� ť���� ��ųʸ��� �߰�
            while(_functionReadyQueue.Count != 0)
            {
                FunctionBase target = _functionReadyQueue.Dequeue();
                if (!_functionBaseDic[target.functionType].Contains(target))
                {
                    target.InitFunction();
                    _functionBaseDic[target.functionType].Add(target);
                }
                    
            }

            foreach (var functionBaseList in _functionBaseDic)
            {
                foreach (var function in functionBaseList.Value)
                {
                    function.CheckTimeOver();
                    function.Update(Time.deltaTime);
                }
            }

            // ��ȸ �� ���� ť�� ������ Ÿ�� ��ųʸ����� ���� 
            while (_functionKillQueue.Count != 0)
            {
                FunctionBase target = _functionKillQueue.Dequeue();
                if (_functionBaseDic[target.functionType].Contains(target))
                    _functionBaseDic[target.functionType].Remove(target);
            }
        }

        // Function Dictionary���� ���� ����.
        public void EnqueueFunction(FunctionBase target)
        {
            _functionReadyQueue.Enqueue(target);
        }

        public void EnqueueKill(FunctionBase target)
        {
            _functionKillQueue.Enqueue(target);
        }


    }
}