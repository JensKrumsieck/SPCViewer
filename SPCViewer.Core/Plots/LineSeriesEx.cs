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
        /// <summary>
        /// Makes Color bindable for UI
        /// </summary>
        public OxyColor BindableColor
        {
            get => Color;
            set
            {
                Color = value;
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
