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
        private FunctionInfo   _functionInfo;
        private CharBase        _caster;
        private CharBase        _target;

        protected ProjectileData _projectileData;

        public Collider Collider => _projectileCollider;
        public Transform ProjectileTransform => _projectileTransform;
        public FunctionInfo FunctionInfo => _functionInfo;
        public CharBase CharBase => _caster;
        public CharBase Target => _target;
        

        private void Awake()
        {
            _projectileTransform = transform;
            _projectileCollider = GetComponent<Collider>();
            _projectileData = DataManager.Instance.GetData<ProjectileData>(_index);
            _functionInfo = new();
            _functionInfo.Init();
        }

        // �ܺο��� ��ȯ�� �� ȣ������. 
        public virtual void InitProjectile(InputParameter param)
        {
            _caster = param.skillCaster;
            _target = param.skillTarget;
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