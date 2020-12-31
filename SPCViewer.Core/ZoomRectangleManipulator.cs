using OxyPlot;

namespace SPCViewer.Core
{
    public class ZoomRectangleManipulator : OxyPlot.ZoomRectangleManipulator
    {
        public ZoomRectangleManipulator(IPlotView plotView) : base(plotView)
        {
        }

        public override void Completed(OxyMouseEventArgs e)
        {
            //Zoom is default
            if (true) base.Completed(e);
        }

    }
}
