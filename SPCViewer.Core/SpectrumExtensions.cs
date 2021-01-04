using ChemSharp.DataProviders;
using ChemSharp.Spectroscopy;
using ChemSharp.Spectroscopy.DataProviders;
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

        /// <summary>
        /// Builds the Parameter Dictionary from provider
        /// </summary>
        /// <param name="prov"></param>
        /// <param name="paramsDictionary"></param>
        /// <returns></returns>
        private static Dictionary<string, string> ParameterDictionary(this IParameterProvider prov, Dictionary<string, (string, Type)> paramsDictionary)
        {
            var dic = new Dictionary<string, string>();
            foreach (var (key, (description, _)) in paramsDictionary)
            {
                if (prov.Storage.ContainsKey(key))
                    dic.Add(description, prov[key]);
            }
            return dic;
        }

        /// <summary>
        /// Returns Dictionary with special parameters
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetSpecialParameters(this Spectrum input)
        {
            var dic = input.DataProvider switch
            {
                BrukerEPRProvider epr => epr.ParameterDictionary(BrukerEPRParameterMap),
                BrukerNMRProvider nmr => nmr.ParameterDictionary(BrukerNMRParameterMap),
                _ => new Dictionary<string, string>()
            };
            dic.Add("Path", input.Title);
            dic.Add("Creation Date", input.CreationDate.ToString("dd. MMM yyyy HH:mm:ss"));
            return dic;
        }
    }
}
