using System.Collections.Generic;
using System.Collections.ObjectModel;
using TinyMVVM;

namespace SPCViewer.ViewModel
{
    public class MainViewModel : BindableBase
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
            set => Set(ref _selectedIndex, value, () => SelectedAction = (int)SelectedItem.MouseAction);
        }

        private int _selectedAction;
        /// <summary>
        /// Gets or Sets the selected action Index
        /// </summary>
        public int SelectedAction
        {
            get => _selectedAction;
            set => Set(ref _selectedAction, value, ActionChanged);
        }

        /// <summary>
        /// Gets the SelectedItem
        /// </summary>
        public SpectrumViewModel SelectedItem => TabItems[SelectedIndex];

        /// <summary>
        /// Gets the SelectedUIAction
        /// </summary>
        public UIAction SelectedUIAction => (UIAction)SelectedAction;

        public MainViewModel() { }

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

        private void ActionChanged()
        {
            if (SelectedAction == -1 || SelectedAction > (int)UIAction.PickValue)
                SelectedAction = 0;

            SelectedItem.MouseAction = SelectedUIAction;
        }
    }
}
