using System;
using OxyPlot;
using OxyPlot.Axes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SPCViewer.Core.Plots
{
    /// <summary>
    /// Linear Axis to bind to
    /// </summary>
    public class LinearAxisEx : LinearAxis, INotifyPropertyChanged
    {
        /// <summary>
        /// Specifies another axis this is linked to
        /// </summary>
        public LinearAxisEx LinkedTo { get; set; }

        /// <summary>
        /// Handle Conversion of Axis Ticks from Linked Axis
        /// </summary>
        public Func<double, double> Converter { get; set; } = x => x;

        /// <summary>
        /// Handles Conversion of Axis Ticks to Linked Axis
        /// </summary>
        public Func<double, double> InverseConverter { get; set; } = x => x;

        #region BindableProperties
        public double BindableActualMinimum
        {
            get => ActualMinimum;
            set
            {
                ActualMinimum = value;
                OnPropertyChanged();
                Zoom(ActualMinimum, ActualMaximum);
                PlotModel.InvalidatePlot(true);
            }
        }

        public double BindableActualMaximum
        {
            get => ActualMaximum;
            set
            {
                ActualMaximum = value;
                OnPropertyChanged();
                Zoom(ActualMinimum, ActualMaximum);
                PlotModel.InvalidatePlot(true);
            }
        }

        public double BindableMajorStep
        {
            get => MajorStep;
            set
            {
                MajorStep = value;
                OnPropertyChanged();
                PlotModel.InvalidatePlot(true);
            }
        }

        public double BindableMinorStep
        {
            get => MinorStep;
            set
            {
                MinorStep = value;
                OnPropertyChanged();
                PlotModel.InvalidatePlot(true);
            }
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged();
                Toggle();
            }
        }

        private bool _isInverted;
        public bool IsInverted
        {
            get => _isInverted;
            set
            {
                _isInverted = value;
                OnPropertyChanged();
                Invert();
            }
        }
        #endregion

        /// <summary>
        /// Toggles Axis (But leaves scrolling intact)
        /// </summary>
        private void Toggle()
        {
            //enable
            if (IsVisible)
            {
                TickStyle = TickStyle.Outside;
                LabelFormatter = null;
                AxislineStyle = LineStyle.Solid;
                TitleColor = OxyColors.Black;
            }
            //disable
            else
            {
                TickStyle = TickStyle.None;
                LabelFormatter = d => null;
                AxislineStyle = LineStyle.None;
                TitleColor = OxyColors.Transparent;
            }
            PlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Inverts Axis
        /// </summary>
        public void Invert()
        {
            StartPosition = IsInverted ? 1 : 0;
            EndPosition = IsInverted ? 0 : 1;
            PlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Raise Update notification
        /// </summary>
        protected override void ActualMaximumAndMinimumChangedOverride()
        {
            base.ActualMaximumAndMinimumChangedOverride();
            OnPropertyChanged(nameof(BindableActualMaximum));
            OnPropertyChanged(nameof(BindableActualMinimum));

            if (LinkedTo == null) return;
            Zoom(Converter(LinkedTo.ActualMinimum), Converter(LinkedTo.ActualMaximum));
        }

        public override double Transform(double x) => LinkedTo?.Transform(Converter(x)) ?? base.Transform(x);
        public override double InverseTransform(double sx) => LinkedTo?.InverseTransform(InverseConverter(sx)) ?? base.InverseTransform(sx);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
