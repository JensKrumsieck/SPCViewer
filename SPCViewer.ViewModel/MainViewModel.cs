using ChemSharp.Spectroscopy.DataProviders;
using SPCViewer.Core;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text.Json;
using TinyMVVM;

namespace SPCViewer.ViewModel
{
    public class MainViewModel : ListingViewModel<SpectrumViewModel>
    {
        /// <summary>
        /// Used to open files / create tabviewmodel
        /// </summary>
        /// <param name="files"></param>
        public void OpenFiles(string[] files)
        {
            if (files == null) return;
            foreach (var file in files)
            {
                if (ExtensionHandler.GetExtension(file) == "csv")
                {
                    var multiCSV = new MultiCSVProvider(file);
                    for (var i = 0; i < multiCSV.MultiXYData.Count; i++)
                    {
                        var page = new SpectrumViewModel(new GenericCSVProvider(file, ',', i));
                        Items.Add(page);
                        SelectedIndex = Items.IndexOf(page);
                    }
                }
                else
                {
                    var page = new SpectrumViewModel(file);
                    Items.Add(page);
                    SelectedIndex = Items.IndexOf(page);
                }
            }
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
