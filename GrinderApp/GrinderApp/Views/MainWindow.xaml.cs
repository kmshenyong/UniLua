using System.Diagnostics.CodeAnalysis;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using MahApps.Metro.Controls;
using GrinderApp.Core;
using Prism.Navigation.Regions;
using Unity;
using GrinderApp.Services.Interfaces;

namespace GrinderApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        IRegionManager regionManager;
        IUnityContainer _unityContainer;
        IAppConfig appConfig;
        public MainWindow(IUnityContainer unityContainer,
            IAppConfig appConfig)
        {
            InitializeComponent();
            _unityContainer = unityContainer;
            regionManager = _unityContainer.Resolve<IRegionManager>();
            this.appConfig = appConfig;
            var h = appConfig.PLcIpAddress;
        }

        #region full screen mode

        /// <summary>
        /// full screen mode.
        /// we cache this value because invoke high frequency by WndProc.
        /// </summary>
        public bool FullScreenMode
        {
            get
            {
                return appConfig.FullScreenMode;

            }
            set
            {

                appConfig.FullScreenMode = value;
            }
        }
        /// <summary>
        /// 更新全屏模式的风格
        /// </summary>
        private void UpdateStyleWithFullScreenMode(bool fullScreenMode)
        {


            // 全屏控制
            if (fullScreenMode) // 全屏
            {
                ShowMaxRestoreButton = false;                 // hide max restore button
                ShowMinButton = false;                 // hide min button
                ShowCloseButton = false;                 // hide close button
                IgnoreTaskbarOnMaximize = true;                  // cover task bar in full screen mode
                WindowState = WindowState.Maximized; // toggle maximize mode
                ResizeMode = ResizeMode.NoResize;   // prevent window restore by [WIN]+[LEFT] keys
                IsWindowDraggable = false;                 // prevent title be drag
            }
            else
            {
                ShowMaxRestoreButton = true;
                ShowMinButton = true;
                ShowCloseButton = true;
                IgnoreTaskbarOnMaximize = false;
                WindowState = WindowState.Normal;
                ResizeMode = ResizeMode.CanResizeWithGrip;
                Width = 1280;
                Height = 800;
                IsWindowDraggable = true;
            }
        }

        /// <summary>
        /// 在拖动标题移动窗口的过程中，对 IsWindowDraggable 的一个补充，默认的，如果标题条上有 TextBlock， 这些区域无法拖动
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // 对 IsWindowDraggable 的一个补充，默认的，如果标题条上有 TextBlock， 这些区域无法拖动
            if (!FullScreenMode && e.ChangedButton == MouseButton.Left)
            {
                var position = e.GetPosition(this);
                if (position.Y < TitleBarHeight)
                {
                    DragMove();
                }
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// register windows message function: WndProc
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(WndProc);
        }

        /// <summary>
        /// process window message
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        [SuppressMessage("ReSharper", "CommentTypo")]
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_SYSCOMMAND = 0x0112; // WM_SYSCOMMAND message
            const int SC_RESTORE = 0xF120; // SC_RESTORE from WM_SYSCOMMAND in wParam

            switch (msg)
            {
                case WM_SYSCOMMAND:
                    if (wParam.ToInt32() == SC_RESTORE)
                    {
                        // prevent restore from maximize by [WIN]+[DOWN]
                        if (FullScreenMode && WindowState == WindowState.Maximized)
                        {
                            handled = true;
                        }
                    }

                    break;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// 防止用户使用其他方式、技巧、工具在全屏模式复原了窗体
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStateChanged(EventArgs e)
        {
            // 全屏模式，必须有这句话，否则用户强制 Restore 就毁了
            if (FullScreenMode && WindowState == WindowState.Maximized)
            {
                IsWindowDraggable = false;
                ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                IsWindowDraggable = true;
                ResizeMode = ResizeMode.CanResizeWithGrip;
            }

            base.OnStateChanged(e);
        }


        #endregion

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateStyleWithFullScreenMode(FullScreenMode);
            regionManager.RequestNavigate(RegionNames.ContentRegion, nameof(HomeMenu));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        { //  禁止退出
            e.Cancel = true;
            // regionManager.RequestNavigate(RegionNames.ContentRegion, nameof(WelcomeView));
            WindowState = WindowState.Minimized;

            return;
        }
    }
}
