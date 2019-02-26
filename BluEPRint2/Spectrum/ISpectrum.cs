namespace BluEPRint2.Spectrum
{
    //standard interface for spectra
    interface ISpectrum
    {
        //returns data for x&y axis
        double[] getX();
        double[] getY();

        //integral & derivative data
        double[] integrate(double[] data);
        double[] derive(double[] yData, double[] xData);

        //and return
        double[] getIntegral();
        double[] getDerivative();

        //return filename
        string getFileName();

        void loadData(double[] xData, double[] yData);

    }
}
