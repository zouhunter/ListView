using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ListView.Internal
{
    public class ContentCtrl<T> where T : MonoBehaviour, IListItem
    {
        private ScrollRect scrollRect;
        private RectTransform content { get { return scrollRect.content; } }
        private RectTransform viewport { get { return scrollRect.viewport; } }
    
        private RectTransform childItem;
        private Vector3[] viewPortWorldPos = new Vector3[4];
        private Vector3[] itemWorldPos = new Vector3[4];
        private float _totalHeight;
        public float TotalHeight {
            get {
                viewport.GetLocalCorners(viewPortWorldPos);
                _totalHeight = viewPortWorldPos[1].y - viewPortWorldPos[0].y;
                return _totalHeight;
            }
        }
        private float _singleHeight;
        public float SingleHeight {
            get {
                childItem.GetWorldCorners(itemWorldPos);
                _singleHeight = itemWorldPos[1].y - itemWorldPos[0].y;
                return _singleHeight;
            }
        }
        public int BestCount
        {
            get{
                if (dir == Direction.Vertical)
                    return Mathf.CeilToInt(TotalHeight / SingleHeight) + 1;
                else
                    return Mathf.CeilToInt(TotalWidth / SingleWidth) + 1;
            }
        }

        private float _totalWidth;
        public float TotalWidth
        {
            get
            {
                viewport.GetLocalCorners(viewPortWorldPos);
                _totalWidth = viewPortWorldPos[2].x - viewPortWorldPos[1].x;
                return _totalWidth;
            }
        }
        private float _singleWidth;
        public float SingleWidth
        {
            get
            {
                childItem.GetWorldCorners(itemWorldPos);
                _singleWidth = itemWorldPos[2].x - itemWorldPos[1].x;
                return _singleWidth;
            }
        }
 
        private int count;
        private Direction dir;

        public ContentCtrl(ScrollRect scrollRect, RectTransform child, Direction dir)
        {
            this.scrollRect = scrollRect;
            this.childItem = child;
            this.dir = dir;
        }

        /// <summary>
        /// 设置显示区域大小
        /// </summary>
        /// <param name="count"></param>
        public void SetContent(int count)
        {
            this.count = count;
            content.SetSizeWithCurrentAnchors(dir == Direction.Vertical ? RectTransform.Axis.Vertical: RectTransform.Axis.Horizontal,
                (dir == Direction.Vertical ? SingleHeight :SingleWidth)* count);
        }
        /// <summary>
        /// 按比例获取区域
        /// </summary>
        /// <param name="ratio"></param>
        /// <param name="startID"></param>
        /// <param name="endID"></param>
        public void CalcuateIndex(float ratio,int maxcount,out int startID,out int endID)
        {
            //float ratio1 = dir == Direction.Vertical ? 1 - ratio : ratio;
            startID = Mathf.FloorToInt((1-ratio) * (count - BestCount + 1));
            startID = startID < 0 ? 0 : startID;
            endID = BestCount + startID - 1;
            endID = endID > maxcount-1 ? maxcount-1 : endID;
            startID = endID - BestCount + 1;
        }
        /// <summary>
        /// 设置指定对象的坐标
        /// </summary>
        /// <param name="item"></param>
        public void SetPosition(T item)
        {
            if (dir == Direction.Vertical)
            {
                Vector3 startPos = Vector3.down * SingleHeight * 0.5f;
                item.transform.localPosition = Vector3.down * item.Id * SingleHeight + startPos;
            }
            else
            {
                Vector3 startPos = Vector3.right * SingleWidth * 0.5f;
                item.transform.localPosition = Vector3.right * item.Id * SingleWidth + startPos;
            }

        }
    }
}