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
        }

        /// <summary>
        /// 버프 시작과 종료
        /// </summary>
        /// <param name="StartFunction"> true: 행동 시작 false 행동 종료 </param>
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
        /// 기능 시간 완료 체크
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