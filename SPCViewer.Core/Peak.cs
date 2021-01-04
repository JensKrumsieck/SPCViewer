using TinyMVVM;
using DataPoint = ChemSharp.DataPoint;
using OxyDataPoint = OxyPlot.DataPoint;

namespace SPCViewer.Core
{
    public class Peak : BindableBase
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
            set => Set(ref _value, value);
        }

        private double _factor = 1;
        /// <summary>
        /// The current normalization factor
        /// </summary>
        public double Factor
        {
            get => _factor;
            set => Set(ref _factor, value, () => Value = Y / Factor);
        }

        public Peak(double x, double y)
        {
            X = x;
            Y = y;
            Value = Y / Factor;
        }

        #region ctors with datapoints
        public Peak(DataPoint cdp) : this(cdp.X, cdp.Y) { }
        public Peak(OxyDataPoint odp) : this(odp.X, odp.Y) { }
        #endregion
    }
}
