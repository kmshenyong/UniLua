using System;
using System.Threading.Tasks;
using FirstLineTamping.Configuration;
using FirstLineTamping.Configuration.Helper;
using NUnit.Framework;

namespace ConfigurationTests
{
    [TestFixture()]
    public class OneWaiterTaskQueueTests
    {
        /// <summary>
        /// 测试任务队列正确性
        /// </summary>
        [Test()]
        public void TryEnqueueTest()
        {
            var queue = new OneWaiterTaskQueue();

            // 第一个任务入队
            var result = queue.TryEnqueue(LongTimeTask);
            Assert.IsTrue(result);

            // 第二个任务入队
            result = queue.TryEnqueue(LongTimeTask);
            Assert.IsTrue(result);
            Assert.IsTrue(queue.HasWaiter);

            // 第三个任务入队
            result = queue.TryEnqueue(LongTimeTask);
            Assert.IsFalse(result);
        }

        public async Task LongTimeTask()
        {
            // 在单元测试函数完成前，不要退出
            await Task.Delay(10000000);
        }

        /// <summary>
        /// 测试任务队列遇到异常情况
        /// </summary>
        [Test()]
        public void TryEnqueueHasException()
        {
            var queue = new OneWaiterTaskQueue();

            var ex = new NotSupportedException("Not Supported");

            var result = queue.TryEnqueue(()=>ExceptionTask(ex));
            Assert.IsTrue(result);
            Assert.IsTrue(queue.HasFaulted);
            Assert.AreEqual(ex, queue.LastException);
        }

        public Task ExceptionTask(NotSupportedException ex)
        {
            throw ex;
        }

    }
}
