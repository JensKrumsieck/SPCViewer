using OxyPlot.Axes;
using System;
using System.Diagnostics;

namespace BluEPRint2.Plot
{
    class LinkedAxis : LinearAxis
    {
        public Axis parent;
    
        public LinkedAxis(LinearAxis parent,  Func<double, double> converter, Func<double,double>invconverter)
        {
            this.InvConverter = invconverter;
            this.Converter = converter;
            this.parent = parent;
            this.StartPosition = 1;
            this.EndPosition = 0;
            //set borders
            this.Maximum = DoConversion(parent.AbsoluteMinimum);
            this.Minimum = DoConversion(parent.AbsoluteMaximum);
            this.AbsoluteMinimum = DoConversion(parent.AbsoluteMaximum);
            this.AbsoluteMaximum = DoConversion(parent.AbsoluteMinimum);
        }

        public Func<double, double> Converter { get; set; }
        public Func<double, double> InvConverter { get; set; }

        public override bool IsXyAxis()
        {
            return true;
        }

        public override bool IsLogarithmic()
        {
            return this.parent.IsLogarithmic();
        }

        public double DoConversion(double value)
        {
            if (this.Converter != null)
            {
                return this.Converter(value);
            }
            else return value;
        }

        public double DoInverseConversion(double value)
        {
            if (this.InvConverter != null)
            {
                return this.InvConverter(value);
            }
            else return value;
        }

        /// <summary>

        /// Inverse transforms the specified screen coordinate. This method can only be used with non-polar coordinate systems.

        /// </summary>

        /// <param name="sx">The screen coordinate.</param>

        /// <returns>The value.</returns>

        /// <summary>

        /// Inverse transforms the specified screen coordinate. This method can only be used with non-polar coordinate systems.

        /// </summary>

        /// <param name="sx">The screen coordinate.</param>

        /// <returns>The value.</returns>

        public override double InverseTransform(double sx)

        {

            // Inline the <see cref="PostInverseTransform" /> method here.
            double inv = DoInverseConversion(sx);
            double y = parent.InverseTransform(inv);
            Debug.WriteLine(sx + " wird zu " + inv + " trans zu " + y);
            return y;

        }



        /// <summary>

        /// Transforms the specified coordinate to screen coordinates.

        /// </summary>

        /// <param name="x">The value.</param>

        /// <returns>The transformed value (screen coordinate).</returns>

        public override double Transform(double x)
        {

            double inv = DoInverseConversion(x);
            double y = parent.Transform(inv);
            Debug.WriteLine(x + " wird zu " + inv + " trans zu " + y);
            return y;

        }


    }
}
