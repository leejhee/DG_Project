using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public static class ExecutionFactory
    {
        public static ExecutionBase ExecutionGenerate(BuffParameter buffParam)
        {
            switch (buffParam.eFunctionType)
            {
                case eFunction.SpawnProjectileByMana:
                case eFunction.SpawnProjectileByAS: return new SpawnProjectileByAS(buffParam);
                case eFunction.DamageByCasterAD: return new DamageByCasterAD(buffParam);
                case eFunction.DamageByCasterAP: return new DamageByCasterAP(buffParam);
            }

            return null;
        }

    }
}