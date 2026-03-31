using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using tagIndex;

namespace SongTaggerGUI
{
    /// <summary>
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        private bool albumIsSearching = false;
        private bool artistIsSearching = false;
        private bool titleIsSearching = false;
        public SearchWindow()
        {
            InitializeComponent();
        }

        private void albumCheck_Checked(object sender, RoutedEventArgs e)
        {
            albumBox.IsEnabled = true;
            albumIsSearching = true;
        }
        private void albumCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            albumBox.IsEnabled = false;
            albumIsSearching = false;
        }
        private void artistCheck_Checked(object sender, RoutedEventArgs e)
        {
            artistBox.IsEnabled = true;
            artistIsSearching = true;
        }
        private void artistCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            artistBox.IsEnabled = false;
            artistIsSearching = false;
        }
        private void titleCheck_Checked(object sender, RoutedEventArgs e)
        {
            titleBox.IsEnabled = true;
            titleIsSearching = true;
        }
        private void titleCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            titleBox.IsEnabled = false;
            titleIsSearching = false;
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            string? artistSearch = null;
            string? albumSearch = null;
            string? titleSearch = null;
            if (artistIsSearching)
            {
                artistSearch = artistBox.Text;
                if (artistSearch.IsWhiteSpace() || artistSearch == null)
                {
                    MessageBox.Show("No Text Provided");
                    return;
                }
            }
            if (albumIsSearching == true)
            {
                albumSearch = albumBox.Text;
                if (albumSearch.IsWhiteSpace() || albumSearch == null)
                {
                    MessageBox.Show("No Text Provided");
                    return;
                }
            }
            if (titleIsSearching)
            {
                titleSearch = titleBox.Text;
                if (titleSearch.IsWhiteSpace() || titleSearch == null)
                {
                    MessageBox.Show("No Text Provided");
                    return;
                }
            }
            tagIndex.songDatabaseManager.searchDispatch(artist: artistIsSearching, album: albumIsSearching, title: titleIsSearching, artistString: artistSearch, albumString: albumSearch, titleString: titleSearch);

            if (Owner is MainWindow mainWindow)
            {
                mainWindow.showSearchResults();
            }

            this.Close();
        }
    }
}
