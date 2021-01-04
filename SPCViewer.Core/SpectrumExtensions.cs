using System;
using System.Collections.Generic;

namespace SPCViewer.Core
{
    public static class SpectrumExtensions
    {
        /// <summary>
        /// Maps ParameterProvider[key] key to displayed key, type for Bruker EPR
        /// </summary>
        private static readonly Dictionary<string, (string, Type)> BrukerEPRParameterMap = new Dictionary<string, (string, Type)>
        {
            {"MF", ("Frequency", typeof(double))},
            {"MP", ("Power", typeof(double))},
            {"RRG", ("Receiver Gain", typeof(double))},
            {"RTC", ("TimeConversion", typeof(double))},
            {"RCT", ("Conversion Time", typeof(double))},
            {"JNS", ("Number Of Scans", typeof(int))},
            {"RMA", ("Modulation Amplitude", typeof(double))},
            {"JCO", ("Comment", typeof(string))},
            {"JON", ("Operator", typeof(string))},
            {"JEX", ("Experiment", typeof(string))},
        };

        /// <summary>
        /// Maps ParameterProvider[key] key to displayed key, type for Bruker NMR
        /// </summary>
        private static readonly Dictionary<string, (string, Type)> BrukerNMRParameterMap = new Dictionary<string, (string, Type)>
        {
            {"##$SFO1", ("Frequency", typeof(double))},
            {"##$NUC1", ("Experiment", typeof(string))},
            {"##$SOLVENT", ("Solvent", typeof(string))},
            {"##$INSTRUM", ("Device", typeof(string))},
        };
    }
}
