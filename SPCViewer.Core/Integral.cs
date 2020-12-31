using System;
using System.Collections;
using System.Collections.Generic;
using ChemSharp;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ChemSharp.Extensions;

namespace SPCViewer.Core
{
    public class Integral : INotifyPropertyChanged
    {
        private DataPoint[] _dataPoints;

        /// <summary>
        /// Integral starting DataPoint
        /// </summary>
        public DataPoint[] DataPoints
        {
            get => _dataPoints;
            set
            {
                _dataPoints = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Value of Integral
        /// </summary>
        public double Value { get; set; }

        public Integral(IEnumerable<DataPoint> selection)
        {
            var dataPoints = selection as DataPoint[] ?? selection.ToArray();
            if(!dataPoints.Any()) throw new ArgumentNullException(nameof(selection));
            PropertyChanged += OnPropertyChanged;
            DataPoints = dataPoints.ToArray();
        }

        public override string ToString() =>
            $"[{DataPoints.Min(s => s.X):N2};{DataPoints.Max(s => s.X):N2}] : {Value:N2}";

        /// <summary>
        /// Recalculates Integral
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(DataPoints)) return;
            Value = DataPoints.Integrate().Last().Y;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
