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
        private record isSelectedBcp(int id, bool isSelected);
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
            // Backup current selection state
            var selectionBackup = dbMgmt.Functions.localDbDisp
                .ToDictionary(x => x.id, x => x.isSelected);

            dbMgmt.Functions.localDbDisp.Clear();

            foreach (var result in songDatabaseManager.SongDatabase)
            {
                var enter = new dbMgmt.SongInfoDisp
                {
                    Title = result.Title,
                    Artist = result.Artist,
                    path = result.path,
                    duration = result.duration,
                    id = result.id,
                    isSelected = selectionBackup.TryGetValue(result.id, out var selected) && selected
                };

                dbMgmt.Functions.localDbDisp.Add(enter);
            }

            songDataGrid.ItemsSource = dbMgmt.Functions.localDbDisp;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void openSpotiWindow(object sender, RoutedEventArgs e)
        {
            SpotiWindow spotiWindow = new SpotiWindow();
            spotiWindow.Show();
            this.Close();
        }
       
        public void SetCurrentSong(string input)
        {
            LoadingCurrentFileTxt.Text = $"Current File: {input}";
        }

        static public void updateIsSelected(int id, bool isSelect)
        {
            var item = dbMgmt.Functions.localDbDisp.FirstOrDefault(x => x.id == id);
            if (item != null)
                item.isSelected = isSelect;
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
            var resultIds = tagIndex.songDatabaseManager.SearchResults
                        .Select(s => s.id)
                        .ToHashSet();
            songDataGrid.ItemsSource = dbMgmt.Functions.localDbDisp
                                             .Where(d => resultIds.Contains(d.id))
                                             .ToList();
        }

        private void makeFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDiag = new SaveFileDialog();
            saveDiag.Filter = ".m3u8|.m3u8";

            bool? result = saveDiag.ShowDialog();

            if (result == true)
            {
                string path = saveDiag.FileName;
                m3u8Manager.Program.runFileMake(path);
            }
        }
    }


}