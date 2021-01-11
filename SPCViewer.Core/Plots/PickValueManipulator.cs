using OxyPlot;
using System;

namespace SPCViewer.Core.Plots
{
    public class PickValueManipulator : TrackerManipulator
    {
        /// <summary>
        /// The action to execute on completed
        /// </summary>
        private readonly Action<ScreenPoint> _action;

        public PickValueManipulator(IPlotView plotView) : base(plotView)
        {
            PointsOnly = true;
        }

        public PickValueManipulator(IPlotView plotView, Action<ScreenPoint> action) : base(plotView)
        {
            PointsOnly = true;
            _action = action;
        }

        public override void Completed(OxyMouseEventArgs e)
        {
            //just use Tracker by default
            _action?.Invoke(e.Position);
            base.Completed(e);
        }
    }
}
