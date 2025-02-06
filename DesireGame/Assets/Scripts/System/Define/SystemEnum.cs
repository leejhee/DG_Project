using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{
    /// <summary>
    /// 게임에 필요한 Enum
    /// </summary>
    public class SystemEnum
    {
        public enum eSounds
        {
            BGM,
            SFX,
            MaxCount
        }
        public enum eBGM
        {

            MaxCount
        }
        public enum eSFX
        {
            MaxCount
        }
        
        public enum eTag
        {
            MaxCount
        }

        /// <summary>
        /// UI Event 종류 지정
        /// </summary>
        public enum eUIEvent
        {
            Click,
            Drag,
            DragEnd,

            MaxCount
        }

        public enum eScene
        {
            Title,
            Loby,
            GameScene,

            MaxCount
        }
       
        public enum eCharType
        {
            None,
            Player,
            PlayerSide,
            Monster,
            NPC,
            Projectal,

            MaxCount
        }

        public enum eCharTier
        {
            NORMAL,
            RARE,
            UNIQUE,
            EPIC,
            LEGEND,
            EMax,
        }

        public enum eSynergy
        {
            None,
            SWORD,
            RANGED,
            SHIELD,
            DUAL_BALDE,
            MAGIC_WAND,
            LAPLACIAN,
            TIMER,
            QUANTUM_WASHER,
            TIDY,
            TEAM6,
            EMax,
        }
        public enum eItemTier
        {
            NORMAL,
            RARE,
            UNIQUE,
            EPIC,
            LEGEND,
            EMax,
        }

        public enum eStats
        {
            AD,
            NAD,

            AP,
            NAP,
            
            HP,
            NHP,
            NMHP,

            AS,
            NAS,

            CRIT_CHANCE,
            NCRIT_CHANCE,

            CRIT_DAMAGE,
            NCRIT_DAMAGE,

            DAMAGE_INCREASE,
            BONUS_DAMAGE,

            ARMOR,
            NARMOR,

            MAGIC_RESIST,
            NMAGIC_RESIST,

            ARMOR_PENETRATION,
            MAGIC_PENETRATION,

            RANGE,
            NRANGE,

            MOVE_SPEED,
            NMOVE_SPEED,

            START_MANA,
            N_MANA,
            MAX_MANA,

            EMax,
        }

        public enum eFunction
        {
            // 공속기반 투사체 생성
            SpawnProjectileByAS,
            // 공격력 기반 대미지
            DamageByCasterAD,
            // 주문력 기반 대미지
            DamageByCasterAP,
            // 다음 공격에 대한 버프
            BuffDuringAttacks,
            // 마나쓰는 스킬 투사체 생성
            SpawnProjectileByMana,
            // 주문력 기반 대미지와 디버프
            DamageByCasterAPWithCC,
            // 공속기반 근접공격
            MeleeADByAS,
            // 최대체력 비례 대미지 & 흡혈
            DamageByTargetMaxHPAndHeal,
            // 공격력 기반 확정크리티컬 대미지
            DamageByCasterADWithCrit,
            // 공격력 기반 일회성버프
            BuffOnceByCasterAD,
            // 여러 효과 동시 발동(최대 4개)
            MultiCasting,
            // 일회성 버프
            BuffOnce,


            MaxCount
        }

        public enum eDebuffType
        {
            통보,
            가학,
            치유력_감소,

            eMax
        }

        public enum eProjectileTargetType
        {
            NEAR_TARGET,
            CURRENT_TARGET,
            FARTHEST_TARGET,
            NEAR_TARGET_2,
            NEAR_TARGET_3,
            FARTHEST_TARGET_2,
            LOW_HP_TARGET,
            EMax,
        }

        public enum eProjectilePathType
        {
            STRAIGHT,
            TARGET_POSITION,
            PINGPONG,
            UNTIL_WALL,
            WALL_BOUNCE,
            EMax,
        }

        public enum eProjectileRangeType
        {
            SINGLE,
            SPLASH,
            EMax,
        }

        public enum eLocalize
        {
            KOR,
            ENG,

            MaxCount
        }

        public enum eTrackOrder
        {
            ANIM_TRACK,

        }

        #region 교체될 수 있는 enum들이므로 용도가 겹칠 경우 삭제해줄 것

        public enum eState
        {
            None,
            HP, // 기본 HP
            NHP, // 현재 HP
            NMHP, // 현재 최고 HP

            Defence, // 기본 방어력
            NDefence, // 현재 방어력

            SP, // 기본 SP
            NSP, // 현재 SP
            NMSP, // 현재 최고 SP

            Speed, // 기본 스피드 
            NSpeed, // 현재 Speed

            Attack, // 공격력
            NAttack, // 현재 공격력

            MaxCount
        }

        public enum eExecutionGroupType
        {
            None,
            Buff,
            Debuff,
            Create,
            Action,

            MaxCount,
        }

        public enum eExecutionType
        {
            None,
            Avoidance, // 무적
            StateBuff, // 상수값 버프 증감 
            StateBuffPer, // 기본State 버프 퍼센트 증감
            StateBuffNPer, // 현재State 버프 퍼센트 증감
            Parrying, // 방향 패링
            DotDamage, // 도트 데미지

            MaxCount
        }

        public enum eExecutionCondition
        {
            None,
            ParryingSuccess,

            MaxCount
        }

        public enum eSkillType
        {
            None,
            BasicAttack,
            CharSkill,
            Dash,
            Parrying,
            Attack,
            Buff,

            MaxCount
        }

        public enum eIsAttack
        {
            None,

            Player,
            Monster,

            MaxCount
        }

        public enum eItemType
        {
            None,

            Equipment,
            Consumable,
            ETC,

            MaxCount
        }
        
        public enum PlayerAnim
        {
            idle,
            move,
            attack,
            damaged,
            debuff,
            death,

            MaxCount
        }

        public enum PlayerState
        {
            IDLE,
            MOVE,
            ATTACK,
            DAMAGED,
            DEBUFF,
            DEATH,
            OTHER,

            MaxCount
        }
        

        #endregion
    }
}