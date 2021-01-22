using ChemSharp.Spectroscopy.DataProviders;
using SPCViewer.Core;
using TinyMVVM;

namespace SPCViewer.ViewModel
{
    public class MainViewModel : ListingViewModel<DocumentViewModel>
    {
        /// <summary>
        /// Used to open files / create tabviewmodel
        /// </summary>
        /// <param name="files"></param>
        public void OpenFiles(string[] files)
        {
            var doc = new DocumentViewModel();
            Items.Add(doc);
            SelectedIndex = Items.IndexOf(doc);
            if (files == null) return;
            foreach (var file in files)
            {
                if (ExtensionHandler.GetExtension(file) == "csv")
                {
                    var multiCSV = new MultiCSVProvider(file);
                    for (var i = 0; i < multiCSV.MultiXYData.Count; i++)
                    {
                        var page = new SpectrumViewModel(new GenericCSVProvider(file, ',', i));
                        SelectedItem.Items.Add(page);
                        SelectedItem.SelectedIndex = SelectedItem.Items.IndexOf(page);
                    }
                }
                else
                {
                    var page = new SpectrumViewModel(file);
                    SelectedItem.Items.Add(page);
                    SelectedItem.SelectedIndex = SelectedItem.Items.IndexOf(page);
                }
            }
        }

        public void SaveFile(string filename) => SaveHandler.Handle(SelectedItem.SelectedItem, filename);

        /// <summary>
        /// DeleteCommand for tabs
        /// </summary>
        /// <param name="tab"></param>
        [DeleteCommand]
        public void DeleteTab(DocumentViewModel tab)
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
