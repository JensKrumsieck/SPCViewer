using OxyPlot;
using OxyPlot.Series;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SPCViewer.Core.Plots
{
    public class LineSeriesEx : LineSeries, INotifyPropertyChanged
    {
        /// <summary>
        /// Makes IsVisible bindable for UI
        /// </summary>
        public bool BindableIsVisible
        {
            get => IsVisible;
            set
            {
                IsVisible = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PlotModel.InvalidatePlot(true);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
