using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using tagIndex;
using dbMgmt;
using m3u8Manager;
using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace SongTaggerGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void dirButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFoldDiag = new OpenFolderDialog();
            string sendPath;

            openFoldDiag.InitialDirectory = Environment.SpecialFolder.UserProfile.ToString();
            
            bool? result = openFoldDiag.ShowDialog();

            if (result == true)
            {
               string path = openFoldDiag.FolderName;
                if (path is null)
                {
                    MessageBox.Show("How the actual fuck did you get here...");
                    System.Diagnostics.Process.Start("shutdown.exe /s /t 0");
                }
                else
                {
                    sendPath = path;
                    LoadingOverlay.Visibility = Visibility.Visible;
                    var progressReporter = new Progress<string>(SetCurrentSong);
                    await Task.Run(() => songDatabaseManager.indexFiles(sendPath, progressReporter));
                    LoadingOverlay.Visibility = Visibility.Collapsed;
                    getSongDatabase();
                }
            }
        }
        private void getSongDatabase()
        {
            dbMgmt.Program.localDbDisp.Clear();
            var LocalDatabase = songDatabaseManager.SongDatabase;
            
            foreach (var result in songDatabaseManager.SongDatabase)
            {
                var enter = new dbMgmt.Program.SongInfoDisp(
                   Title: result.Title,
                   Artist: result.Artist,
                   Album: result.Album,
                   path: result.path,
                   duration: result.duration,
                   id: result.id,
                   isSelected: false
                );

                dbMgmt.Program.localDbDisp.Add(enter);
            }
            songDataGrid.ItemsSource = dbMgmt.Program.localDbDisp;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void makeButton_Click(object sender, EventArgs e)
        {
            m3u8Manager.Program.runFileMake();
        }
        private void DataGrid_EditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit)
                return;

            if (e.Row.Item is not dbMgmt.Program.SongInfoDisp song)
                return;

            if (e.Column is DataGridCheckBoxColumn && e.EditingElement is CheckBox cb)
            {
                dbMgmt.Program.updateIsSelected(song.id, cb.IsChecked == true);
            }
        }
       
        public void SetCurrentSong(string input)
        {
            LoadingCurrentFileTxt.Text = $"Current File: {input}";
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchWindow searchWindow = new SearchWindow();
            searchWindow.Owner = this;
            searchWindow.Show();
        }

        private void clrButton_Click(object sender, RoutedEventArgs e)
        {
            getSongDatabase();
        }
        public void showSearchResults()
        {
            var localSearch = tagIndex.songDatabaseManager.SearchResults;
            songDataGrid.ItemsSource = localSearch;
        }
    }


}