using UnityEngine;

namespace Client
{
    /// <summary>
    /// Execution 베이스 시스템
    /// </summary>
    public abstract class ExecutionBase
    {
        protected CharBase _TargetChar = null; // 기능 타겟
        protected CharBase _CastChar = null; // 기능 캐스팅 캐릭터
        protected ExecutionData _ExecutionData = null; // 기능 데이터

        protected float _StartTime = 0; // 시작 시간
        protected float _RunTime = 0; // 현재 시간
        protected float _LifeTime = -1; // 라이프타임 -1 은 무한 지속

        // 버프 생성자
        public ExecutionBase(BuffParameter buffParam)
        {
            _TargetChar = buffParam.TargetChar;
            _CastChar = buffParam.CastChar;
            _ExecutionData = DataManager.Instance.GetData<ExecutionData>(buffParam.ExecutionIndex);

            if (_ExecutionData == null)
            {
                Debug.LogError($"Execution : {buffParam.ExecutionIndex} 데이터 획득 실패");
            }
        }

        /// <summary>
        /// 버프 시작과 종료
        /// </summary>
        /// <param name="StartExecution"> true: 행동 시작 false 행동 종료 </param>
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
        /// 기능 시간 완료 체크
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