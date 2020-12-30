namespace SPCViewer.Core.Extension
{
    public static class OxyPlotConversion
    {

        /// <summary>
        /// Converts CS -> OP
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static OxyPlot.DataPoint Mapping(this ChemSharp.DataPoint input) => new OxyPlot.DataPoint(input.X, input.Y);

    }
}
