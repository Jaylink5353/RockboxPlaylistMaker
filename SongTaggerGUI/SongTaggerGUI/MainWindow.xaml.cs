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
            var LocalDatabase = songDatabaseManager.SongDatabase;
            songDataGrid.ItemsSource = LocalDatabase;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        public void SetCurrentSong(string input)
        {
            LoadingCurrentFileTxt.Text = $"Current File: {input}";
        }
    }

}