using Microsoft.Web.WebView2.Wpf;
using System.Windows;

namespace LobbyReveal.UI.Windows
{
    /// <summary>
    /// Interaction logic for WebView2Browser.xaml
    /// </summary>
    public partial class WebView2Browser : Window
    {
        public WebView2 WView2 { get; set; }
        public WebView2Browser()
        {
            InitializeComponent();
            WView2 = webv2;
            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = -10000; // some location off-screen
            Top = -10000;
            ShowInTaskbar = false;
            ShowActivated = false;
        }
    }
}
