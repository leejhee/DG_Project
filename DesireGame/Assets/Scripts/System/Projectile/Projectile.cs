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

        // 3���� ����� ��� ����� ���� �ʹ� ���� ��Ӻ��� ���� ���� ä���ϱ�� ��
        private ITargettingStrategy targettingStrategy; // Ÿ����? ��Ÿ����?
        private IPathStrategy       pathStrategy;       // ��� �̵� ���
        private IRangeStrategy      rangeStrategy;      // ��ġ ���� �� ���� ó��
        
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

        // �ܺο��� ��ȯ�� �� ȣ������. 
        public virtual void InitProjectile(StatPackParameter param)
        {
            _caster = param.skillCaster;
            _target = param.skillTarget;
            if(_projectileData == null)
            {
                Debug.LogError("�� ������ ���°Ŷ�µ���? �ε��� ����� Ȯ�ιٶ�");
                return;
            }
            _projectileDamageInput =
                (param.percent / SystemConst.PER_CENT) * _caster.CharStat.GetStat(param.statOperand);

            InitProjectileStrategy();
        }

        // [TODO] : ��� �����ѹ��� ��µ�, ���ǵ��� ��쿡�� ��Ŀ���� ���� ��ŷ������ �����غ���.
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
            ///// ����� ��������.
            target.CharStat.DamageHealth((int)_projectileDamageInput);
            Debug.Log(target.CharStat.GetStat(SystemEnum.eStats.NHP));

            ///// Function ���� ��� : ���� �����Ƿ� ��ó�� ��.
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