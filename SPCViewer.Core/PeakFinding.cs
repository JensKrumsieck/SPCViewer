using System.Collections.Generic;
using System.Linq;

namespace SPCViewer.Core
{
    public static class PeakFinding
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="rangeOfPeaks"></param>
        /// <returns></returns>
        public static IList<int> FindPeakPositions(this IList<double> values, int rangeOfPeaks = 20)
        {
            var peaks = new List<int>();

            var checksOnEachSide = rangeOfPeaks / 2;
            for (var i = 0; i < values.Count; i++)
            {
                var current = values[i];
                IEnumerable<double> range = values;

                if (i > checksOnEachSide) range = range.Skip(i - checksOnEachSide);

                range = range.Take(rangeOfPeaks);
                if (range.Any() && current.Equals(range.Max())) peaks.Add(i);
            }
            return peaks;
        }
    }
}
