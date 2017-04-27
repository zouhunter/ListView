using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ListView
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
                return Mathf.CeilToInt(TotalHeight / SingleHeight);
            }
        }
        private float _totaltop;
        public float TotalTop
        {
            get
            {
                viewport.GetWorldCorners(viewPortWorldPos);
                _totaltop = viewPortWorldPos[1].y;
                return _totaltop;
            }
           
        }
        private float _totaldown;
        public float TotalDown
        {
            get
            {
                viewport.GetWorldCorners(viewPortWorldPos);
                _totaldown = viewPortWorldPos[0].y;
                return _totaldown;
            }
        }
        private int count;
        public ContentCtrl(ScrollRect scrollRect, RectTransform childItem)
        {
            this.scrollRect = scrollRect;
            this.childItem = childItem;
        }
        /// <summary>
        /// 设置显示区域大小
        /// </summary>
        /// <param name="count"></param>
        public void SetContent(int count)
        {
            this.count = count;
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, SingleHeight * count);
        }
        /// <summary>
        /// 按比例获取区域
        /// </summary>
        /// <param name="ratio"></param>
        /// <param name="startID"></param>
        /// <param name="endID"></param>
        public void CalcuateIndex(float ratio,int maxcount,out int startID,out int endID)
        {
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
            Vector3 startPos = Vector3.down * SingleHeight * 0.5f;
            item.transform.localPosition = Vector3.down * item.Id *SingleHeight + startPos;
        }
    }
}