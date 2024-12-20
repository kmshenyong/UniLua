using System;
using System.IO;

namespace grinder.Configuration.StreamProvider
{
    /// <summary>
    /// 内存流提供者，主要用于单元测试
    /// </summary>
    public class MemoryStreamProvider : IStreamProvider
    {
        private MemoryStream _stream = new MemoryStream();

        public MemoryStreamProvider()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public MemoryStreamProvider(MemoryStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _stream = stream;
        }

        /// <summary>
        /// 内存流
        /// </summary>
        public byte[] Buffer => _stream.ToArray();

        #region Implementation of IStreamProvider

        /// <summary>
        /// 以读取的方式打开流
        /// </summary>
        /// <returns></returns>
        public Stream OpenRead()
        {
            var buffer = _stream.ToArray();
            var stream = new MemoryStream(buffer, false);

            return stream;
        }

        /// <summary>
        /// 为写入打开流
        /// </summary>
        /// <returns></returns>
        public Stream OpenWrite()
        {
            _stream = new MemoryStream();
            return _stream;
        }

        #endregion
    }
}
