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

        // �ܺο��� ��ȯ�� �� ȣ������. 
        public virtual void InitProjectile(CharBase Caster)
        {
            _caster = Caster;
            if(_projectileData == null)
            {
                Debug.LogError("�� ������ ���°Ŷ�µ���? �ε��� ����� Ȯ�ιٶ�");
                return;
            }

        }

        // ��Ÿ���� ��� ���̸� execution �߻�
        // Ÿ���� ��� other �˻�� target�̾�� execution �߻�
        public abstract void OnTriggerEnter(Collider other);

    }


}