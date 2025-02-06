using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Client
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        private long            _index;
        
        private Collider        _projectileCollider;
        private Transform       _projectileTransform;
        private ExecutionInfo   _executionInfo;
        private CharBase        _caster;

        protected ProjectileData _projectileData;

        public Collider Collider => _projectileCollider;
        public CharBase CharBase => _caster;
        public Transform ProjectileTransform => _projectileTransform;
        public ExecutionInfo ExecutionInfo => _executionInfo;

        private void Awake()
        {
            _projectileTransform = transform;
            _projectileCollider = GetComponent<Collider>();
            _projectileData = DataManager.Instance.GetData<ProjectileData>(_index);
            _executionInfo = new();
            _executionInfo.Init();
        }

        // 외부에서 소환할 때 호출하자. 
        public virtual void InitProjectile(CharBase Caster)
        {
            _caster = Caster;
            if(_projectileData == null)
            {
                Debug.LogError("이 데이터 없는거라는데요? 인덱스 제대로 확인바람");
                return;
            }

        }

        // 논타겟일 경우 적이면 execution 발생
        // 타겟일 경우 other 검사시 target이어야 execution 발생
        public abstract void OnTriggerEnter(Collider other);

    }


}