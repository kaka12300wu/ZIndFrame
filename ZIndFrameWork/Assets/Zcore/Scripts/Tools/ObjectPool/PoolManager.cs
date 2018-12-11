using UnityEngine;
using System.Collections.Generic;


namespace ZFrameWork
{
    public class PoolManager
    {
        /// <summary>
        /// 超时时间
        /// </summary>
        public const int EXPIRE_TIME = 1 * 10;

        private static Dictionary<string, PoolItem> poolHash;
        private static Dictionary<int, string> pathHash;

        public static void Init()
        {
            poolHash = new Dictionary<string, PoolItem>();
            pathHash = new Dictionary<int, string>();
        }

        /// <summary>
        /// 添加一个缓存池
        /// </summary>
        /// <param name="path"></param>
        /// <param name="gameObject"></param>
        public static void AddPool(string path,GameObject gameObject)
        {
            if (poolHash == null) poolHash = new Dictionary<string, PoolItem>();
            if(!poolHash.ContainsKey(path))
            {
                poolHash.Add(path, new PoolItem(path, gameObject));
            }
            else
            {
                Debug.LogError("Can not add pool item as key: " + path);
            }
        }

        /// <summary>
        /// 移除一个缓存池
        /// </summary>
        /// <param name="path"></param>
        public static void RemovePool(string path)
        {
            if(poolHash.ContainsKey(path))
            {
                poolHash[path].Destory();
                poolHash.Remove(path);
            }
        }

        /// <summary>
        /// 使一个Obj获得自由，可以用于把一个Obj脱离缓存池管理
        /// </summary>
        /// <param name="gameObject"></param>
        public static void FreedomObj(GameObject gameObject)
        {
            if(pathHash.ContainsKey(gameObject.GetInstanceID()))
            {
                string pathKey = pathHash[gameObject.GetInstanceID()];
                if(poolHash.ContainsKey(pathKey))
                {
                    poolHash[pathKey].FreedomObject(gameObject);
                }
                pathHash.Remove(gameObject.GetInstanceID());
            }
        }

        /// <summary>
        /// 获取一个指定key的Obj
        /// </summary>
        /// <param name="path"></param>
        public static GameObject GetObj(string path)
        {
            if (poolHash.ContainsKey(path))
            {
                GameObject gameObject = poolHash[path].GetObject();
                int hashKey = gameObject.GetInstanceID();
                pathHash.Add(hashKey, path);
                return gameObject;
            }
            return null;
        }

        public static T GetObjComp<T>(string path)where T : Component
        {
            GameObject o = GetObj(path);
            if (null != o)
                return o.GetComponent<T>();
            return null;
        }
        
        /// <summary>
        /// 把一个Obj还给缓存池
        /// </summary>
        /// <param name="gameObject"></param>
        public static void PushObj(GameObject gameObject)
        {
            int hashKey = gameObject.GetInstanceID();
            if(pathHash.ContainsKey(hashKey))
            {
                string path = pathHash[hashKey];
                pathHash.Remove(hashKey);
                if(poolHash.ContainsKey(path))
                {
                    poolHash[path].PushObject(gameObject);
                }
                else
                {
                    GameObject.Destroy(gameObject);
                }
            }
        }

        /// <summary>
        /// 处理超时的对象
        /// </summary>
        public static void ExpireObject()
        {
            // 筛选符合条件的数据
            foreach (PoolItem poolItem in poolHash.Values)
            {
                poolItem.ExpireObject();
            }
        }

        /// <summary>
        /// 销毁所有缓存池
        /// </summary>
        public static void Destroy()
        {
            foreach (PoolItem poolItem in poolHash.Values)
            {
                poolItem.Destory();
            }
            poolHash.Clear();
            poolHash = null;
            pathHash.Clear();
            pathHash = null;
        }

    }
}