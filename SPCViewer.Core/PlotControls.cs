using OxyPlot;

namespace SPCViewer.Core
{
    public static class PlotControls
    {
        /// <summary>
        /// Scheme:
        /// MousewheelDown      Panning
        /// Scroll              Zoom
        /// Shift+Scroll        Zoom fine
        /// LeftMouse + CTRL    Rectangle Zoom
        /// CTRL + Z            Reset
        /// CTRL + C            Copy Image
        /// </summary>
        public static PlotController DefaultController
        {
            get
            {
                var ctrl = new PlotController();
                ctrl.UnbindAll();

                //Tracker
                //ctrl.BindMouseEnter(PlotCommands.HoverPointsOnlyTrack);

                //Copy Image
                ctrl.BindKeyDown(OxyKey.C, OxyModifierKeys.Control, PlotCommands.CopyCode);

                //Reset everything
                ctrl.BindKeyDown(OxyKey.Z, OxyModifierKeys.Control, PlotCommands.Reset);

                //Zoom and Pan
                ctrl.BindMouseDown(OxyMouseButton.Middle, PlotCommands.PanAt);
                //var wheel = new DelegatePlotCommand<OxyMouseWheelEventArgs>((view, controller, args) => HandleZoomByWheel(view, args));
                //ctrl.BindMouseWheel(wheel);
                ctrl.BindMouseWheel(OxyModifierKeys.Shift, PlotCommands.ZoomWheelFine);

                //Rectangle Zoom, Zoom X Only unless CTRL is pressed
                //ctrl.BindMouseDown(OxyMouseButton.Left, ZoomX);
                ctrl.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Control, PlotCommands.ZoomRectangle);

                return ctrl;
            }
        }

        /// <summary>
        /// Zooms the view by the mouse wheel delta in the specified <see cref="OxyKeyEventArgs" />.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="args">The <see cref="OxyMouseWheelEventArgs" /> instance containing the event data.</param>
        /// <param name="factor">The zoom speed factor. Default value is 1.</param>
        //private static void HandleZoomByWheel(IPlotView view, OxyMouseWheelEventArgs args, double factor = 1)
        //{
        //    var m = new ZoomYStepManipulator(view) { Step = args.Delta * 0.001 * factor, FineControl = args.IsControlDown };
        //    m.Started(args);
        //}
    }
}
