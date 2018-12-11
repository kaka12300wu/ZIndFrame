using UnityEngine;
using System.Collections.Generic;


namespace ZFrameWork
{
    /// <summary>
    /// 池数据
    /// </summary>
    public class PoolItem
    {
        /// <summary>
        /// 路径，对象标识
        /// </summary>
        public string path;

        /// <summary>
        /// 对象列表
        /// </summary>
        private Dictionary<int, PoolItemTime> objectHash;

        /// <summary>
        /// 源物体
        /// </summary>
        private GameObject srcObj;

        public PoolItem(string path,GameObject _src)
        {
            this.path = path;
            this.srcObj = _src;
            this.objectHash = new Dictionary<int, PoolItemTime>();
        }

        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="gameObject">Game object.</param>
        public void PushObject(GameObject gameObject)
        {
            int hashKey = gameObject.GetHashCode();
            if (!this.objectHash.ContainsKey(hashKey))
            {
                this.objectHash.Add(hashKey, new PoolItemTime(gameObject));
            }
            else
            {
                this.objectHash[hashKey].Active();
            }
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        /// <param name="gameObject">Game object.</param>
        public void DestoryObject(GameObject gameObject)
        {
            int hashKey = gameObject.GetHashCode();
            if (this.objectHash.ContainsKey(hashKey))
            {
                this.objectHash[hashKey].Destory();
            }
        }

        /// <summary>
        /// 使一个Object脱离缓存池
        /// </summary>
        /// <param name="gameObject"></param>
        public void FreedomObject(GameObject gameObject)
        {
            int hashKey = gameObject.GetInstanceID();
            if(objectHash.ContainsKey(hashKey))
            {
                objectHash.Remove(hashKey);
            }
        }

        /// <summary>
        /// 获取未真正销毁的对象（池对象）
        /// </summary>
        public GameObject GetObject()
        {
            if (this.objectHash == null || this.objectHash.Count == 0) return null;

            foreach (PoolItemTime poolItemTime in this.objectHash.Values)
            {
                if (poolItemTime.destoryStatus)
                    return poolItemTime.Active();
            }

            GameObject newItem = GameObject.Instantiate(srcObj);
            PoolItemTime newPoolCell = new PoolItemTime(newItem);
            objectHash.Add(newItem.GetHashCode(), newPoolCell);
            return newPoolCell.Active();
        }

        /// <summary>
        /// 移除并且销毁对象
        /// </summary>
        /// <param name="gameObject">Game object.</param>
        public void RemoveObject(GameObject gameObject)
        {
            int hashKey = gameObject.GetHashCode();
            if (this.objectHash.ContainsKey(hashKey))
            {
                GameObject.Destroy(gameObject);
                this.objectHash.Remove(hashKey);
            }
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        public void Destory()
        {
            IList<PoolItemTime> poolList = new List<PoolItemTime>();
            foreach (PoolItemTime poolItemTime in this.objectHash.Values)
            {
                poolList.Add(poolItemTime);
            }
            while (poolList.Count > 0)
            {
                if (poolList[0] != null && poolList[0].gameObject != null)
                {
                    GameObject.Destroy(poolList[0].gameObject);
                    poolList.RemoveAt(0);
                }
            }
            this.objectHash = new Dictionary<int, PoolItemTime>();
            GameObject.Destroy(srcObj);
        }

        /// <summary>
        /// 超时检测
        /// </summary>
        public void ExpireObject()
        {
            IList<PoolItemTime> expireList = new List<PoolItemTime>();
            foreach (PoolItemTime poolItemTime in this.objectHash.Values)
            {
                if (poolItemTime.IsExpire()) expireList.Add(poolItemTime);
            }
            int expireCount = expireList.Count;
            for (int index = 0; index < expireCount; index++)
            {
                this.RemoveObject(expireList[index].gameObject);
            }
        }
    }
}
