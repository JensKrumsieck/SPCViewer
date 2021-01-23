using System.ComponentModel;
using System.Runtime.CompilerServices;
using OxyPlot.Series;

namespace SPCViewer.Core.Plots
{
    public class LineSeriesEx : LineSeries, INotifyPropertyChanged
    {
        /// <summary>
        /// Makes StrokeThickness bindable for UI
        /// </summary>
        public double BindableStrokeThickness
        {
            get => StrokeThickness;
            set
            {
                StrokeThickness = value;
                OnPropertyChanged();
                PlotModel.InvalidatePlot(true);
            }
        }

        public bool BindableIsVisible
        {
            get => IsVisible;
            set
            {
                IsVisible = value;
                OnPropertyChanged();
                PlotModel.InvalidatePlot(true);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
