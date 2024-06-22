using ICSharpCode.AvalonEdit.Document;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using UniLua;
using static System.Windows.Forms.AxHost;

namespace UniLuaEditor.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        ILuaState Lua;
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        TextDocument code = new TextDocument();
        public TextDocument LuaCode
        {
            get { return code; }
            set { SetProperty(ref code, value); }
        }
        public MainWindowViewModel()
        {
            Lua = new LuaState();
            Lua.L_OpenLibs();
        }


        public ICommand StartCommand => (new DelegateCommand(() =>
        {
            Lua.L_DoString(LuaCode.Text);
        }));


        #region 文件相关命令处理
        public async Task SaveFileAsync(string content, string filePath, CancellationToken cancel = default)
        {
            // 打开文件
            await using var stream = File.Create(filePath);
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(content);
            await writer.FlushAsync();
        }

        public async Task<string> OpenFileAsync(string filePath, CancellationToken cancel = default)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            // 打开文件
            await using var stream = File.OpenRead(filePath);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
        #region 保存文件
        /// <summary>
        /// 保存文件
        /// </summary>
        public ICommand SaveFileCommand => (new DelegateCommand(async () =>
         {        // 读取默认文件
             var _saveFileDialog = new SaveFileDialog
             {
                 AddExtension = true,
                 Title = "保存脚本 ",
                 Filter = "机器人控制命令(.lua)|*.lua",
                 DefaultExt = "lua",
                 FilterIndex = 0,
                 FileName = "新建命令",
             };

             // 让用户选择文件
             if (_saveFileDialog.ShowDialog() != true) return;// 用户放弃
                                                            // 执行存储
             var filePath = _saveFileDialog.FileName;
             await SaveFileAsync(LuaCode.Text, filePath);
         }));
        //      }).ObservesCanExecute(() => CanExecuteSaveFileCommand));
        //   public bool CanExecuteSaveFileCommand => State == DebugState.Stopped /* todo and file has changed*/;
      
        public ICommand ExitAppCommand =>(new DelegateCommand (()=>
        { 
            App.Current.Shutdown(0);
            Environment.Exit(0);
        }));
        #endregion

        #region 读取文件 
        /// <summary>
        /// 打开文件
        /// </summary>
        public ICommand OpenFileCommand => new DelegateCommand(async () =>
        {
            var _openFileDialog = new OpenFileDialog();
            _openFileDialog.DefaultExt = "";
            _openFileDialog.Filter = "机器人控制命令(.lua)|*.lua";
            _openFileDialog.Title = "打开命令";
            if (_openFileDialog.ShowDialog() != true) return;
            string filePath = _openFileDialog.FileName;
            if (!File.Exists(filePath))
                return;
            LuaCode.Text = await OpenFileAsync(filePath);
            LuaCode.FileName = filePath;

        });
        //  }).ObservesCanExecute(() => CanExecuteOpenFileCommand));

        //  public bool CanExecuteOpenFileCommand => State == DebugState.Stopped;
        
        #endregion

        #region 新建文件

        /// <summary>
        /// 新建文件命令
        /// </summary>
        public ICommand NewFileCommand => new DelegateCommand(() =>
        {
            LuaCode.FileName = "Untitled";
            LuaCode.Text = "";
            // GCodeEditor.ScrollToHome();
        });
      //  }).ObservesCanExecute(() => CanExecuteNewFileCommand));

      //  public bool CanExecuteNewFileCommand => State == DebugState.Stopped;
        #endregion
        #endregion
    }
}
