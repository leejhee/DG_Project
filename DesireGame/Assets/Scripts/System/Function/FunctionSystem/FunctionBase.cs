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

        public SystemEnum.eFunction functionType;

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

            // �����Ͱ� null�� �� ������Ƽ�� �ߴٰ� �Ǻ��� �ȵǱ� ������ �����ڿ��� �ʱ�ȭ.
            functionType = buffParam.eFunctionType;

            _LifeTime = _FunctionData.time > 0 ? 
                _FunctionData.time / SystemConst.PER_THOUSAND : _FunctionData.time;
        }

        public virtual void InitFunction() => _StartTime = Time.time;


        /// <summary>
        /// ���� ���۰� ����
        /// </summary>
        /// <param name="StartFunction"> true: �ൿ ���� false �ൿ ���� </param>
        public virtual void RunFunction(bool StartFunction = true)
        {
            if (StartFunction)
            {
            }
            else
            {
            }
        }

        public virtual void Update(float delta) { }

        /// <summary>
        /// ��� �ð� �Ϸ� üũ
        /// </summary>
        public void CheckTimeOver()
        {
            if (_LifeTime == -1f) return;

            float runTime = Time.time - _StartTime;
            if (runTime > _LifeTime || _LifeTime == 0f)
            {
                _TargetChar.FunctionInfo.KillFunction(this);
            }

        }

    }
}