using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MSLabs.Service.Models;
using MSLabs.Service.Services;
using MSLabs.WpfApp.Helpers;

namespace MSLabs.WpfApp
{
    /// <summary>
    /// Interaction logic for LZWPage.xaml
    /// </summary>
    public partial class LZWPage : Page
    {
        private readonly BackgroundWorker worker;
        private Frame navigationFrame;
        private LZWCompressionDataModel model;
        private LZWCompressionResultModel result;

        public LZWPage(Frame navigationFrame)
        {
            this.InitializeComponent();
            this.navigationFrame = navigationFrame;

            this.worker = new BackgroundWorker();
            this.worker.DoWork += this.DoCompressionAsync;
            this.worker.RunWorkerCompleted += this.DoCompressionCompleted;

            this.CompressionStatus.Visibility = Visibility.Visible;
            this.NoConnection.Visibility = Visibility.Visible;

            if (InternetAvailability.GetIsInternetAvailable())
            {
                this.InfoFrame.Navigate(new Uri("https://en.wikipedia.org/wiki/Lempel%E2%80%93Ziv%E2%80%93Welch", UriKind.Absolute));
                this.NoConnection.Visibility = Visibility.Collapsed;
                this.CompressionStatus.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.NoConnection.Visibility = Visibility.Visible;
                this.CompressionStatus.Visibility = Visibility.Collapsed;
            }

            this.ToggleComponents(true, false);
        }

        private void Perform_LWZ_Compression_Button_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleComponents(false, true);
            this.CompressionStatus.Visibility = Visibility.Visible;
            this.PerformActionButton.IsEnabled = false;
            this.model = new LZWCompressionDataModel
            {
                DataToCompress = this.PlainText.Text
            };

            this.worker.RunWorkerAsync();
        }

        private void DoCompressionAsync(object sender, DoWorkEventArgs e)
        {
            this.result = LZWCompressionService.PerformLZWCompression(this.model);
        }

        private void DoCompressionCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.CompressionStatus.Visibility = Visibility.Collapsed;

            this.CompressedTextDecimal.Text = this.result.GetValuesOfCompressedData();
            this.CompressedTextHexadecimal.Text = this.result.GetHexValuesOfCompressedData();
            this.CompressedTextBinary.Text = this.result.GetBinaryValuesOfCompressedData();

            var rand = new Random();
            this.IndexDictionary.Children.Clear();
            this.result.GetCodeIndexDictionary().ForEach(x =>
                this.IndexDictionary.Children.Add(new TextBlock
                {
                    Text = string.Format("{0} => {1}", x.Key, x.Value),
                    MinWidth = 150.0,
                    Background = new SolidColorBrush(Color.FromArgb(128, (byte)rand.Next(200, 255), (byte)rand.Next(200, 255), (byte)rand.Next(200, 255)))
                }));

            this.PlainCount.Text = this.result.PlainTextBitsCount.ToString();
            this.CompressedCount.Text = this.result.CompressedTextBitsCount.ToString();
            this.CompressionDegree.Text = this.result.CompressionDegree.ToString();

            var message = "The compression is done.You can see the compression result in 'Compression Text (dec, hex, bin)' " +
                "tabs, you can see the indexes in 'Index Dictionary' tab, and some statistic in 'Statistics' tab.";

            MessageBoxResult dialoresult = MessageBox.Show(message, "Compression Successful.", button: MessageBoxButton.OK);

            this.PerformActionButton.IsEnabled = true;
        }

        private void PlainText_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.PerformActionButton.IsEnabled = this.PlainText.Text.Length > 0;
            this.ToggleComponents(true, false);
        }

        private void ToggleComponents(bool isHintVisible, bool isDataVisible)
        {
            this.CompressedTextDecimalError.Visibility = isHintVisible ? Visibility.Visible : Visibility.Collapsed;
            this.CompressedTextDecimal.Visibility = isDataVisible ? Visibility.Visible : Visibility.Collapsed;

            this.CompressedTextHexadecimalError.Visibility = isHintVisible ? Visibility.Visible : Visibility.Collapsed;
            this.CompressedTextHexadecimal.Visibility = isDataVisible ? Visibility.Visible : Visibility.Collapsed;

            this.CompressedTextBinaryError.Visibility = isHintVisible ? Visibility.Visible : Visibility.Collapsed;
            this.CompressedTextBinary.Visibility = isDataVisible ? Visibility.Visible : Visibility.Collapsed;

            this.IndexDictionaryError.Visibility = isHintVisible ? Visibility.Visible : Visibility.Collapsed;
            this.IndexDictionary.Visibility = isDataVisible ? Visibility.Visible : Visibility.Collapsed;

            this.StatisticsPanelError.Visibility = isHintVisible ? Visibility.Visible : Visibility.Collapsed;
            this.StatisticsPanel.Visibility = isDataVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
