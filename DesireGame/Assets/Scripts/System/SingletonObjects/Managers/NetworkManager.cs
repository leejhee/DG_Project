using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using System.Text;
using UnityEngine.Networking;



namespace Client
{
    /// <summary>
    /// (���Ž�)Google Sheet�� �о���� ���� Manager
    /// </summary>
    public class NetworkManager
    {
        public GoogleSheet data { get; private set; }
        // Start is called before the first frame update

        public void Init()
        {
            data = Resources.Load<GoogleSheet>("ScriptableObjects/GoogleSheet");
            Debug.Log(data.associatedDataWorksheet);
        }

        /// <summary>
        /// ���� �������� ��Ʈ �� �޾ƿ� �����ϴ� �Լ�
        /// </summary>
        /// <param name="GoogleSheetsID"> ���� �������� ��Ʈ�� ID </param>
        /// <param name="ManufactureData"> �������� ��Ʈ ���� �Լ� input�� �� �� �� �޾ƾ���, Cell�� ��('\t')���� ����</param>
        /// <param name="WorkSheetsID"> ��Ʈ�� WorkSheet ���� default �� 0 ù ��</param>
        /// <param name="startCell"> ���� �� default A1</param>
        /// <param name="endCell"> �� �� default E </param>
        /// <returns></returns>
        public IEnumerator GoogleSheetsDataParsing(string GoogleSheetsID, Action<string> ManufactureData, string WorkSheetsID = "0", string endCell = "Z", string startCell = "A2")
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("https://docs.google.com/spreadsheets/d/");
            sb.Append(GoogleSheetsID);
            sb.Append("/export?format=tsv");
            sb.Append("&gid=" + data.associatedDataWorksheet);
            sb.Append("&range=" + startCell + ":" + endCell);

            Debug.Log(sb.ToString());


            using (UnityWebRequest webData = UnityWebRequest.Get(sb.ToString()))
            {

                yield return webData.SendWebRequest();

                if (webData.isNetworkError || webData.isHttpError)
                {
                    Debug.Log("��Ʈ��ũ ���� �߰� ���ͳ�ȯ�� �Ǵ� URL Ȯ�� �ٶ�");
                }
                else
                {
                    string[] dataLines = webData.downloadHandler.text.Split('\n');
                    Action[] ManufactureDatas = new Action[dataLines.Length];
                    for (int i = 0; i < dataLines.Length; i++)
                    {
                        ManufactureData(dataLines[i]);
                    }
                }

            }

        }


    }
}