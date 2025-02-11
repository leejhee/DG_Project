using UnityEngine;

namespace Client
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private long            _index;
        
        private Collider        _projectileCollider;
        private Transform       _projectileTransform;
        private FunctionInfo    _functionInfo;
        private CharBase        _caster;
        private CharBase        _target;

        protected ProjectileData _projectileData;
        protected float _projectileDamageInput;

        // 3가지 고려할 경우 경우의 수가 너무 많아 상속보단 전략 패턴 채용하기로 함
        private ITargettingStrategy targettingStrategy; // 타겟팅? 논타겟팅?
        private IPathStrategy       pathStrategy;       // 경로 이동 방식
        private IRangeStrategy      rangeStrategy;      // 위치 도달 후 범위 처리
        
        public Collider Collider => _projectileCollider;
        public Transform ProjectileTransform => _projectileTransform;
        public FunctionInfo FunctionInfo => _functionInfo;
        public CharBase Caster => _caster;
        public CharBase Target => _target;
        public ITargettingStrategy TargetGuide => targettingStrategy;
        public IPathStrategy PathGuide => pathStrategy;
        public IRangeStrategy RangeGuide => rangeStrategy;



        private void Awake()
        {
            _projectileTransform = transform;
            _projectileCollider = GetComponent<Collider>();
            _projectileData = DataManager.Instance.GetData<ProjectileData>(_index);
            _functionInfo = new();
            _functionInfo.Init();
        }

        // 외부에서 소환할 때 호출하자. 
        public virtual void InitProjectile(StatPackParameter param)
        {
            _caster = param.skillCaster;
            _target = param.skillTarget;
            if(_projectileData == null)
            {
                Debug.LogError("이 데이터 없는거라는데요? 인덱스 제대로 확인바람");
                return;
            }
            _projectileDamageInput =
                (param.percent / SystemConst.PER_CENT) * _caster.CharStat.GetStat(param.statOperand);

            InitProjectileStrategy();
        }

        // [TODO] : 잠시 매직넘버를 썼는데, 스피드의 경우에는 마커에서 같이 패킹해줄지 생각해보자.
        public void InitProjectileStrategy()
        {
            targettingStrategy = TargetStrategyFactory.CreateTargetStrategy
                (new TargettingStrategyParameter()
                {
                    type = _projectileData.target,
                    Target = _target
                });
            pathStrategy = PathStrategyFactory.CreatePathStrategy
                (new PathStrategyParameter()
                {
                    type = _projectileData.path,
                    Speed = 2f,
                });
            
        }
        private void FixedUpdate()
        {
            targettingStrategy.CheckTargetPoint(this);
            pathStrategy.UpdatePosition(this);
        }

        public void ApplyEffect(CharBase target)
        {
            ///// 대미지 적용하자.
            target.CharStat.DamageHealth((int)_projectileDamageInput);
            Debug.Log(target.CharStat.GetStat(SystemEnum.eStats.NHP));

            ///// Function 있을 경우 : 오류 있으므로 대처할 것.
            var functionData = DataManager.Instance.GetData<FunctionData>(_projectileData.funcIndex);

            if(!(functionData is null))
            {
                var ApplyFunction = FunctionFactory.FunctionGenerate(new BuffParameter()
                {
                    eFunctionType = functionData.function,
                    CastChar = Caster,
                    TargetChar = target,
                    FunctionIndex = functionData.Index
                });
                Target.FunctionBaseDic[functionData.function].Add(ApplyFunction);
            }            
            Destroy(gameObject);
        }

    }


}