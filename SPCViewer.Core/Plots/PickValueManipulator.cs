using System;
using OxyPlot;

namespace SPCViewer.Core.Plots
{
    public class PickValueManipulator : TrackerManipulator
    {
        /// <summary>
        /// The action to execute on completed
        /// </summary>
        private Action<DataPoint> _action;

        public PickValueManipulator(IPlotView plotView) : base(plotView) { }

        public PickValueManipulator(IPlotView plotView, Action<DataPoint> action) : base(plotView) =>_action = action;

        public override void Completed(OxyMouseEventArgs e)
        {
            //just use Tracker by default
            if (_action != null) _action(InverseTransform(e.Position.X, e.Position.Y));
             base.Completed(e);
        }
    }
}
