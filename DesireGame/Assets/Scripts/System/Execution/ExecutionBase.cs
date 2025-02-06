using UnityEngine;

namespace Client
{
    /// <summary>
    /// Execution ���̽� �ý���
    /// </summary>
    public abstract class ExecutionBase
    {
        protected CharBase _TargetChar = null; // ��� Ÿ��
        protected CharBase _CastChar = null; // ��� ĳ���� ĳ����
        protected ExecutionData _ExecutionData = null; // ��� ������

        protected float _StartTime = 0; // ���� �ð�
        protected float _RunTime = 0; // ���� �ð�
        protected float _LifeTime = -1; // ������Ÿ�� -1 �� ���� ����

        // ���� ������
        public ExecutionBase(BuffParameter buffParam)
        {
            _TargetChar = buffParam.TargetChar;
            _CastChar = buffParam.CastChar;
            _ExecutionData = DataManager.Instance.GetData<ExecutionData>(buffParam.ExecutionIndex);

            if (_ExecutionData == null)
            {
                Debug.LogError($"Execution : {buffParam.ExecutionIndex} ������ ȹ�� ����");
            }
        }

        /// <summary>
        /// ���� ���۰� ����
        /// </summary>
        /// <param name="StartExecution"> true: �ൿ ���� false �ൿ ���� </param>
        public virtual void RunExecution(bool StartExecution)
        {
            if (StartExecution)
            {
                if (!_TargetChar.ExecutionBaseDic[_ExecutionData.functionType].Contains(this))
                {
                    _StartTime = Time.time;
                    _TargetChar.ExecutionBaseDic[_ExecutionData.functionType].Add(this);
                }
            }
            else
            {
                if (_TargetChar.ExecutionBaseDic[_ExecutionData.functionType].Contains(this))
                {
                    _TargetChar.ExecutionBaseDic[_ExecutionData.functionType].Remove(this);
                }
            }
        }

        public virtual void Update(float delta) { }

        /// <summary>
        /// ��� �ð� �Ϸ� üũ
        /// </summary>
        public void CheckTimeOver()
        {
            float runTime = Time.time - _StartTime;
            float executionTime = _ExecutionData.duration / SystemConst.PER_THOUSAND;
            if (runTime > executionTime)
            {

                RunExecution(false);
            }

        }

    }
}