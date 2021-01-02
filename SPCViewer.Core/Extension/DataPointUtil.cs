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
    }
}
