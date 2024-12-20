using System;
using System.Threading;
using System.Threading.Tasks;

namespace grinder.Configuration.Helper
{
    /// <summary>
    /// 只允许一个等待着的任务队列，该队列允许一个执行者工作中，同时一个等待着在等待并进入工作
    /// 如果一个任务入队，并且发现已经有一个等待着，则返回False
    /// </summary>
    /// <remarks>场景：配置文件许多模块要求写入磁盘，如果当前有一个等待着等待写入，那么就把这件事情交给他吧，因为大家都是调用相同方法写入文件</remarks>
    public class OneWaiterTaskQueue
    {
        /// <summary>
        /// 等待者
        /// </summary>
        private readonly SemaphoreSlim _waiter;

        /// <summary>
        /// 执行者
        /// </summary>
        private readonly SemaphoreSlim _executor;

        public OneWaiterTaskQueue()
        {
            _waiter   = new SemaphoreSlim(1);
            _executor = new SemaphoreSlim(1);
        }

        /// <summary>
        /// 是否有人在等待
        /// </summary>
        public bool HasWaiter => _waiter.CurrentCount == 0;

        /// <summary>
        /// 是否拥有异常
        /// </summary>
        public bool HasFaulted => LastException != null;

        /// <summary>
        /// 返回最后一个错误
        /// </summary>
        public Exception LastException { get; private set; }

        /// <summary>
        /// 尝试入队执行，如果已经有一个等待着，放弃入队尝试
        /// </summary>
        /// <param name="taskGenerator"></param>
        /// <returns></returns>
        public bool TryEnqueue(Func<Task> taskGenerator)
        {
            // 如果已经拥有一个等待者，return false；
            if (_waiter.Wait(0) == false)
                return false;

            Task.Run(async () =>
            {
                // 得到等待权，开始等待执行
                await _executor.WaitAsync();

                // 得到执行权，我不再是等待者，释放等待权信号，让给下一个等待者
                _waiter.Release();

                try
                {
                    await taskGenerator();
                }
                catch (Exception ex)
                {
                    LastException = ex;
                }
                finally
                {
                    // 释放执行信号
                    _executor.Release();
                }
            });

            return true;
        }
    }
}
