using OxyPlot;

namespace SPCViewer.Core.Plots
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

                //Copy Image
                ctrl.BindKeyDown(OxyKey.C, OxyModifierKeys.Control, PlotCommands.CopyCode);

                //Reset everything
                ctrl.BindKeyDown(OxyKey.Z, OxyModifierKeys.Control, PlotCommands.Reset);

                //Zoom and Pan
                ctrl.BindMouseDown(OxyMouseButton.Middle, PlotCommands.PanAt);
                ctrl.BindMouseWheel(PlotCommands.ZoomWheel);
                ctrl.BindMouseWheel(OxyModifierKeys.Shift, PlotCommands.ZoomWheelFine);

                //Rectangle Zoom, Zoom X Only unless CTRL is pressed
                ctrl.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Control, PlotCommands.ZoomRectangle);

                return ctrl;
            }
        }
        
    }
}
