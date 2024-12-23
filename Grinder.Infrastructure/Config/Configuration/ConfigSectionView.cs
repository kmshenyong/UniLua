using GrinderApp.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrinderApp.Configuration
{

    public class ConfigSectionView
    {
        public string Name
        {
            get;
        }

        public ConfigSection Section
        {
            get;
        }

        public ConfigSectionView(string name, ConfigSection section)
        {
            Name = name;
            Section = section;
        }
    }
}
