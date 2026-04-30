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
using SpotiFechLib;

namespace SongTaggerGUI
{
    /// <summary>
    /// Interaction logic for APICollect.xaml
    /// </summary>
    public partial class APICollect : Window
    {
        public APICollect()
        {
            InitializeComponent();
        }

        private async void buttonClicked(object sender, RoutedEventArgs e)
        {
            string key = keyBox.Text;

            SpotiFechLib.APIStore.writeFile(key);
            MessageBox.Show("Key Collected! Testing verifcation with spotify...");

            try
            {
                await SpotiFechLib.SpotiLib.Auth();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.GetType().Name} | {ex.Message}");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Do not create a new MainWindow here.
            // MainWindow is restored by the caller.
        }
    }
}
