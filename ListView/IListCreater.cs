using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
namespace ListView
{
    public interface IListCreater<T> where T:MonoBehaviour,IListItem {
        event UnityAction<T> onVisiable;
        event UnityAction<T> onInViesiable;
        void CreateItems(int totalCount);
        void AddItem();
        void RemoveItem(T item);
        void ClearOldItems();
    }
}