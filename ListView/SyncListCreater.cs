using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ListView.Internal;
namespace ListView
{
    public class SyncListItemCreater<T> : IListCreater<T> where T : MonoBehaviour, IListItem
    {
        public event UnityAction<T> onVisiable;
        public event UnityAction<T> onInViesiable;
        public List<T> CreatedItems { get { return _createdItems; } }
        Transform parent { get; set; }
        T pfb { get; set; }
        private ObjectPool<T> _objectManager;
        List<T> _createdItems = new List<T>();
        public SyncListItemCreater(Transform parent, T pfb)
        {
            this.parent = parent;
            this.pfb = pfb;
            pfb.gameObject.SetActive(false);
            _objectManager = new ObjectPool<T>(parent,pfb);
        }

        public void CreateItems(int length)
        {
            ClearOldItems();
            if (length <= 0) return;

            T go;
            for (int i = 0; i < length; i++)
            {
                go = _objectManager.GetPoolObject();
                T scr = go.GetComponent<T>();
                scr.Id = i;
                if (onVisiable != null) onVisiable.Invoke(scr);
                _createdItems.Add(scr);
            }
        }

        public void AddItem()
        {
            if (pfb == null) return;
            T scr;
            scr = _objectManager.GetPoolObject();
            scr.Id = CreatedItems.Count;
            _createdItems.Add(scr);
        }

        public void RemoveItem(T item)
        {
            if (onInViesiable != null) onInViesiable.Invoke(item);
            for (int i = 0; i < _createdItems.Count; i++)
            {
                if (_createdItems[i].Id > item.Id)
                {
                    _createdItems[i].Id--;
                }
            }
            _createdItems.Remove(item);
            _objectManager.SavePoolObject(item);
        }

        public void ClearOldItems()
        {
            while(_createdItems.Count > 0) {
                RemoveItem(_createdItems[0]);
            }
        }
    }

}
