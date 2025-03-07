using UnityEngine;

namespace Client
{
    /// <summary>
    /// Execution 베이스 시스템
    /// </summary>
    public abstract class FunctionBase
    {
        protected CharBase _TargetChar = null; // 기능 타겟
        protected CharBase _CastChar = null; // 기능 캐스팅 캐릭터
        protected FunctionData _FunctionData = null; // 기능 데이터

        protected float _StartTime = 0; // 시작 시간
        protected float _RunTime = 0; // 현재 시간
        protected float _LifeTime = -1; // 라이프타임 -1 은 무한 지속

        public SystemEnum.eFunction functionType;

        // 버프 생성자
        public FunctionBase(BuffParameter buffParam)
        {
            _TargetChar = buffParam.TargetChar;
            _CastChar = buffParam.CastChar;
            _FunctionData = DataManager.Instance.GetData<FunctionData>(buffParam.FunctionIndex);

            if (_FunctionData == null)
            {
                Debug.LogError($"Execution : {buffParam.FunctionIndex} 데이터 획득 실패");
            }

            // 데이터가 null일 때 프로퍼티로 했다가 피보면 안되기 때문에 생성자에서 초기화.
            functionType = buffParam.eFunctionType;

            _LifeTime = _FunctionData.time > 0 ? 
                _FunctionData.time / SystemConst.PER_THOUSAND : _FunctionData.time;
        }

        public virtual void InitFunction() => _StartTime = Time.time;


        /// <summary>
        /// 버프 시작과 종료
        /// </summary>
        /// <param name="StartFunction"> true: 행동 시작 false 행동 종료 </param>
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
        /// 기능 시간 완료 체크
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