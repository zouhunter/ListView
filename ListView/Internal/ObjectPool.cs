using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ListView.Internal
{
    public class ObjectPool<T> where T : MonoBehaviour, IListItem
    {
        private T pfb;
        private Transform parent;
        private List<T> objectPool = new List<T>();
        public ObjectPool(Transform parent,T pfb){
            this.pfb = pfb;
            this.parent = parent;
        }
        public T GetPoolObject()
        {
            pfb.gameObject.SetActive(true);
            //遍历每数组，得到一个隐藏的对象
            for (int i = 0; i < objectPool.Count; i++)
            {
                if (!objectPool[i].gameObject.activeSelf)
                {
                    objectPool[i].gameObject.SetActive(true);
                    objectPool[i].transform.SetParent(parent, false);
                    pfb.gameObject.SetActive(false);
                    return objectPool[i];
                }
            }
            //当没有隐藏对象时，创建一个并返回
            T currGo = CreateOne();
            objectPool.Add(currGo);
            pfb.gameObject.SetActive(false);
            return currGo;
        }

        public void SavePoolObject(T go)
        {
            if (!objectPool.Contains(go))
            {
                objectPool.Add(go);
            }
            go.gameObject.SetActive(false);
        }

        public T CreateOne()
        {
            T currentGo = GameObject.Instantiate(pfb);
            currentGo.name = pfb.name;
            currentGo.transform.SetParent(parent, false);
            return currentGo;
        }
        public void DeleteAll()
        {
            foreach (var item in objectPool) {
                GameObject.Destroy(item.gameObject);
            }
            objectPool.Clear();
        }
    }
}