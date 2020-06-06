using ItsyBitsy.Domain;
using System;
using System.Windows;

namespace ItsyBitsy.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly CrawlManager _crawlManager;

        public MainWindow()
        {
            _crawlManager = new CrawlManager();
            InitializeComponent();
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (selectedWebsite.SelectedItem is Website website)
            {
                btnStart.IsEnabled = false;
                pnlSettings.IsEnabled = false;
                pnlWebsites.IsEnabled = false;
                btnPause.IsEnabled = true;
                btnResume.IsEnabled = true;
                btnHardStop.IsEnabled = true;
                await _crawlManager.Start(website);
            }
        }

        private async void BtnHardStop_Click(object sender, RoutedEventArgs e)
        {
            await _crawlManager.HardStop();
            btnStart.IsEnabled = true;
            pnlSettings.IsEnabled = true;
            pnlWebsites.IsEnabled = true;
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            _crawlManager.Pause();
        }

        private void BtnResume_Click(object sender, RoutedEventArgs e)
        {
            _crawlManager.Resume();
        }

        private async void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            await _crawlManager.HardStop();
        }

        private async void AddWebsite_Click(object sender, RoutedEventArgs e)
        {
            var seed = websiteSeed.Text;
            try
            {
                var newWebsite = await Repository.CreateWebsite(seed);
                CrawlContext.Instance.WebsiteSeeds.Add(newWebsite);
                MessageBox.Show($"{seed} was added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
