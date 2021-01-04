using ChemSharp;
using ChemSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyMVVM;

namespace SPCViewer.Core
{
    public class Integral : BindableBase
    {
        private DataPoint[] _dataPoints;

        /// <summary>
        /// Integral starting DataPoint
        /// </summary>
        public DataPoint[] DataPoints
        {
            get => _dataPoints;
            set => Set(ref _dataPoints, value, SetValue);
        }


        private bool _editIndicator;
        /// <summary>
        /// Indicates to GUI that editing is in progress
        /// </summary>
        public bool EditIndicator
        {
            get => _editIndicator;
            set => Set(ref _editIndicator, value);
        }

        /// <summary>
        /// X Value of beginning
        /// </summary>
        public double From => DataPoints.Min(s => s.X);
        /// <summary>
        /// X Vlue of End
        /// </summary>
        public double To => DataPoints.Max(s => s.X);

        private double _value;
        /// <summary>
        /// Value of Integral
        /// </summary>
        public double Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        /// <summary>
        /// Integrals RawValue without Factor Multiplication
        /// </summary>
        public double RawValue => DataPoints.Integrate().Last().Y;

        private double _factor = 1;
        /// <summary>
        /// Factor of Integral
        /// </summary>
        public double Factor
        {
            get => _factor;
            set => Set(ref _factor, value, SetValue);
        }

        public Integral(IEnumerable<DataPoint> selection)
        {
            var dataPoints = selection as DataPoint[] ?? selection.ToArray();
            if (!dataPoints.Any()) throw new ArgumentNullException(nameof(selection));
            DataPoints = dataPoints.ToArray();
        }

        /// <summary>
        /// Recalculates Integral value
        /// </summary>
        private void SetValue() => Value = RawValue / Factor;

    }
}
