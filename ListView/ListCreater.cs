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
        public List<T> CreatedItems { get { return createdItems; } }

        public UnityAction<T> onVisiable;
        public UnityAction<T> onInViesiable;

        private ScrollRect scrollRect;
        private RectTransform content { get { return scrollRect.content; } }
        private RectTransform viewport { get { return scrollRect.viewport; } }

        private T pfb { get; set; }
        private List<T> createdItems = new List<T>();
        private ObjectPool<T> _objectPool;
        private ContentCtrl<T> _contentCtrl;
        private ScrollCtrl<T> _scrollCtrl;
        private int totalCount;
        private int startID;
        private int endID;
        public ListCreater(ScrollRect scrollRect, T pfb)
        {
            Debug.Assert(scrollRect);
            Debug.Assert(pfb);

            this.scrollRect = scrollRect;
            this.pfb = pfb;
            pfb.gameObject.SetActive(false);

            _objectPool = new ObjectPool<T>();
            _contentCtrl = new ContentCtrl<T>(scrollRect, pfb.GetComponent<RectTransform>());
            _scrollCtrl = new ScrollCtrl<T>(scrollRect);
            _scrollCtrl.onUpdateScroll = UpdateItems;
        }

        public void CreateItemsAsync(int totalCount)
        {
            scrollRect.verticalNormalizedPosition = 1;
            scrollRect.horizontalNormalizedPosition = 1;
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
                this.startID = 0;
                this.endID = -1;
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
            _scrollCtrl.ViewPort = 0;
            ShowAnItem(false);
            int start;
            int end;
            _contentCtrl.CalcuateIndex(_scrollCtrl.ViewPort, totalCount, out start, out end);
            if (startID < start)
            {
                HideAnItem(true);
            }
        }

        public void RemoveItem(T item)
        {
            _contentCtrl.SetContent(--totalCount);
            _objectPool.SavePoolObject(item, false);
            createdItems.Remove(item);
            if (onInViesiable != null) onInViesiable.Invoke(item);
            for (int i = 0; i < createdItems.Count; i++)
            {
                if (createdItems[i].Id > item.Id)
                {
                    createdItems[i].Id--;
                    _contentCtrl.SetPosition(createdItems[i]);
                    if (onVisiable != null) onVisiable(createdItems[i]);
                }
            }
            endID--;
            int start;
            int end;
            _contentCtrl.CalcuateIndex(_scrollCtrl.ViewPort,totalCount, out start, out end);
            if (end > endID)
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
            foreach (var item in createdItems)
            {
                RemoveItem(item);
            }
            createdItems.Clear();
            totalCount = 0;
        }

        private void ShowArea(int start, int end)
        {
            ///移出超出顶部
            if (start > startID)
            {
                var count = start - startID;
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
            if (end < endID)
            {
                var count = endID - end;
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
            if (!head && endID == totalCount) return null;
            if (head && startID == 0) return null;
            Debug.Log("Show:" + (head ? "Head" : "End"));
            T scr = _objectPool.GetPoolObject(pfb, content, false);
            createdItems.Insert(!head ? createdItems.Count : 0, scr);
            scr.Id = !head ? ++endID : --startID;
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
            if (createdItems.Count == 0) return null;
            T item = null;
            if (head)
            {
                item = createdItems[0];
                startID++;
            }
            else
            {
                item = createdItems[createdItems.Count - 1];
                endID--;
            }
            _objectPool.SavePoolObject(item, false);
            createdItems.Remove(item);
            if (onInViesiable != null) onInViesiable.Invoke(item);
            return item;
        }

        private void RefeshConnect(bool hidehead, int count)
        {
            //隐藏同时显示
            for (int i = 0; i < count; i++)
            {
                if (createdItems.Count == 0) return;
                if (hidehead && endID == totalCount) return;
                if (!hidehead && startID == 0) return;

                T itemSwith = null;
                if (hidehead)
                {
                    itemSwith = createdItems[0];
                    startID++;
                    endID++;
                    itemSwith.Id = endID;
                }
                else
                {
                    itemSwith = createdItems[createdItems.Count - 1];
                    startID--;
                    endID--;
                }
                if (onInViesiable != null) onInViesiable.Invoke(itemSwith);

                createdItems.Remove(itemSwith);
                itemSwith.Id = hidehead ? endID : startID;
                createdItems.Insert(hidehead ? createdItems.Count : 0, itemSwith);
                _contentCtrl.SetPosition(itemSwith);
                if (onVisiable != null) onVisiable(itemSwith);
            }
        }
        private void RefeshJump(bool hidehead, int count)
        {
            if (hidehead && endID == totalCount) return;
            if (!hidehead && startID == 0) return;

            if (hidehead)
            {
                startID += count;
                endID += count;
            }
            else
            {
                startID -= count;
                endID -= count;
            }

            for (int i = 0; i < _contentCtrl.BestCount; i++)
            {
                T item = null;
                if (hidehead)
                {
                    item = createdItems[0];
                }
                else
                {
                    item = createdItems[createdItems.Count - 1];
                }
                createdItems.Remove(item);
                if (onInViesiable != null) onInViesiable.Invoke(item);
                _objectPool.SavePoolObject(item, false);
            }

            for (int i = 0; i < _contentCtrl.BestCount; i++)
            {
                T item = _objectPool.GetPoolObject(pfb, content, false);
                createdItems.Add(item);
                item.Id = startID + i;
                _contentCtrl.SetPosition(item);
                if (onVisiable != null) onVisiable(item);
            }
        }
    }
}