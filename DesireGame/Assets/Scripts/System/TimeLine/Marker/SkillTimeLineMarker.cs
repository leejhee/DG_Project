using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Client
{
    /// <summary>
    /// ��ų Ÿ�Ӷ��ο� ��Ŀ
    /// </summary>
    public class SkillTimeLineMarker : Marker, INotification
    {
        public PropertyName id => new PropertyName("SkillTimeLineMarker");


        public virtual void  MarkerAction()
        {

        }

        public virtual void SkillInitialize()
        {

        }

        public override void OnInitialize(TrackAsset aPent)
        {
            base.OnInitialize(aPent);
            SkillInitialize();
        }
    }
    
    

}