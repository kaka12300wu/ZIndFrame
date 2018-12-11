using UnityEngine;
using System.Collections;


namespace ZFrameWork
{
    /// <summary>
    /// 对象超时数据，销毁的对象在多久之后自动销毁
    /// </summary>
    public class PoolItemTime
    {
        /// <summary>
        /// 对象
        /// </summary>
        public GameObject gameObject;

        /// <summary>
        /// 存取时间
        /// </summary>
        public float expireTime;

        /// <summary>
        /// 销毁状态，外部不要随便修改这个值
        /// </summary>
        public bool destoryStatus;

        public PoolItemTime(GameObject gameObject)
        {
            this.gameObject = gameObject;
            this.destoryStatus = false;
        }

        /// <summary>
        /// 激活对象
        /// </summary>
        public GameObject Active()
        {
            this.gameObject.SetActive(true);
            this.destoryStatus = false;
            return this.gameObject;
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        public void Destory()
        {
            // 重置角色状态
            this.gameObject.SetActive(false);

            this.destoryStatus = true;
            this.expireTime = Time.time;
        }

        /// <summary>
        /// 是否超时
        /// </summary>
        /// <value><c>true</c> if this instance is expire; otherwise, <c>false</c>.</value>
        public bool IsExpire()
        {
            if (!this.destoryStatus) return false;
            if (Time.time - this.expireTime >= PoolManager.EXPIRE_TIME) return true;
            return false;
        }
    }
}