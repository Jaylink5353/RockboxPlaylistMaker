using Microsoft.Win32;
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
using tagIndex;
using dbMgmt;
using m3u8Manager;
using SpotiFechLib;

namespace SongTaggerGUI
{
    /// <summary>
    /// Interaction logic for SpotiWindow.xaml
    /// </summary>
    public partial class SpotiWindow : Window
    {
        List<SpotiFechLib.SpotiLib.TrackInfo> localSpotiDb = new List<SpotiFechLib.SpotiLib.TrackInfo>();
        bool readLocalLib = false;
        public SpotiWindow()
        {
            InitializeComponent();
            if (SpotiFechLib.APIStore.checkFile() == false)
            {
                //Popup API Input Window
                //Make sure the API key is good before continuing
            }
        }


        private async void fetchButtonClicked(object sender, RoutedEventArgs e)
        {
            LoadingOverlay.Visibility = Visibility.Visible;
            LoadingCurrentFileTxt.Text = "Parsing Through Spotify Results...";
            string url = urlBox.Text;
            var tracks = await SpotiFechLib.SpotiLib.GetPlaylist(url);
            foreach (var result in tracks)
            {
                localSpotiDb.Add(result);
            }
            PlaylistInfo playlistInfo = new PlaylistInfo(localSpotiDb);
            playlistInfo.Owner = this;
            playlistInfo.Show();
            await selectPlaylistSongs();
            LoadingOverlay.Visibility = Visibility.Collapsed;
        }
            
        public async Task selectPlaylistSongs()
        {
            
            foreach (var spotiSong in localSpotiDb)
            {
                bool matched = false;
                foreach (var localSong in songDatabaseManager.SongDatabase)
                {
                    
                    if (spotiSong.title.Contains(localSong.Title) && spotiSong.artists.Contains(localSong.Artist) && (spotiSong.album.Contains(localSong.Album)))
                    {
                        var uiSong = dbMgmt.Functions.localDbDisp.FirstOrDefault(s => s.id == localSong.id);
                        if (uiSong is not null)
                        {
                            uiSong.isSelected = true;
                        }
                        matched = true;
                        break;
                    }
                }
                if (!matched)
                {
                    MessageBox.Show($"Your song library doesn't Contain {spotiSong.title} by {spotiSong.artists}");
                }
            }
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
        public void SetCurrentSong(string input)
        {
            LoadingCurrentFileTxt.Text = $"Current File: {input}";
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

    }
}
