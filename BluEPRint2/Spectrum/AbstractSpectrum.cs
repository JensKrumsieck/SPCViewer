namespace BluEPRint2.Spectrum
{

    //abstract class for functions every spectrum needs
    public abstract class AbstractSpectrum : ISpectrum
    {
        protected double[] xAxis;
        protected double[] yAxis;
        private double[] derivative;
        private double[] integral;
        public string fileName;

        //empty constructor
        public AbstractSpectrum()
        {
            //does nothing
        }

        //constructor with x & y data 
        public AbstractSpectrum(double[] xData, double[] yData)
        {
            this.xAxis = xData;
            this.yAxis = yData;
        }

        //returns data for x&y axis
        public double[] getX()
        {
            return this.xAxis;
        }
        public double[] getY()
        {
            return this.yAxis;
        }

        //integral & derivative data
        public double[] integrate(double[] data)
        {
            double[] intData = new double[data.Length];
            for (int i = 0; i <= data.Length - 1; i++)
            {
                if (i != 0) intData[i] = intData[i - 1] + data[i];
                else intData[i] = data[i];
            }
            return intData;
        }

        public double[] derive(double[] xData, double[] yData)
        {
            double[] derivData = new double[yData.Length];
            for (int i = 0; i <= yData.Length - 1; i++)
            {
                if (i != 0)
                {
                    derivData[i] = (yData[i] - yData[i - 1]) / (xData[i] - xData[i - 1]);
                }
                else derivData[i] = 0;
            }
            return derivData;
        }

        //and return
        public double[] getIntegral()
        {
            if(this.integral == null || this.integral.Length != this.getY().Length)
            {
                this.integral = this.integrate(this.getY());
            }
            return this.integral;
        }
        public double[] getDerivative()
        {
            if (this.derivative == null || this.derivative.Length != this.getY().Length)
            {
                this.derivative = this.derive(this.getX(), this.getY());
            }
            return this.derivative;
        }

        //return filename
        public string getFileName()
        {
            return this.fileName;
        }

        public void loadData(double[] xData, double[] yData)
        {
            this.xAxis = xData;
            this.yAxis = yData;
        }
    }
}
