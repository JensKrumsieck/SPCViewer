using TinyMVVM;

namespace SPCViewer.ViewModel
{
    public class DocumentViewModel : ListingViewModel<SpectrumViewModel>
    {

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
