using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SPCViewer.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// TabItems
        /// </summary>
        public ObservableCollection<SpectrumViewModel> TabItems { get; set; } = new ObservableCollection<SpectrumViewModel>(new List<SpectrumViewModel>());

        private int _selectedIndex;
        /// <summary>
        /// Gets or Sets the selectedIndex
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the SelectedItem
        /// </summary>
        public SpectrumViewModel SelectedItem => TabItems[SelectedIndex];


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Used to open files / create tabviewmodel
        /// </summary>
        /// <param name="files"></param>
        public void OpenFiles(string[] files)
        {
            if (files == null) return;
            foreach (var file in files)
            {
                var page = new SpectrumViewModel(file);
                TabItems.Add(page);
                SelectedIndex = TabItems.IndexOf(page);
            }
        }
    }
}
