﻿using System.Windows;
using ConfigurationEditor.Helper;

namespace ConfigurationEditor.ValueEdit
{
    /// <summary>
    /// Interaction logic for ConfigurationEdit.xaml
    /// </summary>
    public partial class ValueEditWidow : Window, IDialogWindow
    {
        /// <summary>
        /// </summary>
        public ValueEditWidow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 视图模型
        /// </summary>
        public ValueEditWidowViewModel ViewModel => DataContext as ValueEditWidowViewModel;

        /// <summary>
        /// 对话框确定关闭
        /// </summary>
        public void Ok()
        {
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// 对话框取消关闭
        /// </summary>
        public void Cancel()
        {
            DialogResult = false;
            Close();
        }
    }
}
