using OxyPlot;

namespace BluEPRint2.Utility
{
    public class EPRDataPoint : IDataPointProvider
    {
        public double X { get; set; }
        public double Y { get; set; }

        public EPRDataPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public DataPoint GetDataPoint()
        {
            return new DataPoint(X, Y);
        }
    }
}
