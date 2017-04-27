using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ListView.Internal;

namespace ListView
{
    public class ListCreater<T> where T : MonoBehaviour, IListItem
    {
        public List<T> VisiableItems { get { return _createdItems; } }

        public UnityAction<T> onVisiable;
        public UnityAction<T> onInViesiable;

        private ScrollRect scrollRect;
        private RectTransform parent { get { return scrollRect.content; } }

        private T pfb { get; set; }
        private List<T> _createdItems = new List<T>();
        private ObjectPool<T> _objectPool;
        private ContentCtrl<T> _contentCtrl;
        private ScrollCtrl<T> _scrollCtrl;
        private int totalCount;
        private int _startID;
        private int _endID;
        private Direction dir;
        public ListCreater(ScrollRect scrollRect, T pfb,Direction dir)
        {
            Debug.Assert(scrollRect);
            Debug.Assert(pfb);

            this.scrollRect = scrollRect;
            this.pfb = pfb;
            pfb.gameObject.SetActive(false);

            _objectPool = new ObjectPool<T>();
            _contentCtrl = new ContentCtrl<T>(scrollRect, pfb.GetComponent<RectTransform>(),dir);
            _scrollCtrl = new ScrollCtrl<T>(scrollRect,dir);
            _scrollCtrl.onUpdateScroll = UpdateItems;
        }

        public void CreateItemsAsync(int totalCount)
        {
            _scrollCtrl.NormalizedPosition = 1;
            scrollRect.StartCoroutine(DelyCreate(totalCount));
        }

        IEnumerator DelyCreate(int totalCount)
        {
            ClearOldItems();
            yield return new WaitForEndOfFrame();
            this.totalCount = totalCount;
            _contentCtrl.SetContent(totalCount);
            if (_contentCtrl.BestCount >= 0 || totalCount >= 0)
            {
                this._startID = 0;
                this._endID = -1;
                float count = Mathf.Min(_contentCtrl.BestCount, totalCount);
                for (int i = 0; i < count; i++)
                {
                    ShowAnItem(false);
                }
            }
        }

        public void UpdateItems(float ratio)
        {
            int start;
            int end;
            _contentCtrl.CalcuateIndex(ratio,totalCount, out start, out end);
            ShowArea(start, end);
        }

        public void AddItem()
        {
            _contentCtrl.SetContent(++totalCount);
            _scrollCtrl.NormalizedPosition = 0;
            ShowAnItem(false);
            int start;
            int end;
            _contentCtrl.CalcuateIndex(_scrollCtrl.NormalizedPosition, totalCount, out start, out end);
            Debug.Log("start:" + start + "StartID" + _startID);
            if (_startID < start)
            {
                HideAnItem(true);
            }
        }

        public void RemoveItem(T item)
        {
            _contentCtrl.SetContent(--totalCount);
            _objectPool.SavePoolObject(item, false);
            _createdItems.Remove(item);
            if (onInViesiable != null) onInViesiable.Invoke(item);
            for (int i = 0; i < _createdItems.Count; i++)
            {
                if (_createdItems[i].Id > item.Id)
                {
                    _createdItems[i].Id--;
                    _contentCtrl.SetPosition(_createdItems[i]);
                    if (onVisiable != null) onVisiable(_createdItems[i]);
                }
            }
            _endID--;
            int start;
            int end;
            _contentCtrl.CalcuateIndex(_scrollCtrl.NormalizedPosition,totalCount, out start, out end);
            if (end > _endID)
            {
                ShowAnItem(false);
            }
            else
            {
                ShowAnItem(true);
            }
        }

        public void ClearOldItems()
        {
            foreach (var item in _createdItems)
            {
                RemoveItem(item);
            }
            _createdItems.Clear();
            totalCount = 0;
        }

