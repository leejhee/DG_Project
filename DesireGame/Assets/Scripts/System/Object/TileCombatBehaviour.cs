using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
namespace Client
{
    /// <summary> ���� �� Ÿ���� å�ӿ� ���� Ŭ���� </summary>
    public class TileCombatBehaviour : MonoBehaviour
    {
        private HashSet<CharBase> steppingOnChar = new();
        public bool IsEmpty => steppingOnChar.Count == 0;

        /// <summary> Ÿ�� �� �ؽü��� �������� ����. Ÿ�� ��� ������ ó�� </summary>
        /// <param name="charList"> union�� �ؽü� </param>
        /// <returns> ���� Ÿ�ϰ� union�� ĳ���� �ؽü� </returns>
        public HashSet<CharBase> GetTotalChar(HashSet<CharBase> charList)
        {
            // ���纻���� ���� ���� ����
            HashSet<CharBase> result = new(steppingOnChar);
            result.UnionWith(charList);
            return result;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out CharBase client))
            {
                if(!steppingOnChar.Contains(client)) 
                    steppingOnChar.Add(client);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out CharBase client))
            {
                if (steppingOnChar.Contains(client))
                    steppingOnChar.Remove(client);
            }
        }
    }
}