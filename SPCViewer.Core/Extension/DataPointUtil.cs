using System;
using ChemSharp;
using System.Collections.Generic;
using System.Linq;
using OxyDataPoint = OxyPlot.DataPoint;

namespace SPCViewer.Core.Extension
{
    public static class DataPointUtil
    {
        /// <summary>
        /// Get Chemsharp DataPoints from OxyPlot Tuple
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pick"></param>
        /// <returns></returns>
        public static DataPoint[] PointsFromRect(this IEnumerable<DataPoint> data, (OxyDataPoint, OxyDataPoint) pick)
        {
            var (item1, item2) = pick;
            var min = item1.X < item2.X ? item1 : item2;
            var max = item1.X > item2.X ? item1 : item2;
            return data.Where(s => s.X >= min.X && s.X <= max.X).ToArray();
        }

        /// <summary>
        /// Gets Chemsharp DataPoint from OxyDataPoint
        /// </summary>
        /// <param name="data"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static DataPoint FromOxyDataPoint(this IEnumerable<DataPoint> data, OxyDataPoint point) => data.FirstOrDefault(s => Math.Abs(s.X - point.X) < 1e-9);

        /// <summary>
        /// Converts CS -> OP
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static OxyDataPoint Mapping(this DataPoint input) => input.Mapping(1);

        /// <summary>
        /// Converts CS -> OP
        /// </summary>
        /// <param name="input"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static OxyDataPoint Mapping(this DataPoint input, double factor) => new OxyDataPoint(input.X, input.Y / factor);
    }
}
