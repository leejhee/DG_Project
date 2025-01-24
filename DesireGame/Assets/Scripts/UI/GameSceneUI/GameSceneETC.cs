using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace Client
{
    public class GameSceneETC : MonoBehaviour
    {
        [SerializeField] Button CombatStartBtn;
        [SerializeField] TMP_Text MoneyInfo;

        [SerializeField] Button CreditTestBtn;

        private void Awake()
        {
            UIManager.Instance.SetCanvas(gameObject, false);

            CreditTestBtn.onClick.RemoveAllListeners();
            CreditTestBtn.onClick.AddListener(() => TestCredit());
        }

        void TestCredit()
        {
            if(TestManager.Instance.LobbySave != null)
            {
                TestManager.Instance.TestSave();
                TestManager.Instance.currentSave.credit += 10;
                MoneyInfo.SetText(TestManager.Instance.currentSave.credit.ToString());
            }
        }
    }
}