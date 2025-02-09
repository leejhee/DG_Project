using UnityEngine;

namespace Client
{
    /// <summary>
    /// Execution ���̽� �ý���
    /// </summary>
    public abstract class FunctionBase
    {
        protected CharBase _TargetChar = null; // ��� Ÿ��
        protected CharBase _CastChar = null; // ��� ĳ���� ĳ����
        protected FunctionData _FunctionData = null; // ��� ������

        protected float _StartTime = 0; // ���� �ð�
        protected float _RunTime = 0; // ���� �ð�
        protected float _LifeTime = -1; // ������Ÿ�� -1 �� ���� ����

        // ���� ������
        public FunctionBase(BuffParameter buffParam)
        {
            _TargetChar = buffParam.TargetChar;
            _CastChar = buffParam.CastChar;
            _FunctionData = DataManager.Instance.GetData<FunctionData>(buffParam.FunctionIndex);

            if (_FunctionData == null)
            {
                Debug.LogError($"Execution : {buffParam.FunctionIndex} ������ ȹ�� ����");
            }
        }

        /// <summary>
        /// ���� ���۰� ����
        /// </summary>
        /// <param name="StartFunction"> true: �ൿ ���� false �ൿ ���� </param>
        public virtual void RunFunction(bool StartFunction)
        {
            if (StartFunction)
            {
                if (!_TargetChar.FunctionBaseDic[_FunctionData.function].Contains(this))
                {
                    _StartTime = Time.time;
                    _TargetChar.FunctionBaseDic[_FunctionData.function].Add(this);
                }
            }
            else
            {
                if (_TargetChar.FunctionBaseDic[_FunctionData.function].Contains(this))
                {
                    _TargetChar.FunctionBaseDic[_FunctionData.function].Remove(this);
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
            float executionTime = _FunctionData.time / SystemConst.PER_THOUSAND;
            if (runTime > executionTime)
            {

                RunFunction(false);
            }

        }

    }
}