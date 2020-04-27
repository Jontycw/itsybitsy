using ItsyBitsy.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ItsyBitsy.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly CrawlManager _crawlManager;
        //readonly CrawlProgress CrawlProgress;
        //readonly Settings Settings;
        //readonly CrawlContext CrawlContext;

        public MainWindow()
        {
            //CrawlContext = CrawlContext.Instance;
            //CrawlProgress = CrawlProgress.Instance;
            //Settings = Settings.Instance;
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
                await _crawlManager.Start(website);
            }
        }

        private async void BtnHardStop_Click(object sender, RoutedEventArgs e)
        {
            await _crawlManager.HardStop();
        }

        private async void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            await _crawlManager.Pause();
        }

        private async void BtnResume_Click(object sender, RoutedEventArgs e)
        {
            await _crawlManager.Resume();
        }

        private void BtnDrainStop_Click(object sender, RoutedEventArgs e)
        {
            _crawlManager.DrawinStop();
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
