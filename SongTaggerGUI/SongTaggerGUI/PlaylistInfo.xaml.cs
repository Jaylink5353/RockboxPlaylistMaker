using SpotiFechLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SongTaggerGUI
{
    /// <summary>
    /// Interaction logic for PlaylistInfo.xaml
    /// </summary>
    public partial class PlaylistInfo : Window
    {
        bool Continue = false;
        public PlaylistInfo(List<SpotiLib.TrackInfo> db, int mode)
        {
            InitializeComponent();
            songDataGrid.ItemsSource = db;
            if (mode == 1)
            {
                TitleText.Text = "Your library is missing these songs:";
            }
        }

        private async void dispPlaylist(List<SpotiLib.TrackInfo> db)
        {
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
