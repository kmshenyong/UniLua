using GrinderApp.Configuration.Store.Json;
using GrinderApp.Configuration.StreamProvider;
using GrinderApp.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrinderApp.Configuration
{
    /// <summary>
    /// 配置参数管理器
    /// </summary>
    public static class ConfigFactory
    {
        /// <summary>
        /// 应用程序的全局配置参数对象
        /// </summary>
        private static Config _config;

        /// <summary>
        /// MachineSetting 创建锁
        /// </summary>
        private static readonly object _configLock = new object();

        /// <summary>
        /// 大机参数
        /// </summary>
        public static Config Config
        {
            get
            {
                if (_config == null)
                {
                    lock (_configLock)
                    {
                        if (_config == null)
                        {
                            //var baseFolder = new MachineFolderService();
                          //  var path = baseFolder.GetPreparedFolder(AppFolder.Setting);
                            var path= Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                            Log.Information($"Loading settings from {path}");
                            var filePath = Path.Combine(path, "setting.json");

                            // 如果没有配置文件，使用工厂设置参数
                            var fileExist = File.Exists(filePath);
                            var config = new Config(new JsonConfigStore(new FileStreamProvider(filePath)));

                            // 使用工厂设置覆盖(如果有)
                            if (!fileExist)
                                MergeWithFactory(config);

                            _config = config;
                        }
                    }
                }

                return _config;
            }
        }

        /// <summary>
        /// 复位所有的设置
        /// </summary>
        public static void Reset()
        {
            try
            {
                // 清空系统的设置
                Config.Clear();

                // 使用工厂设置覆盖(如果有)
                MergeWithFactory(Config);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Cannot reset config settings");
            }
        }

        /// <summary>
        /// 从工厂默认参数恢复
        /// </summary>
        /// <param name="config">设置参数</param>
        private static void MergeWithFactory(Config config)
        {
            var factoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, "factory-setting.json");
            if (File.Exists(factoryPath))
            {
                var factorySetting = new Config(new JsonConfigStore(new FileStreamProvider(factoryPath)));
                config.MergeWith(factorySetting, true);
            }
        }
    }
}
