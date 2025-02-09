using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;
using UnityEngine.AI;

namespace Client
{
    /// <summary>
    /// 캐릭터 베이스 class
    /// </summary>
    public abstract class CharBase : MonoBehaviour
    {
        // SerializeField 
        [SerializeField] private   long         _index;         // CharData 테이블의 인덱스 
        [SerializeField] private   Collider     _FightCollider; // 전투 콜라이더
        [SerializeField] private   Collider     _MoveCollider;  // 이동 콜라이더
        [SerializeField] private   GameObject   _SkillRoot;     // 스킬 루트
        [SerializeField] protected NavMeshAgent _NavMeshAgent;  // 네비 메쉬 에이전트
        //[SerializeField] protected CharacterController _CharController; // 캐릭터 애니메이션 리스트
        [SerializeField] protected GameObject _CharCamaraPos; // 캐릭터 애니메이션 리스트
        [SerializeField] protected Animator _Animator;       // 애니메이터\

        private FunctionInfo _functionInfo = null; // 기능 정보
        //private CharItemInfo  _charItemInfo;         // 캐릭터 보유/장비 아이템

        private CharSKillInfo _charSKillInfo;       // 캐릭터 스킬
        private CharStat     _charStat = null;      // Stat 정보
        private CharData     _charData = null;      // 캐릭터 데이터
        private CharAnim     _charAnim = null;      // 캐릭터 애니메이션 리스트
        private CharAction  _charAction = null;     // 캐릭터 동작 명령 클래스
        private CharAI      _charAI = null;

        //private GameObject _LWeapon = null;       // 왼손 무기
        //private GameObject _RWeapon = null;       // 오른손 무기
        private Transform  _CharTransform = null; // 캐릭터 트렌스폼
        private Transform  _CharUnitRoot = null;  // 캐릭터 유닛 루트 트렌스폼

        private PlayerState _currentState; // 현재 상태
        private bool _isAction = false;    // 행동중인가? 판별
        private Dictionary<PlayerState, int> _indexPair = new(); // 
        
        protected long _uid;  // 캐릭터 고유 ID

        private Vector3 _rightRotation = new Vector3(0, 180,0); // 오른쪽 로테이션
        private Vector2 _lefRotationtion = Vector3.zero;        // 왼쪽 로테이션

        private Vector3 _rightPos = new Vector3(1, 0, 0); // 오른쪽 방향
        private Vector2 _leftPos = new Vector3(-1, 0, 0); // 왼쪽 방향

        public Vector3 LookAtPos { get; private set; } = Vector3.right; // 현재 방향성

        protected virtual SystemEnum.eCharType CharType => SystemEnum.eCharType.None; // 캐릭터 타입

        public Dictionary<eFunction, List<FunctionBase>> FunctionBaseDic => _functionInfo.FunctionBaseDic; // 행동 
        public Collider FightCollider => _FightCollider; // 
        public Collider MoveCollider  => _MoveCollider;
        public FunctionInfo FunctionInfo => _functionInfo;  // 기능 정보
        public CharSKillInfo CharSKillInfo => _charSKillInfo; // 캐릭터 스킬
        public Transform CharTransform => _CharTransform;
        private Transform CharUnitRoot => _CharUnitRoot; // 캐릭터 유닛 루트 트렌스폼
        //public CharItemInfo CharItemInfo => _charItemInfo;
        public GameObject CharCamaraPos => _CharCamaraPos; // 카메라 위치
        public CharAnim CharAnim => _charAnim; // 카메라 위치
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
                    Debug.LogError($"캐릭터 ID : {_index} 데이터 Get 성공 charStat {_charData.specIndex} 데이터 Get 실패");
                }
                _charStat = new CharStat(charStat);
            }
            else
            {
                Debug.LogError($"캐릭터 ID : {_index} Data 데이터 Get 실패");
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

        // Char의 Start시점에 불림
        protected virtual void CharInit()
        {
            CharManager.Instance.SetChar<CharBase>(this);
            
            // 스킬
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
        //        Debug.LogWarning($"{transform.name} 의 Stat가 없음");
        //        return;
        //    }
        //    float angle = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
            
        //    // Y축을 기준으로 회전 (2D 평면에서 바라보는 방향)
        //    Vector3 deltaRotation = new Vector3(0, angle, 0);
        //    transform.eulerAngles += deltaRotation * Time.deltaTime;
            
        //    Vector3 deltaMove = transform.forward * _charStat.GetStat(eState.NSpeed);
            
        //    deltaMove = deltaMove * Time.deltaTime;
        //    _NavMeshAgent.Move(deltaMove);
        //}

        ///// <summary>
        ///// 호출 시점 해당 위치로 이동
        ///// </summary>
        ///// <param name="vector"></param>
        //public void CharMoveTo(Vector2 vector)
        //{
        //    if (_NavMeshAgent == false)
        //        return;

        //    _NavMeshAgent.SetDestination(vector);
        //}

        ///// <summary>
        ///// 호출 시점 캐릭터의 위치로 이동
        ///// </summary>
        ///// <param name="targetChar"></param>
        //public void CharMoveTo(CharBase targetChar)
        //{
        //    if (targetChar == false || _NavMeshAgent == false)
        //        return;

        //    _NavMeshAgent.SetDestination(targetChar.transform.position);
        //}

        ///// <summary>
        ///// 호출 시점 캐릭터의 위치로 이동
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