using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Askmethat.Aspnet.JsonLocalizer.Extensions
{
    public class JsonLocalizationOptions : LocalizationOptions
    {
        private const char PLURAL_SEPARATOR = '|';
        private const string DEFAULT_RESOURCES = "Resources";
        private const string DEFAULT_CULTURE = "en-US";

        /// <summary>
        /// Base path of your resources.
        /// The plugin will browse the folder and sub-folders and load all present
        /// JSON files matching the settings,
        /// depending on the "UseBaseName"-property.
        /// </summary>
        public new string ResourcesPath { get; set; } = DEFAULT_RESOURCES;

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(30);
        public IMemoryCache Caching { get; set; } = new MemoryCache(new MemoryCacheOptions
        {
        });

        private CultureInfo defaultCulture = new CultureInfo(DEFAULT_CULTURE);

        public CultureInfo DefaultCulture
        {
            get
            {
                return defaultCulture;
            }
            set                 
            {
                if (value != defaultCulture)
                {
                    defaultCulture = value ?? CultureInfo.InvariantCulture;
                }
            }
        }

        private HashSet<CultureInfo> supportedCultureInfos = new HashSet<CultureInfo>
        {
            
        };

        public HashSet<CultureInfo> SupportedCultureInfos
        {
            get
            {
                return supportedCultureInfos;
            }
            set
            {
                if (value != supportedCultureInfos)
                {
                    supportedCultureInfos = value;
                }
            }
        }


        public bool IsAbsolutePath { get; set; } = false;

        public Encoding FileEncoding { get; set; } = Encoding.UTF8;

        public bool UseBaseName { get; set; } = false;
        public char PluralSeparator { get; set; } = PLURAL_SEPARATOR;
    }
}
