using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ListView.Internal
{
    public class ObjectPool<T> where T : MonoBehaviour, IListItem
    {
        private List<T> objectPool = new List<T>();

        public T GetPoolObject(T pfb, Transform parent, bool world, bool resetLocalPosition = true, bool resetLocalScale = false, bool activepfb = false)
        {
            pfb.gameObject.SetActive(true);
            //遍历每数组，得到一个隐藏的对象
            for (int i = 0; i < objectPool.Count; i++)
            {
                if (!objectPool[i].gameObject.activeSelf)
                {
                    objectPool[i].gameObject.SetActive(true);
                    objectPool[i].transform.SetParent(parent, world);
                    if (resetLocalPosition)
                    {
                        objectPool[i].transform.localPosition = Vector3.zero;
                    }
                    if (resetLocalScale)
                    {
                        objectPool[i].transform.localScale = Vector3.one;
                    }
                    pfb.gameObject.SetActive(activepfb);
                    return objectPool[i];
                }
            }
            //当没有隐藏对象时，创建一个并返回
            T currGo = CreateOne(pfb, parent, world, resetLocalPosition, resetLocalScale);
            objectPool.Add(currGo);
            pfb.gameObject.SetActive(activepfb);
            return currGo;
        }

        public void SavePoolObject(T go, bool world = false)
        {
            if (!objectPool.Contains(go))
            {
                objectPool.Add(go);
            }
            go.gameObject.SetActive(false);
        }

        public T CreateOne(T pfb, Transform parent, bool world, bool resetLocalPositon, bool resetLocalScale)
        {
            T currentGo = GameObject.Instantiate(pfb);
            currentGo.name = pfb.name;
            currentGo.transform.SetParent(parent, world);
            if (resetLocalPositon)
            {
                currentGo.transform.localPosition = Vector3.zero;
            }
            if (resetLocalScale)
            {
                currentGo.transform.localScale = Vector3.one;
            }
            return currentGo;
        }
    }
}