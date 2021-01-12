using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using TinyMVVM.Utility;

namespace SPCViewer.Core
{
    public sealed class Settings : Singleton<Settings>
    {
        #region Font
        public string Font { get; set; }
        public int FontWeight { get; set; }
        public int FontSize { get; set; }
        #endregion

        #region PlotArea
        public double BorderThickness { get; set; }
        public double Padding { get; set; }
        #endregion

        #region Axis
        public string AxisFormat { get; set; }
        public double AxisThickness { get; set; }
        #endregion

        #region Series
        public double SeriesThickness { get; set; }
        public double AnnotationThickness { get; set; }
        #endregion

        #region Export
        public float ExportWidth { get; set; }
        public float ExportHeight { get; set; }
        public float ExportDPI { get; set; }
        #endregion

        #region Colors
        public string ExperimentalColor { get; set; }
        public string IntegralColor { get; set; }
        public string DerivativeColor { get; set; }
        public string AnnotationColor { get; set; }
        #endregion

        /// <summary>
        /// json ctor
        /// use for json only!
        /// For accessing settings use <see cref="Singleton.Instance"/>
        /// </summary>
        [JsonConstructor]
        public Settings() { }

        /// <summary>
        /// Load Settings
        /// Loads defaults if no path is given...
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path = "")
        {
            var settings = Default();
            if (File.Exists(path))
            {
                var file = File.ReadAllText(path);
                settings = JsonSerializer.Deserialize<Settings>(file);
            }
            // ReSharper disable once InconsistentNaming

            var properties = typeof(Settings).GetProperties();
            foreach (var p in properties.Where(s => s.PropertyType != typeof(Settings))) p.SetValue(this, p.GetValue(settings));

            //save here to update potential missing settings into file
            Instance.Save();
        }

        /// <summary>
        /// Save Settings
        /// </summary>
        public void Save()
        {
            var content = JsonSerializer.Serialize(Instance);
            File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}/settings.json", content);
        }

        /// <summary>
        /// Load Default Settings
        /// </summary>
        /// <returns></returns>
        private static Settings Default() =>
            new Settings()
            {
                FontWeight = 200,
                Font = "Arial",
                FontSize = 14,
                BorderThickness = 1.5,
                Padding = 1.5,
                AxisFormat = "{0} / {1}",
                AxisThickness = 0,
                ExperimentalColor = "#222831",
                DerivativeColor = "#ff5722",
                IntegralColor = "#2d4059",
                AnnotationColor = "#0092ca",
                AnnotationThickness = 1.5,
                SeriesThickness = 1.5,
                ExportWidth = 2000,
                ExportHeight = 1000,
                ExportDPI = 300
            };
    }
}
