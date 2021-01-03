using System.ComponentModel;
using System.Runtime.CompilerServices;
using DataPoint = ChemSharp.DataPoint;
using OxyDataPoint = OxyPlot.DataPoint;

namespace SPCViewer.Core
{
    public class Peak : INotifyPropertyChanged
    {

        /// <summary>
        /// Peaks X Value
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Peaks Y Value
        /// </summary>
        public double Y { get; }

        private double _value;
        /// <summary>
        /// The shown value!
        /// </summary>
        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        private double _factor = 1;
        /// <summary>
        /// The current normalisation factor
        /// </summary>
        public double Factor
        {
            get => _factor;
            set
            {
                _factor = value;
                OnPropertyChanged();
            }
        }

        public Peak(double x, double y)
        {
            X = x;
            Y = y;
            Value = Y / Factor;
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Factor)) return;
            Value = Y / Factor;
        }

        #region ctors with datapoints
        public Peak(DataPoint cdp) : this(cdp.X, cdp.Y) { }
        public Peak(OxyDataPoint odp) : this(odp.X, odp.Y) { }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
