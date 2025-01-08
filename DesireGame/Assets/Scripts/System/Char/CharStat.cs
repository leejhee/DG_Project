using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    /// <summary>
    /// char 스테이트
    /// </summary>
    public class CharStat
    {
        private long[] _charStat = new long[(int)eState.MaxCount];
        
        public CharStat(CharStatData charStat)
        {
            _charStat[(int)eState.HP] = charStat.HP;
            _charStat[(int)eState.NHP] = charStat.HP;
            _charStat[(int)eState.NMHP] = charStat.HP;

            _charStat[(int)eState.Defence] = charStat.Def;
            _charStat[(int)eState.NDefence] = charStat.Def;

            _charStat[(int)eState.SP] = charStat.SP;
            _charStat[(int)eState.NSP] = charStat.SP;
            _charStat[(int)eState.NMSP] = charStat.SP;

            _charStat[(int)eState.Speed] = charStat.Speed;
            _charStat[(int)eState.NSpeed] = charStat.Speed;

            _charStat[(int)eState.Attack] = charStat.Attack;
            _charStat[(int)eState.NAttack] = charStat.Attack;
        }
        public float GetStat(eState eState)
        {
            return _charStat[(int)eState] / SystemConst.Persent;
        }
        public long GetStatRaw(eState eState)
        {
            return _charStat[(int)eState];
        }



    }
}