using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ListView.Internal
{
    public class ScrollCtrl<T> where T : MonoBehaviour, IListItem
    {
        private ScrollRect scrollRect;
        private Scrollbar verticalScrollbar { get { return scrollRect.verticalScrollbar; } }
        private Scrollbar horizontalScrollbar { get { return scrollRect.horizontalScrollbar; } }
        public UnityEngine.Events.UnityAction<float> onUpdateScroll;

        public ScrollCtrl(ScrollRect scrollRect)
        {
            this.scrollRect = scrollRect;
            RegistScrollEvent();
        }
        private void RegistScrollEvent()
        {
            if (verticalScrollbar != null)
            {
                verticalScrollbar.value = 1;
                verticalScrollbar.onValueChanged.AddListener(UpdateItems);
            }
            if (horizontalScrollbar != null)
            {

            }
        }
        private void UpdateItems(float ratio)
        {
            if (onUpdateScroll != null) onUpdateScroll.Invoke(ratio);
        }

        public float ViewPort
        {
            get { return scrollRect.verticalNormalizedPosition; }
            set { scrollRect.verticalNormalizedPosition = value; }
        }
    }
}