using ChemSharp.DataProviders;
using ChemSharp.Spectroscopy.DataProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace SPCViewer.Core
{

    public static class ExtensionHandler
    {
        /// <summary>
        /// Contains a file extension - type relationship
        /// </summary>
        public static Dictionary<string, Type> DataProviderDictionary = new Dictionary<string, Type>();

        static ExtensionHandler()
        {
            DataProviderDictionary.Add("par", typeof(BrukerEPRProvider));
            DataProviderDictionary.Add("spc", typeof(BrukerEPRProvider));
            DataProviderDictionary.Add("dsw", typeof(VarianUVVisProvider));
            DataProviderDictionary.Add("fid", typeof(BrukerNMRProvider));
            DataProviderDictionary.Add("1r", typeof(BrukerNMRProvider));
            DataProviderDictionary.Add("1i", typeof(BrukerNMRProvider));
            DataProviderDictionary.Add("acqus", typeof(BrukerNMRProvider));
            DataProviderDictionary.Add("procs", typeof(BrukerNMRProvider));
        }

        /// <summary>
        /// Handles IDataProvider creation
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AbstractXYDataProvider Handle(string path)
        {
            var ext = GetExtension(path);
            //check if ext is supported
            if (!DataProviderDictionary.ContainsKey(ext)) throw new FileLoadException($"File type {ext} is not supported");
            var type = DataProviderDictionary[ext];
            var param = Expression.Parameter(typeof(string), "path");
            var creator = Expression
                .Lambda<Func<string, AbstractXYDataProvider>>(
                    Expression.New(
                        type.GetConstructor(new[] { typeof(string) }) ??
                        throw new InvalidOperationException("null given, Extension Handling failed"), param), param)
                .Compile();
            return creator(path);
        }

        /// <summary>
        /// Get Extension as string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetExtension(string path)
        {
            var ext = Path.GetExtension(path);
            //fallback for nmr files
            ext = string.IsNullOrEmpty(ext) ? Path.GetFileName(path) : ext.Remove(0, 1);
            return ext.ToLower();
        }
    }
}
