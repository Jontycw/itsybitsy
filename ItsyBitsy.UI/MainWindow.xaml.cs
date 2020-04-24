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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool b;
        public bool FollowExtenalLinks
        {
            get { return b; }
            set
            {
                b = value;
                PropertyChanged(this, new PropertyChangedEventArgs("FollowExtenalLinks"));
            }
        }

        public bool DownloadExternalContent;
        public bool RespectRobots { get; set; }
        public bool FollowRedirects { get; set; }
        public bool UseCookies { get; set; }
        public bool IncludeImages
        {
            get { return Settings.Instance.IncludeImages; }
            set { Settings.Instance.IncludeImages = value; }
        }
        public bool IncludeCss
        {
            get { return Settings.Instance.IncludeCss; }
            set { Settings.Instance.IncludeCss = value; }
        }
        public bool IncludeJs { get; set; }
        public bool IncludeJson { get; set; }
        public bool IncludeOther { get; set; }

        public MainWindow()
        {
            //_ = Settings.Instance; //loads settings

            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
