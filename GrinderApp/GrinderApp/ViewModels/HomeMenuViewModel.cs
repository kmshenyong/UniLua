using GrinderApp.Configuration;
using GrinderApp.Core;
using GrinderApp.Core.Interface;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace GrinderApp.ViewModels
{
    public class HomeMenuViewModel : BindableBase, INavigationAware
    {
        Config config;
        public HomeMenuViewModel(IEnumerable<IModuleViewLoader> moduleViewLoaders,
             Config config
        )
        {
            this.config = config;
            foreach (var moduleViewLoader in moduleViewLoaders)
            {
                //if (moduleViewLoader is ISettingModuleLoader)
                //{
                //    settingModuleLoader = moduleViewLoader as ISettingModuleLoader;
                //}
                //if (moduleViewLoader is IWorkingModuleLoader)
                //{
                //    workingModuleLoader = moduleViewLoader as IWorkingModuleLoader;
                //}
                //if (moduleViewLoader is IJogsModuleLoader)
                //{
                //    jogsModuleLoader = moduleViewLoader as IJogsModuleLoader;
                //}
                if (moduleViewLoader is IConfigurationEditorViewLoader)
                {
                    configurationEditorViewLoader = moduleViewLoader as IConfigurationEditorViewLoader;
                }
                //if (moduleViewLoader is IDevelopmentModuleLoader)
                //{
                //    developmentModuleLoader = moduleViewLoader as IDevelopmentModuleLoader;
                //}
                //if (moduleViewLoader is IBackDoorModuleLoader)
                //{
                //    backDoorModuleLoader = moduleViewLoader as IBackDoorModuleLoader;
                //}
                //if (moduleViewLoader is IScriptEditorModuleLoader)
                //{
                //    scriptEditorModuleLoader = moduleViewLoader as IScriptEditorModuleLoader;
                //}

                //if (moduleViewLoader is ILogViewLoader)
                // {
                //     logViewLoader = moduleViewLoader as ILogViewLoader;
                // }
            }

            
        }
        private bool _isRunning=false  ;
        /// <summary>
        /// 设备是否在运行
        /// </summary>
        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public IConfigurationEditorViewLoader configurationEditorViewLoader { get; }

        private DelegateCommand<IViewLoader> _launchCommand;
        public DelegateCommand<IViewLoader> LaunchCommand =>
            _launchCommand ?? (_launchCommand = new DelegateCommand<IViewLoader>(ExecuteLaunchCommand, CanExecuteLaunchCommand))
        .ObservesProperty(() => IsRunning);
        void ExecuteLaunchCommand(IViewLoader viewLoader)
        {
            try
            {
                viewLoader?.Show(RegionNames.ContentRegion);
            }
            catch (Exception e)
            {

                throw;
            }

        }

        bool CanExecuteLaunchCommand(IViewLoader viewLoader)
        {
            if (viewLoader == null)
                return false;

            // 允许任何时候运行的模块
            //if (viewLoader is IAlarmMessageViewLoader)
            //    return true;
            /*
            // 作业模块
            if (viewLoader is IWorkingModuleLoader)
            {
                //note: 为使PLC 通讯故障时还能进入作业，进行手动解算和测试取消进入作业限制
                //return !IsMeasureRunning && WorkingState != RunningStateTable.WorkingStates.Failure;
                return true;
            }

            // Setting 
            if (viewLoader is ISettingModuleLoader)
            {
                //  return !IsTampingJobRunning && !IsRecorderRunning && WorkingState != RunningStateTable.WorkingStates.Failure;
                return true;
            }
            if (viewLoader is ISettingModuleLoader)
            {
                //  return !IsTampingJobRunning && !IsRecorderRunning && WorkingState != RunningStateTable.WorkingStates.Failure;
                return true;
            }
            //手动
            if (viewLoader is IConfigurationEditorViewLoader)
            {
                //  return !IsTampingJobRunning && !IsRecorderRunning && WorkingState != RunningStateTable.WorkingStates.Failure;
                return true;
            }

            // 开发测试
            if (viewLoader is IDevelopmentModuleLoader)
            {
                //  return !IsTampingJobRunning && !IsRecorderRunning && WorkingState != RunningStateTable.WorkingStates.Failure;
                return true;
            }

            //// 记录仪模块
            //if (viewLoader is IRecorderViewLoader)
            //{
            //    // return !IsMeasureRunning && RecorderState != RunningStateTable.RecorderStates.Failure;
            //    return true;
            //}

            // 其他普通模块
            // return !IsTampingJobRunning && !IsMeasureRunning && !IsRecorderRunning;
            */
            return true;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsRunning = true;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
           // throw new NotImplementedException();
           return true ;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //
        }
    }

}
