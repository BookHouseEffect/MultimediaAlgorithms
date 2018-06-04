using System.Windows;
using System.Windows.Controls;

namespace MSLabs.WpfApp
{
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        private Frame navigationFrame;

        public MenuPage(Frame navigationFrame)
        {
            this.InitializeComponent();
            this.navigationFrame = navigationFrame;
            this.navigationFrame.NavigationService.RemoveBackEntry();
        }

        private void LZW_Button_Click(object sender, RoutedEventArgs e)
        {
            this.navigationFrame.Navigate(new LZWPage(this.navigationFrame));
        }

        private void AC_Button_Click(object sender, RoutedEventArgs e)
        {
            this.navigationFrame.Navigate(new ACPage(this.navigationFrame));
        }
    }
}
