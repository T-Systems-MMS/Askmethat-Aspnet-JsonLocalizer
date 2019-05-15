﻿using Microsoft.Extensions.Caching.Memory;
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
        /// We cache all values to memory to avoid loading files for each request, this parameter defines the time after which the cache is refreshed.
        /// </summary>
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(30);

        /// <summary>
        /// This property stores the MemoryCache for the cached translations.
        /// </summary>
        public IMemoryCache Caching { get; set; } = new MemoryCache(new MemoryCacheOptions
        {
        });

        private CultureInfo defaultCulture = new CultureInfo(DEFAULT_CULTURE);

        /// <summary>
        /// Sets the default culture to use.
        /// </summary>
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

        /// <summary>
        /// Optional array of cultures that you should provide to plugin. (Like RequestLocalizationOptions)
        /// </summary>
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

        /// <summary>
        /// Look for an absolute path instead of project path.
        /// </summary>
        public bool IsAbsolutePath { get; set; } = false;

        /// <summary>
        /// Specify the file encoding.
        /// </summary>
        public Encoding FileEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Use base name location for Views and constructors like default Resx localization in ResourcePathFolder. 
        /// </summary>
        public bool UseBaseName { get; set; } = false;

        /// <summary>
        /// Seperator used to get singular or pluralized version of localization.
        /// </summary>
        public char PluralSeparator { get; set; } = PLURAL_SEPARATOR;
    }
}
