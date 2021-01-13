namespace SPCViewer.WPF.Resources
{
    public static class Constants
    {
        public const string OpenFileFilter =
            "All Spectroscopy Files|*.csv;*.dsw;*.par;*.spc;fid;acqus;1r;1i;procs;*.json|" +
            "EPR Files (*.par;*.spc)|*.par;*.spc|" +
            "NMR Files (fid;acqus;1r;1i;procs)|fid;acqus;1r;1i;procs|" +
            "UV/Vis Files (*.dsw)|*.dsw|" +
            "CSV Files (*.csv)|*.csv|" +
            "All Files|*.*";

        public const string SaveFileFilter =
            "Vector Image (*.svg)|*.svg|" +
            "PNG Image (*.png)|*.png|" +
            "ASCII (*.dat)|*.dat|" +
            "CSV File (*csv)|*.csv";
    }
}
