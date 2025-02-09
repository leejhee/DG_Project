using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;
using UnityEngine.AI;

namespace Client
{
    /// <summary>
    /// ĳ���� ���̽� class
    /// </summary>
    public abstract class CharBase : MonoBehaviour
    {
        // SerializeField 
        [SerializeField] private   long         _index;         // CharData ���̺��� �ε��� 
        [SerializeField] private   Collider     _FightCollider; // ���� �ݶ��̴�
        [SerializeField] private   Collider     _MoveCollider;  // �̵� �ݶ��̴�
        [SerializeField] private   GameObject   _SkillRoot;     // ��ų ��Ʈ
        [SerializeField] protected NavMeshAgent _NavMeshAgent;  // �׺� �޽� ������Ʈ
        //[SerializeField] protected CharacterController _CharController; // ĳ���� �ִϸ��̼� ����Ʈ
        [SerializeField] protected GameObject _CharCamaraPos; // ĳ���� �ִϸ��̼� ����Ʈ
        [SerializeField] protected Animator _Animator;       // �ִϸ�����\

        private FunctionInfo _functionInfo = null; // ��� ����
        //private CharItemInfo  _charItemInfo;         // ĳ���� ����/��� ������

        private CharSKillInfo _charSKillInfo;       // ĳ���� ��ų
        private CharStat     _charStat = null;      // Stat ����
        private CharData     _charData = null;      // ĳ���� ������
        private CharAnim     _charAnim = null;      // ĳ���� �ִϸ��̼� ����Ʈ
        private CharAction  _charAction = null;     // ĳ���� ���� ��� Ŭ����
        private CharAI      _charAI = null;

        //private GameObject _LWeapon = null;       // �޼� ����
        //private GameObject _RWeapon = null;       // ������ ����
        private Transform  _CharTransform = null; // ĳ���� Ʈ������
        private Transform  _CharUnitRoot = null;  // ĳ���� ���� ��Ʈ Ʈ������

        private PlayerState _currentState; // ���� ����
        private bool _isAction = false;    // �ൿ���ΰ�? �Ǻ�
        private Dictionary<PlayerState, int> _indexPair = new(); // 
        
        protected long _uid;  // ĳ���� ���� ID

        private Vector3 _rightRotation = new Vector3(0, 180,0); // ������ �����̼�
        private Vector2 _lefRotationtion = Vector3.zero;        // ���� �����̼�

        private Vector3 _rightPos = new Vector3(1, 0, 0); // ������ ����
        private Vector2 _leftPos = new Vector3(-1, 0, 0); // ���� ����

        public Vector3 LookAtPos { get; private set; } = Vector3.right; // ���� ���⼺

        protected virtual SystemEnum.eCharType CharType => SystemEnum.eCharType.None; // ĳ���� Ÿ��

        public Dictionary<eFunction, List<FunctionBase>> FunctionBaseDic => _functionInfo.FunctionBaseDic; // �ൿ 
        public Collider FightCollider => _FightCollider; // 
        public Collider MoveCollider  => _MoveCollider;
        public FunctionInfo FunctionInfo => _functionInfo;  // ��� ����
        public CharSKillInfo CharSKillInfo => _charSKillInfo; // ĳ���� ��ų
        public Transform CharTransform => _CharTransform;
        private Transform CharUnitRoot => _CharUnitRoot; // ĳ���� ���� ��Ʈ Ʈ������
        //public CharItemInfo CharItemInfo => _charItemInfo;
        public GameObject CharCamaraPos => _CharCamaraPos; // ī�޶� ��ġ
        public CharAnim CharAnim => _charAnim; // ī�޶� ��ġ
        public NavMeshAgent Nav => _NavMeshAgent;
        public CharStat CharStat => _charStat;
        public CharAction CharAction => _charAction;
        public CharAI CharAI => _charAI;

        protected CharBase() { }

