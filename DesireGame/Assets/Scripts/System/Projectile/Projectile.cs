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
        public virtual void InitProjectile(StatPackedSkillParameter param)
        {
            _caster = param.skillCaster;
            _target = param.skillTargets[param.TargetIndex];
            if(_projectileData == null)
            {
                Debug.LogError("�� ������ ���°Ŷ�µ���? �ε��� ����� Ȯ�ιٶ�");
                return;
            }
            _projectileDamageInput =
                (param.Percent / SystemConst.PER_CENT) * _caster.CharStat.GetStat(param.StatOperand);

            targettingStrategy = TargetStrategyFactory.CreateTargetStrategy
                (new TargettingStrategyParameter()
                {
                    type = param.skillTargetType,
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
            if (_caster == null || _target == null)
                Destroy(gameObject);
            targettingStrategy.CheckTargetPoint(this);
            pathStrategy.UpdatePosition(this);
        }

        public void ApplyEffect(CharBase target)
        {
            ///// ����� ��������.
            target.CharStat.ReceiveDamage(_caster.CharStat.SendDamage(_projectileDamageInput));
            Debug.Log(target.CharStat.GetStat(SystemEnum.eStats.NHP));

            var functionData = DataManager.Instance.GetData<FunctionData>(_projectileData.funcIndex);

            if(functionData is not null)
            {
                var ApplyFunction = FunctionFactory.FunctionGenerate(new BuffParameter()
                {
                    eFunctionType = functionData.function,
                    CastChar = Caster,
                    TargetChar = target,
                    FunctionIndex = functionData.Index
                });
                ApplyFunction.RunFunction();
            }            
            Destroy(gameObject);
        }

    }


}