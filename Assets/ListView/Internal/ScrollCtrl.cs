using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ListView.Internal
{
    public class ScrollCtrl<T> where T : MonoBehaviour, IListItem
    {
        private ScrollRect scrollRect;
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
            scrollRect.onValueChanged.AddListener((vec) =>
            {
                switch (dir)
                {
                    case Direction.Vertical:
                        UpdateItems(vec.y);
                        break;
                    case Direction.Horizontal:
                        UpdateItems(vec.x);
                        break;
                    default:
                        break;
                }
            });
            
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