        private void Awake()
        {
            _CharTransform = transform;
            _CharUnitRoot = Util.FindChild<Transform>(gameObject,"UnitRoot");
            _uid = CharManager.Instance.GetNextID();
            _functionInfo = new FunctionInfo();
            _functionInfo.Init();
            _charData = DataManager.Instance.GetData<CharData>(_index);
            _NavMeshAgent = GetComponent<NavMeshAgent>();
            _charAnim = new();
            _charAction = new(this);
            if (_charData != null)
            {
                StatsData charStat = DataManager.Instance.GetData<StatsData>(_charData.specIndex);
                if (charStat == null)
                {
                    Debug.LogError($"ĳ���� ID : {_index} ������ Get ���� charStat {_charData.specIndex} ������ Get ����");
                }
                _charStat = new CharStat(charStat);
            }
            else
            {
                Debug.LogError($"ĳ���� ID : {_index} Data ������ Get ����");
            }
        }
        private void Start()
        {
            if (_charAnim != null)
            {
                _charAnim.Initialized(_Animator);
            }
            foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
            {
                _indexPair[state] = 0;
            }
            CharInit();
        }

        void Update()
        {
            foreach (var functionBaseList in FunctionBaseDic)
            {
                foreach (var function in functionBaseList.Value)
                {
                    function.CheckTimeOver();
                    function.Update(Time.deltaTime);
                }
            }
            
        }

        // Char�� Start������ �Ҹ�
        protected virtual void CharInit()
        {
            CharManager.Instance.SetChar<CharBase>(this);
            
            // ��ų
            _charSKillInfo = new CharSKillInfo(this);
            if (_charSKillInfo != null)
            {               
                _charSKillInfo.Init(new List<long>() 
                { _charData.autoAttack, _charData.skillAttack});
            }
        }

        public virtual void CharDistroy()
        {
            Type myType = this.GetType();
            CharManager.Instance.Clear(myType, _uid);
            Destroy(gameObject);
        }

        public long GetID() => _uid;

        public SystemEnum.eCharType GetCharType()
        {
            return CharType;
        }

        //public void CharMove(Vector2 vector)
        //{
        //    if (vector == Vector2.zero)
        //        return;
        //    if (_charStat == null)
        //    {
        //        Debug.LogWarning($"{transform.name} �� Stat�� ����");
        //        return;
        //    }
        //    float angle = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
            
        //    // Y���� �������� ȸ�� (2D ��鿡�� �ٶ󺸴� ����)
        //    Vector3 deltaRotation = new Vector3(0, angle, 0);
        //    transform.eulerAngles += deltaRotation * Time.deltaTime;
            
        //    Vector3 deltaMove = transform.forward * _charStat.GetStat(eState.NSpeed);
            
        //    deltaMove = deltaMove * Time.deltaTime;
        //    _NavMeshAgent.Move(deltaMove);
        //}

        ///// <summary>
        ///// ȣ�� ���� �ش� ��ġ�� �̵�
        ///// </summary>
        ///// <param name="vector"></param>
        //public void CharMoveTo(Vector2 vector)
        //{
        //    if (_NavMeshAgent == false)
        //        return;

        //    _NavMeshAgent.SetDestination(vector);
        //}

        ///// <summary>
        ///// ȣ�� ���� ĳ������ ��ġ�� �̵�
        ///// </summary>
        ///// <param name="targetChar"></param>
        //public void CharMoveTo(CharBase targetChar)
        //{
        //    if (targetChar == false || _NavMeshAgent == false)
        //        return;

        //    _NavMeshAgent.SetDestination(targetChar.transform.position);
        //}

        ///// <summary>
        ///// ȣ�� ���� ĳ������ ��ġ�� �̵�
        ///// </summary>
        ///// <param name="targetChar"></param>
        //public void CharMoveTo(long targetCharUID)
        //{
        //    CharBase charBase = CharManager.Instance.GetFieldChar(targetCharUID);
        //    if (charBase == false)
        //        return;
        //    CharMoveTo(charBase);
        //}

        public void SetStateAnimationIndex(PlayerState state, int index = 0)
        {
            _indexPair[state] = index;
        }
        public void PlayStateAnimation(PlayerState state)
        {
            _charAnim.PlayAnimation(state);
        }
    }
}