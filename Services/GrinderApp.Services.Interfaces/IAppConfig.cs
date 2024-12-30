using System;
using System.Collections.Generic;
using System.Text;

namespace GrinderApp.Services.Interfaces
{
    public interface IAppConfig
    {
        public bool FullScreenMode { get; set; }
        public string PLcIpAddress
        { get; set; }
    }   
}
