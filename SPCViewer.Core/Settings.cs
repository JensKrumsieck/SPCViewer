using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SPCViewer.Core
{
    public sealed class Settings
    {
        [JsonIgnore]
        private static readonly Lazy<Settings> Lazy = new Lazy<Settings>(() => new Settings());

        /// <summary>
        /// Returns Global instance
        /// </summary>
        /// <returns></returns>
        public static Settings Instance => Lazy.Value;

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
        #endregion

        /// <summary>
        /// json ctor
        /// use for json only!
        /// For accessing settings use <see cref="Instance"/>
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

        }

        /// <summary>
        /// Save Settings
        /// </summary>
        public void Save()
        {
            var content = JsonSerializer.Serialize(Instance);
            File.WriteAllText("settings.json", content);
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
                AxisFormat = "{0} / {1}"
            };
    }
}
