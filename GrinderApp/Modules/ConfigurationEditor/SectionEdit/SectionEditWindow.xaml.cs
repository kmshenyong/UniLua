using ConfigurationEditor.Helper;

namespace ConfigurationEditor.SectionEdit
{
    /// <summary>
    /// Interaction logic for SectionEditView.xaml
    /// </summary>
    public partial class SectionEditWindow : IDialogWindow
    {
        public SectionEditWindow()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                SectionNameTextBox.Focus();
            };
        }

        public SectionEditWindowViewModel ViewModel => (SectionEditWindowViewModel) DataContext;

        public void Ok()
        {
            DialogResult = true;
            Close();
        }

        public void Cancel()
        {
            DialogResult = false;
            Close();
        }
    }
}
