using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private long            _index;
        
        private Collider        _projectileCollider;
        private Transform       _projectileTransform;
        private CharBase        _caster;
        private CharBase        _target;

        protected ProjectileData _projectileData;
        protected float _projectileDamageInput;
        protected List<FunctionData> _functionDataList;
        protected int _projectileLife; // ����ü�� ���� ��� ��ġ
        
        // ����������.
        private bool destroyFlag = false;
        public void SetDestroyFlag(bool flag) => destroyFlag = flag;



        // 3���� ����� ��� ����� ���� �ʹ� ���� ��Ӻ��� ���� ���� ä���ϱ�� ��
        private IPathStrategy       pathStrategy;       // ��� �̵� ���
        private IRangeStrategy      rangeStrategy;      // ��ġ ���� �� ���� ó��
        
        public Collider Collider => _projectileCollider;
        public Transform ProjectileTransform => _projectileTransform;
        public CharBase Caster => _caster;
        public CharBase Target => _target;
        public IPathStrategy PathGuide => pathStrategy;
        public IRangeStrategy RangeGuide => rangeStrategy;

        private void Awake()
        {
            _projectileTransform = transform;
            _projectileCollider = GetComponent<Collider>();
            _projectileData = DataManager.Instance.GetData<ProjectileData>(_index);
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

            pathStrategy = PathStrategyFactory.CreatePathStrategy
                (new PathStrategyParameter()
                {
                    target = _target,
                    type = _projectileData.path,
                    Speed = 2f, // �̰ž�� ��������
                });
        }

        public void InjectProjectileFunction(List<long> indices)
        {
            foreach(long index in indices)
            {
                _functionDataList.Add(DataManager.Instance.GetData<FunctionData>(index));
            }
        }

        private void FixedUpdate()
        {
            if (_caster == null || destroyFlag == true)
                Destroy(gameObject);
            pathStrategy.UpdatePosition(this);
        }


        // ȿ�� ���� ������ �� ��󿡰� ����� �� function�� ����
        public void ApplyEffect(CharBase target)
        {           
            // ����� ��Ʈ
            target.CharStat.ReceiveDamage(_caster.CharStat.SendDamage(_projectileDamageInput));
            Debug.Log(target.CharStat.GetStat(SystemEnum.eStats.NHP));
            
            // ȿ�� ��Ʈ
            foreach(var data in _functionDataList)
            {
                target.FunctionInfo.AddFunction(new BuffParameter()
                {
                    eFunctionType = data.function,
                    CastChar = Caster,
                    TargetChar = target,
                    FunctionIndex = data.Index
                });
            }

            // ��쿡 ���� ��ġ �ٲ����...? 2���� 3���� �׷��� �������� �ְ�
            if(--_projectileLife < 0)
            {
                SetDestroyFlag(true);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //getcomponent�� ���� ���°Ϳ� ����....
            CharBase target = other.GetComponent<CharBase>();


        }

    }


}