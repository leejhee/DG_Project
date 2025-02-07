using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public static class FunctionFactory
    {
        public static FunctionBase FunctionGenerate(BuffParameter buffParam)
        {
            switch (buffParam.eFunctionType)
            {
                case eFunction.SpawnProjectileByMana: return new SpawnProjectileByMana(buffParam);
                case eFunction.SpawnProjectileByAS: return new SpawnProjectileByAS(buffParam);
                case eFunction.DamageByCasterAD: return new DamageByCasterAD(buffParam);
                case eFunction.DamageByCasterAP: return new DamageByCasterAP(buffParam);
            }

            return null;
        }

    }
}