using UnityEngine.Playables;
using UnityEngine;
namespace Client
{
    public class SkillMarkerReceiver : MonoBehaviour, INotificationReceiver
    {
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is SkillTimeLineMarker skillMarker)
            {
                IContextProvider provider = GetComponent<IContextProvider>();

                skillMarker.InitInput(provider.InputParameter);
                skillMarker.MarkerAction();

                Debug.Log("여기까지 왔네요.");
            }
        }
    }
}


