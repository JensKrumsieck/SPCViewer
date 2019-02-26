namespace BluEPRint2
{
    class Constants
    {
        //planck
        public const double planckh = 6.626070040e-34; //Js
        public const string planckhUnit = "Js";

        //mass electron
        public const double emass = 9.10938356e-31; //kg
        public const string emassUnit = "kg";

        //e charge
        public const double echarge = 1.6021766208e-19; //C
        public const string echargeUnit = "C";

        //bohrm
        public const double bohrm = (echarge * planckh / (2 * System.Math.PI)) / (2 * emass);
        public const string bohrmUnit = "J/T";

        //gfree
        public const double gfree = 2.00231930436182; //unitless

        //boltzmann
        public const double boltzm = 1.38064852e-23; //J/K
        public const string boltzmUnit = "J/K";
    }
}
