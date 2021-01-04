namespace SPCViewer.Core.Extension
{
    public static class OxyPlotConversion
    {
        /// <summary>
        /// Converts CS -> OP
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static OxyPlot.DataPoint Mapping(this ChemSharp.DataPoint input) => input.Mapping(1);

        /// <summary>
        /// Converts CS -> OP
        /// </summary>
        /// <param name="input"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static OxyPlot.DataPoint Mapping(this ChemSharp.DataPoint input, double factor) => new OxyPlot.DataPoint(input.X, input.Y / factor);

    }
}
