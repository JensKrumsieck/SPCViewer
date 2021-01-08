using ChemSharp.Spectroscopy.DataProviders;
using SPCViewer.Core;
using TinyMVVM;

namespace SPCViewer.ViewModel
{
    public class MainViewModel : ListingViewModel<SpectrumViewModel>
    {

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
        /// Gets the SelectedUIAction
        /// </summary>
        public UIAction SelectedUIAction => (UIAction)SelectedAction;

        public MainViewModel() => SelectedIndexChanged += (s, e) => SelectedAction = (int)SelectedItem.MouseAction;

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
                    for (var i = 0; i < multiCSV.MultiXYData.Count; i++)
                    {
                        page = new SpectrumViewModel(new GenericCSVProvider(file, ',', i));
                        Items.Add(page);
                        SelectedIndex = Items.IndexOf(page);
                    }
                }
                else
                {
                    page = new SpectrumViewModel(file);
                    Items.Add(page);
                    SelectedIndex = Items.IndexOf(page);
                }
            }
        }

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

        /// <summary>
        /// DeleteCommand for tabs
        /// </summary>
        /// <param name="tab"></param>
        [DeleteCommand]
        public void DeleteTab(SpectrumViewModel tab)
        {
            if (SelectedItem == tab)
            {
                if (SelectedIndex == 0 && Items.Count > 1) SelectedIndex++;
                else SelectedIndex--;
            }
            Items.Remove(tab);
        }
    }
}
