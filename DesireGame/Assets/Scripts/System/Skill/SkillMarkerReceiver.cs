using UnityEngine.Playables;
using UnityEngine;
using Client;

public class SkillMarkerReceiver : MonoBehaviour, INotificationReceiver
{
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification is SkillTimeLineMarker skillMarker)
        {
            var director = origin.GetGraph().GetResolver() as PlayableDirector;
            if(director == GetComponent<PlayableDirector>())
            {
                IContextProvider provider = GetComponent<IContextProvider>();
                skillMarker.SetBuffParameter(provider.BuffParameter);
            }
            Debug.Log("여기까지 왔네요.");
        }
    }
}