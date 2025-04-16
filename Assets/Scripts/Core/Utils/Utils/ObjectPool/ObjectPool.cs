using System.Collections.Generic;
using UnityEngine;

namespace CDTU.Utils
{
    /// <summary>
    ///     通用对象池实现
    ///     用途：
    ///     1. 管理和重用Unity对象，减少实例化和销毁的性能开销
    ///     2. 自动管理对象的激活/关闭状态
    ///     3. 支持预热和动态扩容
    /// </summary>
    /// <typeparam name="T">要池化的对象类型，必须继承自UnityEngine.Object</typeparam>
    public class ObjectPool<T> where T : Object
    {
        #region 构造函数

        /// <summary>
        ///     对象池构造函数
        /// </summary>
        /// <param name="prefab">要池化的预制体</param>
        /// <param name="defaultSize">池的初始大小</param>
        /// <param name="parent">对象的父级Transform</param>
        /// <param name="collectionChecks">是否启用重复回收检查</param>
        public ObjectPool(T prefab, int defaultSize = 10, Transform parent = null, bool collectionChecks = true)
        {
            _prefab = prefab;
            _defaultSize = Mathf.Max(0, defaultSize);
            _parent = parent;
            _collectionChecks = collectionChecks;

            _pool = new Queue<T>(_defaultSize);
            _activeObjects = new HashSet<T>();

            // 预热对象池
            Warmup(_defaultSize);
        }

        #endregion

        #region 私有方法

        /// <summary>
        ///     创建新对象
        /// </summary>
        private T CreateNewObject()
        {
            var obj = Object.Instantiate(_prefab, _parent);
            if (obj is GameObject gameObj) gameObj.SetActive(false);
            return obj;
        }

        #endregion

        #region 私有字段

        /// <summary>
        /// 存储未使用的池化对象的队列，当Get方法被调用时从队列前端获取对象，Release时将对象添加到队列末端
        /// </summary>
        private readonly Queue<T> _pool;

        /// <summary>
        /// 对象池使用的原始预制体，所有池化对象都基于此预制体实例化
        /// </summary>
        private readonly T _prefab;

        /// <summary>
        /// 池化对象的父级Transform，所有池化对象都将被设置为此Transform的子物体
        /// </summary>
        private readonly Transform _parent;

        /// <summary>
        /// 对象池的默认初始容量，决定预热时创建的对象数量
        /// </summary>
        private readonly int _defaultSize;

        /// <summary>
        /// 是否启用重复回收检查，启用后可防止同一对象被多次回收或回收非池内对象
        /// </summary>
        private readonly bool _collectionChecks = true;

        /// <summary>
        /// 当前处于活跃状态（已从池中取出但尚未回收）的对象集合，用于跟踪和验证对象状态
        /// </summary>
        private readonly HashSet<T> _activeObjects;

        #endregion

        #region 公共属性

        /// <summary>
        ///     当前池中可用对象数量
        /// </summary>
        public int CountInactive => _pool.Count;

        /// <summary>
        ///     当前活跃对象数量
        /// </summary>
        public int CountActive => _activeObjects.Count;

        /// <summary>
        ///     总对象数量
        /// </summary>
        public int CountAll => CountInactive + CountActive;

        #endregion

        #region 公共方法

        /// <summary>
        ///     预热对象池，预先创建指定数量的对象
        /// </summary>
        /// <param name="count">要预创建的对象数量</param>
        public void Warmup(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var obj = CreateNewObject();
                _pool.Enqueue(obj);
            }
        }

        /// <summary>
        ///     从对象池获取一个对象
        /// </summary>
        /// <returns>可用的对象</returns>
        public T Get()
        {
            T obj;
            if (_pool.Count > 0)
                obj = _pool.Dequeue();
            else
                obj = CreateNewObject();

            _activeObjects.Add(obj);

            if (obj is GameObject gameObj) gameObj.SetActive(true);

            return obj;
        }

        /// <summary>
        ///     释放对象回对象池
        /// </summary>
        /// <param name="obj">要释放的对象</param>
        public void Release(T obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("尝试回收空对象到对象池");
                return;
            }

            if (_collectionChecks)
            {
                if (_pool.Contains(obj))
                {
                    Debug.LogError($"尝试多次回收同一个对象到对象池: {obj}");
                    return;
                }

                if (!_activeObjects.Remove(obj))
                {
                    Debug.LogError($"尝试回收不是从此对象池获取的对象: {obj}");
                    return;
                }
            }

            if (obj is GameObject gameObj) gameObj.SetActive(false);

            _pool.Enqueue(obj);
        }

        /// <summary>
        ///     清空对象池
        /// </summary>
        /// <param name="destroyActive">是否也销毁当前活跃的对象</param>
        public void Clear(bool destroyActive = false)
        {
            // 清理池中的非活跃对象
            while (_pool.Count > 0)
            {
                var obj = _pool.Dequeue();
                Object.Destroy(obj);
            }

            // 如果需要，也清理活跃对象
            if (destroyActive)
            {
                foreach (var obj in _activeObjects) Object.Destroy(obj);
                _activeObjects.Clear();
            }
        }

        #endregion
    }
}