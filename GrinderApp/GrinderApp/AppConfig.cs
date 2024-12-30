using GrinderApp.Configuration;
using GrinderApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrinderApp
{
    public class AppConfig : ConfigBase, IAppConfig
    {
        public AppConfig(Config config) : base(config)
        {

        }
        #region Overrides of ConfigBase

        public TValue GetValueOrThrow<TValue>(string path)
        {
            return Config.GetValueOrThrow<TValue>(path);
        }

        /// <summary>
        /// 定义作用域(包)
        /// 在多个配置文件合并的时候，用来区分不同的作用域
        /// </summary>
      
        public override string PathName => "GrinderApp";

        #endregion

        /// <summary>
        /// full screen mode.
        /// we cache this value because invoke high frequency by WndProc.
        /// </summary>
        [Description("全屏显示模式 ")]
        public bool FullScreenMode
        {
            get => GetPropertyValue(false);
            set => SetPropertyValue(value);
        }
        /// <summary>
        /// full screen mode.
        /// we cache this value because invoke high frequency by WndProc.
        /// </summary>
        [Description("plc ip ")]
        public string  PLcIpAddress
        {
            get => GetPropertyValue("192.168.0.5");
            set => SetPropertyValue(value);
        }
    }
}
