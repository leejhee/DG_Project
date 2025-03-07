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
        protected int _projectileLife; // 투사체의 관통 허용 수치
        
        // 뭔가뭔가다.
        private bool destroyFlag = false;
        public void SetDestroyFlag(bool flag) => destroyFlag = flag;



        // 3가지 고려할 경우 경우의 수가 너무 많아 상속보단 전략 패턴 채용하기로 함
        private IPathStrategy       pathStrategy;       // 경로 이동 방식
        private IRangeStrategy      rangeStrategy;      // 위치 도달 후 범위 처리
        
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

        // 외부에서 소환할 때 호출하자. 
        public virtual void InitProjectile(StatPackedSkillParameter param)
        {
            _caster = param.skillCaster;
            _target = param.skillTargets[param.TargetIndex];
            if(_projectileData == null)
            {
                Debug.LogError("이 데이터 없는거라는데요? 인덱스 제대로 확인바람");
                return;
            }
            _projectileDamageInput =
                (param.Percent / SystemConst.PER_CENT) * _caster.CharStat.GetStat(param.StatOperand);

            pathStrategy = PathStrategyFactory.CreatePathStrategy
                (new PathStrategyParameter()
                {
                    target = _target,
                    type = _projectileData.path,
                    Speed = 2f, // 이거어떻게 결정하지
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


        // 효과 적용 판정이 된 대상에게 대미지 및 function을 주입
        public void ApplyEffect(CharBase target)
        {           
            // 대미지 파트
            target.CharStat.ReceiveDamage(_caster.CharStat.SendDamage(_projectileDamageInput));
            Debug.Log(target.CharStat.GetStat(SystemEnum.eStats.NHP));
            
            // 효과 파트
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

            // 경우에 따라 수치 바뀔수도...? 2저지 3저지 그런거 있을수도 있고
            if(--_projectileLife < 0)
            {
                SetDestroyFlag(true);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //getcomponent를 많이 쓰는것에 대해....
            CharBase target = other.GetComponent<CharBase>();


        }

    }


}