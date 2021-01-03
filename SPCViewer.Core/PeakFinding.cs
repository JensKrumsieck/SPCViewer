using System;
using System.Collections.Generic;
using System.Linq;

namespace SPCViewer.Core
{
    public static class PeakFinding
    {
        private const int MaxPeaks = 32;
        /// <summary>
        /// Peak Finding Algorithm
        /// translated C-code by Hong Xu  to C# from
        /// https://github.com/xuphys/peakdetect/blob/master/peakdetect.c
        /// ### (FreeBSD License); Copyright 2011, 2013 Hong Xu. All rights reserved. ###
        /// originally inspired by Eli Billauer's peakdet for MATLAB:
        /// http://billauer.co.il/peakdet.html
        /// </summary>
        /// <param name="values">Input values</param>
        /// <param name="threshold">search threshold, 0 = auto</param>
        /// <param name="includeMin">also search for minima?</param>
        /// <returns></returns>
        public static IList<int> FindPeakPositions(this IList<double> values, double threshold = 0, bool includeMin = false)
        {
            var delta = threshold == 0
                ? values.Max(Math.Abs) * .025
                : threshold;
            double mn = values[0], mx = values[0];
            int mnPos = 0, mxPos = 0;
            var maxim = new List<int>();
            var minim = new List<int>();
            var lfm = true;
            for (var i = 0; i < values.Count; i++)
            {
                if (maxim.Count > MaxPeaks || minim.Count > MaxPeaks) break;
                if (values[i] > mx) mx = values[mxPos = i];
                if (values[i] < mn) mn = values[mnPos = i];
                if (lfm && values[i] < mx - delta)
                {
                    maxim.Add(mxPos);
                    lfm = false;
                    i = mxPos - 1;
                    mn = values[mnPos = mxPos];
                }
                else if (!lfm && values[i] > mn + delta)
                {
                    if (includeMin) minim.Add(mnPos);
                    lfm = true;
                    i = mnPos - 1;
                    mx = values[mxPos = mnPos];
                }
            }
            return maxim.Concat(minim).ToList();
        }
    }
}