        private void ShowArea(int start, int end)
        {
            ///移出超出顶部
            if (start > _startID || end > _endID)
            {
                var count = start - _startID;
                ///当移动在可视范围内，连续移动
                if (count < _contentCtrl.BestCount)
                {
                    RefeshConnect(true, count);
                }
                ///当一次移动最较大时，跳跃移动
                else
                {
                    RefeshJump(true, count);
                }
            }
            ///移除超出底部
            if (end < _endID|| start <_startID)
            {
                var count = _endID - end;
                ///当移动在可视范围内，连续移动
                if (count < _contentCtrl.BestCount)
                {
                    RefeshConnect(false, count);
                }
                ///当一次移动最较大时，跳跃移动
                else
                {
                    RefeshJump(false, count);
                }
            }
        }

        /// <summary>
        /// 显示出一条，可以后接也可以前置
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        private T ShowAnItem(bool head)
        {
            if (!head && _endID == totalCount) return null;
            if (head && _startID == 0) return null;
            Debug.Log("Show:" + (head ? "Head" : "End"));
            T scr = _objectPool.GetPoolObject(pfb, parent, false);
            _createdItems.Insert(!head ? _createdItems.Count : 0, scr);
            scr.Id = !head ? ++_endID : --_startID;
            _contentCtrl.SetPosition(scr);
            if (onVisiable != null) onVisiable(scr);
            return scr;
        }

        /// <summary>
        /// 隐藏掉一条
        /// </summary>
        /// <param name="head"></param>
        private T HideAnItem(bool head)
        {
            Debug.Log("Hide:" + (head ? "Head" : "End"));
            if (_createdItems.Count == 0) return null;
            T item = null;
            if (head)
            {
                item = _createdItems[0];
                _startID++;
            }
            else
            {
                item = _createdItems[_createdItems.Count - 1];
                _endID--;
            }
            _objectPool.SavePoolObject(item, false);
            _createdItems.Remove(item);
            if (onInViesiable != null) onInViesiable.Invoke(item);
            return item;
        }

        private void RefeshConnect(bool hidehead, int count)
        {
            //隐藏同时显示
            for (int i = 0; i < count; i++)
            {
                if (_createdItems.Count == 0) return;
                if (hidehead && _endID == totalCount) return;
                if (!hidehead && _startID == 0) return;

                T itemSwith = null;
                if (hidehead)
                {
                    itemSwith = _createdItems[0];
                    _startID++;
                    _endID++;
                    itemSwith.Id = _endID;
                }
                else
                {
                    itemSwith = _createdItems[_createdItems.Count - 1];
                    _startID--;
                    _endID--;
                }
                if (onInViesiable != null) onInViesiable.Invoke(itemSwith);

                _createdItems.Remove(itemSwith);
                itemSwith.Id = hidehead ? _endID : _startID;
                _createdItems.Insert(hidehead ? _createdItems.Count : 0, itemSwith);
                _contentCtrl.SetPosition(itemSwith);
                if (onVisiable != null) onVisiable(itemSwith);
            }
        }
        private void RefeshJump(bool hidehead, int count)
        {
            //if (hidehead && _endID == totalCount) return;
            //if (!hidehead && _startID == 0) return;

            if (hidehead)
            {
                _startID += count;
                _endID += count;
            }
            else
            {
                _startID -= count;
                _endID -= count;
            }

            for (int i = 0; i < _contentCtrl.BestCount; i++)
            {
                T item = null;
                if (hidehead)
                {
                    item = _createdItems[0];
                }
                else
                {
                    item = _createdItems[_createdItems.Count - 1];
                }
                _createdItems.Remove(item);
                if (onInViesiable != null) onInViesiable.Invoke(item);
                _objectPool.SavePoolObject(item, false);
            }

            for (int i = 0; i < _contentCtrl.BestCount; i++)
            {
                T item = _objectPool.GetPoolObject(pfb, parent, false);
                _createdItems.Add(item);
                item.Id = _startID + i;
                _contentCtrl.SetPosition(item);
                if (onVisiable != null) onVisiable(item);
            }
        }
    }
}