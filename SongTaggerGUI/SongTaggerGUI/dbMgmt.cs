using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using tagIndex;

namespace dbMgmt
{
    public class Functions
    {
        //public record SongInfoDisp(string Title, string Artist, string Album, string path, TimeSpan duration, int id, bool isSelected);


        static public ObservableCollection<SongInfoDisp> localDbDisp = new ObservableCollection<SongInfoDisp>();

    }
    public class SongInfoDisp : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string path { get; set; }
        public TimeSpan duration { get; set; }
        public int id { get; set; }
        private bool _isSelected;
        public bool isSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(nameof(isSelected)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
