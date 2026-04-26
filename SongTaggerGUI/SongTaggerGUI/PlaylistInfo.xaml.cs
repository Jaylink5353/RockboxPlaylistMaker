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
        public PlaylistInfo(List<SpotiLib.TrackInfo> db)
        {
            InitializeComponent();
            songDataGrid.ItemsSource = db;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
