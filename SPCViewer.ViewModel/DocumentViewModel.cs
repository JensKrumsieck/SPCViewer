using ChemSharp.Extensions;
using ChemSharp.Spectroscopy.DataProviders;
using OxyPlot;
using SPCViewer.Core;
using SPCViewer.Core.Extension;
using SPCViewer.Core.Plots;
using System;
using System.Collections.Specialized;
using System.Linq;
using TinyMVVM;
using OxyDataPoint = OxyPlot.DataPoint;


namespace SPCViewer.ViewModel
{
    public sealed class DocumentViewModel : ListingViewModel<SpectrumViewModel>, IListItemViewModel<MainViewModel, DocumentViewModel>
    {
        /// <summary>
        /// The active MainViewModel
        /// </summary>
        public MainViewModel Parent { get; }

        /// <summary>
        /// Indicates whether this is the selected item
        /// </summary>
        public bool IsSelected => Parent?.SelectedItem == this;

        /// <summary>
        /// The used PlotModel
        /// </summary>
        public DefaultPlotModel Model { get; }

        /// <summary>
        /// Gets the PlotController
        /// </summary>
        public PlotController Controller { get; }

        private UIAction _mouseAction;
        /// <summary>
        /// Currently Active UI Action
        /// </summary>
        public UIAction MouseAction
        {
            get => _mouseAction;
            set
            {
                Set(ref _mouseAction, value);
                MouseActionChanged();
            }
        }

        public DocumentViewModel(MainViewModel parent)
        {
            Parent = parent;
            Parent.SelectedIndexChanged += (s, e) => OnPropertyChanged(nameof(IsSelected));
            Title = $"Untitled_{DateTime.Now:yyyy-MM-dd}";
            Model = new DefaultPlotModel();
            Controller = PlotControls.DefaultController;
            MouseAction = UIAction.Zoom;
            Items.CollectionChanged += ItemsOnCollectionChanged;
        }

        /// <summary>
        /// Binds to event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;
            foreach (SpectrumViewModel svm in e.NewItems) Subscribe(svm.Annotations, Model.Annotations, () => Model.InvalidatePlot(true));
        }

        /// <summary>
        /// Fires when MouseAction changes
        /// </summary>
        private void MouseActionChanged()
        {
            var action = MouseAction switch
            {
                UIAction.Integrate => UIActions.PrepareRectangleAction(AddIntegral),
                UIAction.PeakPicking => UIActions.PrepareRectangleAction(AddPeak),
                UIAction.Normalize => UIActions.PrepareRectangleAction(Normalize),
                UIAction.PickValue => UIActions.PreparePickAction(PickValue),
                UIAction.Tracker => UIActions.PreparePickAction(null),
                _ => UIActions.PrepareRectangleAction(null)
            };
            Controller.BindMouseDown(OxyMouseButton.Left, action);
        }

        /// <summary>
        /// Adds an Integral to List
        /// </summary>
        /// <param name="rect"></param>
        private void AddIntegral((OxyDataPoint, OxyDataPoint) rect)
        {
            var points = SelectedItem.Spectrum.XYData.PointsFromRect(rect);
            if (!points.Any()) return;
            if (SelectedItem.Integrals.Count == 0) SelectedItem.IntegralFactor = points.Integrate().Last().Y;
            SelectedItem.Integrals.Add(new Integral(points)
            {
                Factor = SelectedItem.IntegralFactor
            });
        }

        /// <summary>
        /// Adds Peaks to List
        /// </summary>
        /// <param name="rect"></param>
        private void AddPeak((OxyDataPoint, OxyDataPoint) rect)
        {
            var points = SelectedItem.Spectrum.XYData.PointsFromRect(rect);
            if (!points.Any()) return;
            var epr = SelectedItem.Spectrum.DataProvider is BrukerEPRProvider;
            var peaksIndices = points.Select(s => s.Y).ToList().FindPeakPositions(null, epr);
            foreach (var index in peaksIndices)
                if (!SelectedItem.Peaks.Any(s => Math.Abs(s.X - points[index].X) < 1e-9))
                    SelectedItem.Peaks.Add(new Peak(points[index]) { Factor = Model.NormalizationFactor });
        }

        /// <summary>
        /// Adds Peaks to List
        /// </summary>
        /// <param name="rect"></param>
        private void Normalize((OxyDataPoint, OxyDataPoint) rect)
        {
            var points = SelectedItem.Spectrum.XYData.PointsFromRect(rect);
            if (!points.Any()) return;
            var max = points.Max(s => s.Y);
            //send normalization factor to model
            Model.NormalizationFactor = max;
            //send factor to peaks
            foreach (var peak in SelectedItem.Peaks) peak.Factor = max;
            Model.YAxisRefresh();
            Model.InvalidatePlot(true);
        }

        /// <summary>
        /// Pick value into peak list
        /// not sure if it stays peak list
        /// </summary>
        /// <param name="point"></param>
        private void PickValue(ScreenPoint point)
        {
            var odp = SelectedItem.ExperimentalSeries.GetNearestPoint(point, false);
            var realDataPoint = SelectedItem.Spectrum.XYData.FromOxyDataPoint(odp.DataPoint);
            if (!SelectedItem.Peaks.Any(s => Math.Abs(s.X - point.X) < 1e-9))
                SelectedItem.Peaks.Add(new Peak(realDataPoint) { Factor = Model.NormalizationFactor });
        }

        /// <summary>
        /// DeleteCommand for tabs
        /// </summary>
        /// <param name="tab"></param>
        [DeleteCommand]
        public void DeleteTab(SpectrumViewModel tab)
        {
            Model.Series.Remove(tab.ExperimentalSeries);
            Model.Series.Remove(tab.DerivSeries);
            Model.Series.Remove(tab.IntegralSeries);
            tab.Annotations.Clear();
            if (SelectedItem == tab)
            {
                if (SelectedIndex == 0 && Items.Count > 1) SelectedIndex++;
                else SelectedIndex--;
            }
            Items.Remove(tab);
            Model.InvalidatePlot(true);
        }
    }
}
