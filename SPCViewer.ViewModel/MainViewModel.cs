using SPCViewer.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ChemSharp.Spectroscopy.DataProviders;
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
            set => Set(ref _selectedIndex, value,
                () => SelectedAction = SelectedItem != null ? (int) SelectedItem.MouseAction : 0);
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
        public SpectrumViewModel SelectedItem => SelectedIndex == -1 ? null : TabItems[SelectedIndex];

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
                SpectrumViewModel page;
                if (ExtensionHandler.GetExtension(file) == "csv")
                {
                    var multiCSV = new MultiCSVProvider(file);
                    for(var i = 0; i < multiCSV.MultiXYData.Count; i++)
                    {
                        page = new SpectrumViewModel(new GenericCSVProvider(file, ',',  i));
                        TabItems.Add(page);
                        SelectedIndex = TabItems.IndexOf(page);
                    }
                }
                else
                {
                    page = new SpectrumViewModel(file); 
                    TabItems.Add(page);
                    SelectedIndex = TabItems.IndexOf(page);
                }
            }
        }

        private ICommand _delete;
        /// <summary>
        /// DeleteCommand
        /// </summary>
        public ICommand Delete => _delete ??= new RelayCommand<SpectrumViewModel>(DeleteTab);

        /// <summary>
        /// Fires on Action Changed
        /// </summary>
        private void ActionChanged()
        {
            if (SelectedAction == -1 || SelectedAction > (int)UIAction.PickValue)
                SelectedAction = 0;
            if (SelectedIndex == -1) return;
            SelectedItem.MouseAction = SelectedUIAction;
        }

        private void DeleteTab(SpectrumViewModel tab)
        {
            if (SelectedItem == tab)
            {
                if (SelectedIndex == 0 && TabItems.Count > 1) SelectedIndex++;
                else SelectedIndex--;
            }
            TabItems.Remove(tab);
        }
    }
}
