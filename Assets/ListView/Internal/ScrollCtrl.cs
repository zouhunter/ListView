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
        private Direction dir;

        public ScrollCtrl(ScrollRect scrollRect, Direction dir)
        {
            this.scrollRect = scrollRect;
            this.dir = dir;
            RegistScrollEvent();
        }

        private void RegistScrollEvent()
        {
            switch (dir)
            {
                case Direction.Vertical:
                    verticalScrollbar.onValueChanged.AddListener(UpdateItems);
                    break;
                case Direction.Horizontal:
                    horizontalScrollbar.onValueChanged.AddListener(UpdateItems);
                    break;
                default:
                    break;
            }
        }
        private void UpdateItems(float ratio)
        {
            if (onUpdateScroll != null) onUpdateScroll.Invoke(dir==Direction.Vertical ? ratio:1-ratio);
        }

        public float NormalizedPosition
        {
            get { return dir == Direction.Vertical ? scrollRect.verticalNormalizedPosition: 1 - scrollRect.horizontalNormalizedPosition; }
            set { if (dir == Direction.Vertical)
                    scrollRect.verticalNormalizedPosition = value;
                else scrollRect.horizontalNormalizedPosition = 1 - value;
            }
        }
    }
}