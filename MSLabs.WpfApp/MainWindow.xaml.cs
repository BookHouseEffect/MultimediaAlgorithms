using System.Windows;

namespace MSLabs.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.MainFrame.Navigate(new MenuPage(this.MainFrame));
        }
    }
}
