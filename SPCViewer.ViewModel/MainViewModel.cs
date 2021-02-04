using ChemSharp.Spectroscopy;
using ChemSharp.Spectroscopy.DataProviders;
using SPCViewer.Core;
using TinyMVVM;

namespace SPCViewer.ViewModel
{
    public class MainViewModel : ListingViewModel<DocumentViewModel>
    {
        /// <summary>
        /// Creates a new DocumentViewModel
        /// </summary>
        public void CreateDocument()
        {
            var doc = new DocumentViewModel(this);
            Items.Add(doc);
            SelectedIndex = Items.IndexOf(doc);
        }

        /// <summary>
        /// Used to open files / create tabviewmodel
        /// </summary>
        /// <param name="files"></param>
        public void OpenFiles(string[] files)
        {
            if (SelectedItem == null) CreateDocument();
            var doc = SelectedItem;
            if (files == null) return;
            foreach (var file in files)
            {
                if (ExtensionHandler.GetExtension(file) == "csv")
                {
                    var multiCSV = new MultiCSVProvider(file);
                    for (var i = 0; i < multiCSV.MultiXYData.Count; i++)
                    {
                        var page = new SpectrumViewModel(doc, new Spectrum(new GenericCSVProvider(file, ',', i)));
                        doc.Items.Add(page);
                        doc.SelectedIndex = doc.Items.IndexOf(page);
                    }
                }
                else
                {
                    var page = new SpectrumViewModel(doc, file);
                    doc.Items.Add(page);
                    doc.SelectedIndex = doc.Items.IndexOf(page);
                }
            }
        }

        /// <summary>
        /// Saves the current document
        /// </summary>
        /// <param name="filename"></param>
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
