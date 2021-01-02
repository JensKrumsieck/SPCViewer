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

        private int _selectedAction;
        /// <summary>
        /// Gets or Sets the selected action Index
        /// </summary>
        public int SelectedAction
        {
            get => _selectedAction;
            set
            {
                _selectedAction = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the SelectedItem
        /// </summary>
        public SpectrumViewModel SelectedItem => TabItems[SelectedIndex];

        /// <summary>
        /// Gets the SelectedUIAction
        /// </summary>
        public UIAction SelectedUIAction => (UIAction)SelectedAction;

        public MainViewModel()
        {
            //register event
            PropertyChanged += OnPropertyChanged;
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

        #region Event Stuff

        /// <summary>
        /// PropertyChanged
        /// Sets UI Action to children
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedAction):
                    {
                        //set selected action to active tab
                        if (SelectedAction == -1 || SelectedAction > (int)UIAction.Integrate) SelectedAction = 0;
                        SelectedItem.MouseAction = SelectedUIAction;
                        break;
                    }
                case nameof(SelectedIndex):
                    //set selected action from tab
                    SelectedAction = (int)SelectedItem.MouseAction;
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
