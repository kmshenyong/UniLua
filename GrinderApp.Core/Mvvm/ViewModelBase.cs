using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace GrinderApp.Core.Mvvm
{

    /// <summary>
    /// ViewModel 的基类
    /// </summary>
    /// <remarks>
    /// IDestructible 接口不靠谱, 不要使用, 不当使用会导致内存泄露
    /// </remarks>
    public abstract class ViewModelBase : BindableBase
    {
        /// <summary>
        /// 视图模型对象Id
        /// </summary>
        public string InstanceId { get; }

        protected ViewModelBase()
        {
            InstanceId = $"{Guid.NewGuid().ToString().ToUpper()[..8]}-{GetType().Name}";

            // 初始化 ObservesProperty
            InitializeObservesPropertyChanged();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            // 激发 ObservesProperty
            UpdateObservesPropertyChanged(args);

            base.OnPropertyChanged(args);
        }

        #region Event subscribe
        
        /// <summary>
        /// 保存 Token
        /// </summary>
        private readonly ConcurrentDictionary<TrackingPeriod, ConcurrentBag<EventSubscriptionHolder>> _eventTokens =
            new();

        /// <summary>
        /// 保持订阅的 Token
        /// </summary>
        /// <param name="holder"></param>
        public SubscriptionToken AddSubscribedToken(EventSubscriptionHolder holder)
        {
            var period = IsLoaded == true ? TrackingPeriod.OnUnloaded : TrackingPeriod.OnDestroy;
            return AddSubscribedToken(holder, period);
        }

        /// <summary>
        /// 保持事件的订阅状态
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="period"></param>
        public SubscriptionToken AddSubscribedToken(EventSubscriptionHolder holder, TrackingPeriod period)
        {
            if (holder == null)
                throw new ArgumentNullException(nameof(holder));

            var list = _eventTokens.GetOrAdd(period, new ConcurrentBag<EventSubscriptionHolder>());
            list.Add(holder);

            return holder.Token;
        }

        /// <summary>
        /// 取消指定周期的 Token 订阅
        /// </summary>
        public void UnsubscribeTokens(TrackingPeriod period)
        {
            if (!_eventTokens.TryRemove(period, out var list))
                return;

            while (list.TryTake(out var handler))
            {
                handler.Token.Dispose();
            }
        }


        /// <summary>
        /// 取消所有的 Token 订阅
        /// </summary>
        public void ReleaseAllSubscribedTokens()
        {
            var keys = _eventTokens.Keys.ToArray();
            foreach (var key in keys)
            {
                UnsubscribeTokens(key);
            }
        }
        
        #endregion

        #region Cached property

        /// <summary>
        /// 保存缓存的属性值
        /// </summary>
        private readonly ConcurrentDictionary<string, object> _propertyCached = new();


        /// <summary>
        /// 获取缓存的属性值, 这个版本性能更好
        /// 用法:
        /// public ICommand TestCommand => GetPropertyCached(new DelegateCommand(.....));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected T GetPropertyCached<T>(Func<T> cache, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("PropertyName cannot be null or empty.", nameof(propertyName));

            return (T)_propertyCached.GetOrAdd(propertyName, _ => cache());
        }

        #endregion

        #region Loaded or Unloaded

        /// <summary>
        /// 绑定至视图
        /// </summary>
        /// <param name="view"></param>
        public void BindingView(FrameworkElement view)
        {
            _view = view;

            if (view.IsLoaded)
            {
                OnLoaded();
            }

            view.Loaded -= FrameworkElement_Loaded;
            view.Loaded += FrameworkElement_Loaded;

            view.Unloaded -= FrameworkElement_Unloaded;
            view.Unloaded += FrameworkElement_Unloaded;
        }

        /// <summary>
        /// 解除与视图的绑定
        /// </summary>
        /// <param name="view"></param>
        public void UnbindingView(FrameworkElement view)
        {
            view.Loaded -= FrameworkElement_Loaded;
            view.Unloaded -= FrameworkElement_Unloaded;
            _view = null;
        }

        /// <summary>
        /// 是否被加载
        /// </summary>
        public bool? IsLoaded
        {
            get
            {
                if (_view is not { } view)
                    return false;

                if (view.Dispatcher.CheckAccess())
                    return view.IsLoaded;

                bool isLoaded = false;
                view.Dispatcher.Invoke(() => isLoaded = view.IsLoaded);

                return isLoaded;
            }
        }

        /// <summary>
        /// 绑定的视图
        /// </summary>
        private FrameworkElement _view;

        private void FrameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            OnLoaded();
        }

        private void FrameworkElement_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                OnUnloaded();
            }
            finally
            {
                // 释放 Token, 如果用户重写了 OnLoaded 但是没有 base.OnLoaded(), 这里是用来兜底
                UnsubscribeTokens(TrackingPeriod.OnUnloaded);
            }
        }

        /// <summary>
        /// 加载时执行, 确保 BindingLoadedUnloadedAction 被 View 构造函数调用
        /// </summary>
        public virtual void OnLoaded()
        {
        }

        /// <summary>
        /// 卸载时执行, 确保 BindingLoadedUnloadedAction 被 View 构造函数调用
        /// </summary>
        public virtual void OnUnloaded()
        {
        }

        #endregion

        #region ObservesProperty

        /// <summary>
        /// 保存更改观察字典表
        /// </summary>
        private Dictionary<string, string[]> _observesDict;

        /// <summary>
        /// 初始化属性变更激发观察者
        /// </summary>
        private void InitializeObservesPropertyChanged()
        {
            _observesDict = CreateObservesPropertyDictionaryOrOpenCache(GetType());
        }

        /// <summary>
        /// 属性更改观察字典 Cache
        /// </summary>
        private static readonly Dictionary<Type, Dictionary<string, string[]>> _observesDictCache =
            new Dictionary<Type, Dictionary<string, string[]>>();

        /// <summary>
        /// 创建或者从缓存打开属性观察字典, 为了提高效率，一次创建完成的字典，将被多个对象复用
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Dictionary<string, string[]> CreateObservesPropertyDictionaryOrOpenCache(Type type)
        {
            // 返回已有字典
            if (_observesDictCache.TryGetValue(type, out var result))
                return result;

            // 新建字典缓存并返回
            var dict = CreateObservesPropertyDictionary(type);
            _observesDictCache.Add(type, dict);

            return dict;
        }

        /// <summary>
        /// 创建指定类型的属性更改观察字典
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Dictionary<string, string[]> CreateObservesPropertyDictionary(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var observesList = new List<KeyValuePair<string, string>>();

            var props = type.GetProperties();
            foreach (var prop in props)
            {
                // 获取属性上的所有如下声明
                // [ObservesProperty(nameof(FirstName)]
                // [ObservesProperty(nameof(LastName)]
                var attrs = prop.GetCustomAttributes(typeof(ObservesPropertyAttribute), true)
                    .OfType<ObservesPropertyAttribute>().ToArray();

                // 去重
                var propertyNames = attrs.Select(c => c.PropertyName).Distinct();

                // 转换为 FirstName => Name    LastName => Name 的结构
                var list = from name in propertyNames
                           select new KeyValuePair<string, string>(name, prop.Name);

                // 存储观察映射结构
                observesList.AddRange(list);
            }

            // 存储观察字典
            var dict = (from item in observesList
                        group item by item.Key
                into g
                        select g).ToDictionary(c => c.Key, c => c.Select(v => v.Value).ToArray());

            // 检查循环依赖
            CheckCircleDependency(dict);

            return dict;
        }

        /// <summary>
        /// 递归检查循环引用的声明情况，防止程序后续崩溃.
        /// </summary>
        /// <param name="dependencyDict"></param>
        private static void CheckCircleDependency(Dictionary<string, string[]> dependencyDict)
        {
            // 存储当前属性和，当前属性的路径
            // 例如： (Name, [LastName, Name])
            Stack<(string, string[])> stack = new Stack<(string, string[])>();

            // 初始化递归栈变量
            foreach (var item in dependencyDict)
            {
                stack.Push((item.Key, new string[0]));
            }

            // 递归处理
            while (stack.Any())
            {
                // 判断依据：对于当前属性，如果他的某个子节点已经存在于路径 parentPathOfDependency 中，则判断循环依赖
                // 例如路径 A.B.C.D,  验证目标 X 的子节点数组 [A,Y,Z] 中，A在他的parentPathOfDependency已经存在，则判断循环依赖

                // 属性名，当前属性的依赖路径
                var (propertyName, parentPathOfDependency) = stack.Pop();

                // 尝试获取检查属性节点的子节点
                if (!dependencyDict.TryGetValue(propertyName, out var children))
                    continue;

                // 递归检查每一个子节点
                foreach (var child in children)
                {
                    // 检查 child 节点是否存在依赖
                    if (parentPathOfDependency.Contains(child))
                        throw new ArgumentException($"{string.Join("-", parentPathOfDependency)}-{child}");

                    // 递归向下检查
                    stack.Push((child, parentPathOfDependency.Union(new[] { child }).ToArray()));
                }
            }
        }

        private void UpdateObservesPropertyChanged(PropertyChangedEventArgs args)
        {
            // 如果该属性订阅了事件通知的依赖属性
            if (_observesDict.TryGetValue(args.PropertyName, out var dependencyPropertyNames))
            {
                foreach (var propertyName in dependencyPropertyNames)
                {
                    RaisePropertyChanged(propertyName);
                }
            }
        }

        #endregion
    }
    /// <summary>
    /// 事件跟踪的时间段
    /// </summary>
    public enum TrackingPeriod
    {
        OnUnloaded,
        OnDestroy,
    }
    //public abstract class _ViewModelBase : BindableBase, IDestructible
    //{
    //    protected _ViewModelBase()
    //    {
    //        InstanceId = $"{Guid.NewGuid().ToString().ToUpper()[..8]}-{GetType().Name}";

    //        // 初始化 ObservesProperty
    //       // InitializeObservesPropertyChanged();
    //    }
    //    /// <summary>
    //    /// 视图模型对象Id
    //    /// </summary>
    //    public string InstanceId { get; }

    //    public virtual void Destroy()
    //    {

    //    }
    //}
}
