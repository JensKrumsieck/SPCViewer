using OxyPlot;
using SPCViewer.Core.Plots;
using System;
using ZoomRectangleManipulator = SPCViewer.Core.Plots.ZoomRectangleManipulator;

namespace SPCViewer.ViewModel
{
    public enum UIAction
    {
        Zoom,
        Tracker,
        Normalize,
        PeakPicking,
        Integrate,
        PickValue
    }

    public static class UIActions
    {
        /// <summary>
        /// Prepares a ZoomRectangle DelegatePlotCommand for given Action
        /// </summary>
        /// <param name="rectAction"></param>
        /// <returns></returns>
        public static DelegatePlotCommand<OxyMouseDownEventArgs> PrepareRectangleAction(Action<(DataPoint, DataPoint)> rectAction) =>
            new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) =>
                controller.AddMouseManipulator(view, new ZoomRectangleManipulator(view, rectAction),
                    args));

        /// <summary>
        /// Prepares a PickValue DelegatePlotCommand for given Action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static DelegatePlotCommand<OxyMouseDownEventArgs> PreparePickAction(Action<ScreenPoint> action) =>
            new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) =>
                controller.AddMouseManipulator(view, new PickValueManipulator(view, action),
                    args));
    }
}
