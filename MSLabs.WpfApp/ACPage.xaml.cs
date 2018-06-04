using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MSLabs.Repository.Calculator;
using MSLabs.Repository.Calculator.Entities;
using MSLabs.Service.Models;
using MSLabs.Service.Services;
using MSLabs.WpfApp.Helpers;

namespace MSLabs.WpfApp
{
    /// <summary>
    /// Interaction logic for ACPage.xaml
    /// </summary>
    public partial class ACPage : Page
    {
        private readonly BackgroundWorker worker;
        private Frame navigationFrame;
        private string regexExpression = "(^0\\.[0-9]+$)|(^1\\.0+$)|(^[1-9][0-9]*\\/[1-9][0-9]*$)|(^0\\/[1-9][0-9]*)";
        private ArithmeticCodingDataModel model;
        private ArithmeticCodingRangeModel result;
        private string lowerFractionResult = string.Empty;
        private string upperFractionResult = string.Empty;

        public ACPage(Frame navigationFrame)
        {
            this.InitializeComponent();
            this.navigationFrame = navigationFrame;

            this.worker = new BackgroundWorker();
            this.worker.DoWork += this.DoCodingAsync;
            this.worker.RunWorkerCompleted += this.DoCodingCompleted;

            this.CodingStatus.Visibility = Visibility.Collapsed;
            if (InternetAvailability.GetIsInternetAvailable())
            {
                this.InfoFrame.Navigate(new Uri("https://en.wikipedia.org/wiki/Arithmetic_coding", UriKind.Absolute));
                this.NoConnection.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.NoConnection.Visibility = Visibility.Visible;
            }

            this.ToggleComponents(true, false);
        }

        private void AnalyzeCharacters_Click(object sender, RoutedEventArgs e)
        {
            var text = this.PlainText.Text;
            var characters = text.OrderBy(x => x).Distinct().ToList();

            List<Pair<string, string>> current = new List<Pair<string, string>>();
            foreach (var item in this.ProbalityTable.Children)
            {
                if (item is StackPanel)
                {
                    foreach (var t in ((StackPanel)item).Children)
                    {
                        if (t is TextBox txtbox)
                        {
                            current.Add(new Pair<string, string>(txtbox.Name, txtbox.Text));
                        }
                    }
                }
            }

            this.ProbalityTable.Children.Clear();

            foreach (var item in characters)
            {
                var name = "chr_" + (int)item;
                var textBox = new TextBox() { Name = name, Text = "0.0", Margin = new Thickness(5) };
                textBox.TextChanged += this.Probability_Value_Text_Changed;

                var y = current.Where(x => x.Key == name).SingleOrDefault();
                if (y != null)
                {
                    textBox.Text = y.Value;
                }

                this.CheckValue(textBox);

                var textLabel = new TextBlock { Text = string.Format("{0} ( {1} ) \t", item, (int)item), Margin = new Thickness(5) };
                var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                stackPanel.Children.Add(textLabel);
                stackPanel.Children.Add(textBox);

                this.ProbalityTable.Children.Add(stackPanel);
            }
        }

        private void Probability_Value_Text_Changed(object sender, TextChangedEventArgs e)
        {
            this.CheckValue((TextBox)sender);
            this.ToggleComponents(true, false);
        }

        private void PerformCoding_Click(object sender, RoutedEventArgs e)
        {
            this.PerformCoding.IsEnabled = false;
            this.AnalyzeCharacters.IsEnabled = false;
            try
            {
                this.model = new ArithmeticCodingDataModel
                {
                    DataToCode = this.PlainText.Text
                };

                foreach (var item in this.ProbalityTable.Children)
                {
                    if (item is StackPanel)
                    {
                        foreach (var t in ((StackPanel)item).Children)
                        {
                            if (t is TextBox txtbox)
                            {
                                char x = (char)int.Parse(txtbox.Name.Replace("chr_", string.Empty));
                                this.CheckValue(txtbox);
                                if (Regex.IsMatch(txtbox.Text, this.regexExpression))
                                {
                                    var txt = txtbox.Text.Trim().Replace(" ", string.Empty);
                                    BigDouble number = BigDouble.ZERO;
                                    if (txt.Contains('.'))
                                    {
                                        number = new BigDouble(txt);
                                    }
                                    else if (txt.Contains('/'))
                                    {
                                        var spltxt = txt.Split('/');
                                        var a = BigInteger.Parse(spltxt[0]);
                                        var b = BigInteger.Parse(spltxt[1]);
                                        number = new BigDouble(a, b);
                                    }

                                    this.model.Probabilities.Add(new Pair<char, BigDouble>(x, number));
                                }
                            }
                        }
                    }
                }

                this.CodingStatus.Visibility = Visibility.Visible;
                this.worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Some values are invalid fix them. Additional: " + ex.Message, "ERROR", MessageBoxButton.OK);
                this.PerformCoding.IsEnabled = true;
                this.AnalyzeCharacters.IsEnabled = true;
            }
        }

        private void DoCodingAsync(object sender, DoWorkEventArgs e)
        {
            this.result = ArithmeticCodingService.PerformArithmeticCoding(this.model);
            this.lowerFractionResult = this.result.LowerBound.ToString();
            this.upperFractionResult = this.result.UpperBound.ToString();
        }

        private void DoCodingCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Something went wrong: " + e.Error.Message, "ERROR", MessageBoxButton.OK);
            }
            else
            {
                this.varA.Text = this.result.LowerBound.Numerator.ToString();
                this.varB.Text = this.result.LowerBound.Denumerator.ToString();
                this.varC.Text = this.result.UpperBound.Numerator.ToString();
                this.varD.Text = this.result.UpperBound.Denumerator.ToString();
                this.decA.Text = this.lowerFractionResult;
                this.decB.Text = this.upperFractionResult;

                this.ToggleComponents(false, true);

                var message = "The coding is done.You can see the coding range result as fractions in 'Encoded range (A -> B) as a fraction' or" +
                    "as high precission decimal numbers in 'Encoded range (A -> B) as a decimal' tab";

                MessageBoxResult dialoresult = MessageBox.Show(message, "Coding Successful.", button: MessageBoxButton.OK);
            }

            this.CodingStatus.Visibility = Visibility.Collapsed;
            this.PerformCoding.IsEnabled = true;
            this.AnalyzeCharacters.IsEnabled = true;
        }

        private void PlainText_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ToggleComponents(true, false);
        }

        private void CheckValue(TextBox t)
        {
            var txt = t.Text;
            txt = txt.Replace(" ", string.Empty).Trim();

            if (!Regex.IsMatch(txt, this.regexExpression))
            {
                t.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                t.Foreground = new SolidColorBrush(Colors.Black);
            }

            t.Text = txt;
        }

        private void ToggleComponents(bool isHintVisible, bool isDataVisible)
        {
            this.EncodedTextFractionError.Visibility = isHintVisible ? Visibility.Visible : Visibility.Collapsed;
            this.EncodedTextFraction.Visibility = isDataVisible ? Visibility.Visible : Visibility.Collapsed;

            this.EncodedTextDecimalError.Visibility = isHintVisible ? Visibility.Visible : Visibility.Collapsed;
            this.EncodedTextDecimal.Visibility = isDataVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
