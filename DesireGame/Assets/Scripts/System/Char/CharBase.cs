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
        [SerializeField] protected GameObject _CharCamaraPos; // ĳ���� �ִϸ��̼� ����Ʈ
        [SerializeField] protected Animator _Animator;       // �ִϸ�����\

        protected FunctionInfo _functionInfo = null; // ��� ����
        //private CharItemInfo  _charItemInfo;         // ĳ���� ����/��� ������

        private CharSKillInfo _charSKillInfo;       // ĳ���� ��ų
        private CharStat     _charStat = null;      // Stat ����
        private CharAnim     _charAnim = null;      // ĳ���� �ִϸ��̼� ����Ʈ
        private CharAction  _charAction = null;     // ĳ���� ���� ��� Ŭ����
        protected CharData _charData = null;      // ĳ���� ������
        protected CharAI      _charAI = null;

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

        private Coroutine _coroutine; // UpdateAI��,,
        public Vector3 LookAtPos { get; private set; } = Vector3.right; // ���� ���⼺


        protected virtual SystemEnum.eCharType CharType => SystemEnum.eCharType.None; // ĳ���� Ÿ��

        public long Index => _index;
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
        public CharData CharData => _charData;
        public PlayerState PlayerState => _currentState;

        public int TileIndex { get; set; } = default;

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
            _charAnim =     new();
            _charAction =   new(this);
            _charAI =       new(this);
            if (_charData != null)
            {
                StatsData charStat = DataManager.Instance.GetData<StatsData>(_charData.statsIndex);
                if (charStat == null)
                {
                    Debug.LogError($"ĳ���� ID : {_index} ������ Get ���� charStat {_charData.statsIndex} ������ Get ����");
                }
                _charStat = new CharStat(charStat, this);
            }
            else
            {
                Debug.LogError($"ĳ���� ID : {_index} Data ������ Get ����");
            }
            
        }

        protected virtual void Start()
        {
            if (_charAnim != null)
            {
                //_charAnim.Initialized(_Animator);
                _charAnim.Initialized(GetComponentInChildren<Animator>());
            }
            foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
            {
                _indexPair[state] = 0;
            }
            CharInit();
            // TODO : on off ���
            // turnonandoff �̷������� �ؼ� ���� �״ٸ� �� �� �ְ� �ؾ��ϱ� ������
            // ���� ������ �ؾ� �Ѵٰ� �Ѵ�
            // CharBase�� �Լ��� �����Ͽ� CharManager���� _cache �� ��� CharBase�� ������� 
            // �ش� �Լ��� ȣ���ϰ� �ϸ� AI�� Ű�� �� �� �ִ�.
        }

        void Update()
        {
            FunctionInfo?.UpdateFunctionDic();            
        }

        // Char�� Start������ �Ҹ�
        protected virtual void CharInit()
        {
            #region ��ų
            _charSKillInfo = new CharSKillInfo(this);
            if (_charSKillInfo != null)
            {               
                _charSKillInfo.Init(new List<long>() 
                { _charData.skill1, _charData.skill2});
            }
            #endregion

            #region �ʱ� �нú�
            foreach (long functionIndex in _charData.func)
            {
                var functiondata = DataManager.Instance.GetData<FunctionData>(functionIndex);
                var passiveFunction = FunctionFactory.FunctionGenerate(
                    new BuffParameter()
                    {
                        FunctionIndex = functiondata.Index,
                        CastChar = this,
                        TargetChar = this,
                        eFunctionType = functiondata.function
                    });
            }
            #endregion
         
            _charStat.OnDeath += () =>
            {
                Debug.Log($"ĳ���� ��� : uid {_uid}, �̸� {_charData.charName}");
                CharDead();
            };

            Dead += () =>
            {
                CharDead();
                Destroy(gameObject);
            };
        }

        public virtual void CharDead()
        {
            Type myType = this.GetType();
            CharManager.Instance.Clear(myType, _uid);
            gameObject.SetActive(false);
            //Destroy(gameObject);
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

        /// <summary>
        /// ĳ���� AI on & off ���
        /// </summary>
        public void AISwitch(bool turnOn = true)
        {
            if (CharAI == null) return;
            // CharBase�� �Լ��� �����Ͽ� CharManager����(ȣ���ϴ³�) _cache �� ��� CharBase�� ������� 
            // �ش� �Լ��� ȣ���ϰ� �ϸ� AI�� Ű�� �� �� �ִ�.
            CharAI.isAIRun = turnOn;
            if (turnOn)
            {
                
                _coroutine = StartCoroutine(CharAI.UpdateAI(_currentState));
            }
            else
            {
                StopCoroutine(_coroutine);
            }
        }

        public void SetNavMeshAgent(bool active)
        {
            if (_NavMeshAgent == false)
                return;
            _NavMeshAgent.enabled = active;
        }

        public void SetStateAnimationIndex(PlayerState state, int index = 0)
        {
            _indexPair[state] = index;
        }
        public void PlayStateAnimation(PlayerState state)
        {
            _charAnim.PlayAnimation(state);
        }

        public Action Dead;

        //public void Dead()
        //{
        //    CharDead();
        //    Destroy(gameObject);
        //}

        // �÷��̾� ���� �ȸ� ���� ������� �ó��� ���־��Ѵ�... �Ǹ� �ִ�����
        public void Sell()
        {
            Dead.Invoke();
            //�� ������ �ϴ� �͵� ��������
        }

    }
}