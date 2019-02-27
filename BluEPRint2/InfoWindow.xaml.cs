using System.Windows;
using System.Windows.Navigation;

namespace BluEPRint2
{
    /// <summary>
    /// Interaktionslogik für InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
        //In AssemblyInfo.cs [assembly: AssemblyVersion("...")]
        public string AssemblyVersion
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
