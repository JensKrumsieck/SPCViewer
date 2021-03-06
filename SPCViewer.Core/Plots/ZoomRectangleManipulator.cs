﻿using OxyPlot;
using System;

namespace SPCViewer.Core.Plots
{
    public class ZoomRectangleManipulator : OxyPlot.ZoomRectangleManipulator
    {
        private readonly Action<(DataPoint, DataPoint)> _rectangleAction;

        public ZoomRectangleManipulator(IPlotView plotView) : base(plotView) { }

        public ZoomRectangleManipulator(IPlotView plotView, Action<(DataPoint, DataPoint)> action) : base(plotView) => _rectangleAction = action;


        public override void Completed(OxyMouseEventArgs e)
        {
            //Zoom is default
            if (_rectangleAction == null)
            {
                base.Completed(e);
                return;
            }

            PlotView.SetCursorType(CursorType.Default);
            PlotView.HideZoomRectangle();

            //get rectangle
            var x = Math.Min(this.StartPosition.X, e.Position.X);
            var w = Math.Abs(this.StartPosition.X - e.Position.X);
            var y = Math.Min(this.StartPosition.Y, e.Position.Y);
            var h = Math.Abs(this.StartPosition.Y - e.Position.Y);
            var rectangle = new OxyRect(x, y, w, h);
            var p0 = InverseTransform(rectangle.Left, rectangle.Top);
            var p1 = InverseTransform(rectangle.Right, rectangle.Bottom);
            var rect = (p0, p1);
            _rectangleAction(rect);

            PlotView.InvalidatePlot();
            e.Handled = true;
        }

    }
}
