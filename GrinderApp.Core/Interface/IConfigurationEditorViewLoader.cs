using GrinderApp.Configuration;

namespace GrinderApp.Core.Interface
{
    /// <summary>
    /// 配置文件编辑器
    /// </summary>
    public interface IConfigurationEditorViewLoader
    {
        void Show(string regionName, params ConfigSectionView[] configSectionView);
        public void Show(string regionName);
    }
}
