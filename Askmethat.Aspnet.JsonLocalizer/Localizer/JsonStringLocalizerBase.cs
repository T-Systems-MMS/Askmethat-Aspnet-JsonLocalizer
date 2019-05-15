using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.Format;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    internal class JsonStringLocalizerBase
    {
        protected Dictionary<string, LocalizatedFormat> Localization;
        protected readonly IMemoryCache MemCache;
        protected readonly IOptions<JsonLocalizationOptions> LocalizationOptions;
        protected string ResourcesRelativePath;
        protected readonly string BaseName;

        protected readonly TimeSpan MemCacheDuration;
        protected const string CACHE_KEY = "LocalizationBlob";
        protected string CurrentCulture = string.Empty;
        public JsonStringLocalizerBase(IOptions<JsonLocalizationOptions> localizationOptions, string baseName = null)
        {
            BaseName = CleanBaseName(baseName);
            LocalizationOptions = localizationOptions;
            MemCache = LocalizationOptions.Value.Caching;
            MemCacheDuration = LocalizationOptions.Value.CacheDuration;
        }

        private string GetCacheKey(CultureInfo ci)
        {
            if (LocalizationOptions.Value.UseBaseName)
            {
                return $"{CACHE_KEY}_{ci.DisplayName}_{BaseName}";
            }
            return $"{CACHE_KEY}_{ci.DisplayName}";
        }

        private void SetCurrentCultureToCache(CultureInfo ci) => CurrentCulture = ci.Name;
        protected bool IsUICultureCurrentCulture(CultureInfo ci)
        {
            return string.Equals(CurrentCulture, ci.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        protected void GetCultureToUse(CultureInfo cultureToUse)
        {
            // https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/issues/52#issuecomment-492104806
            if (MemCache.TryGetValue(GetCacheKey(cultureToUse), out Localization))
            {
                SetCurrentCultureToCache(cultureToUse);
                return;
            }

            if (MemCache.TryGetValue(GetCacheKey(cultureToUse.Parent), out Localization))
            {
                SetCurrentCultureToCache(cultureToUse.Parent);
                return;
            }

            if (MemCache.TryGetValue(GetCacheKey(LocalizationOptions.Value.DefaultCulture), out Localization))
            {
                SetCurrentCultureToCache(LocalizationOptions.Value.DefaultCulture);
            }
        }

    protected void InitJsonStringLocalizer()
    {
        AddMissingCultureToSupportedCulture(CultureInfo.CurrentUICulture);
        AddMissingCultureToSupportedCulture(LocalizationOptions.Value.DefaultCulture);

        foreach (CultureInfo ci in LocalizationOptions.Value.SupportedCultureInfos)
        {
            InitJsonStringLocalizer(ci);
        }

        //after initialization, get current ui culture
        GetCultureToUse(CultureInfo.CurrentUICulture);
    }

    protected void AddMissingCultureToSupportedCulture(CultureInfo cultureInfo)
    {
        if (!LocalizationOptions.Value.SupportedCultureInfos.Contains(cultureInfo))
        {
            LocalizationOptions.Value.SupportedCultureInfos.Add(cultureInfo);
        }
    }

    protected void InitJsonStringLocalizer(CultureInfo currentCulture)
    {
        //Look for cache key.
        if (!MemCache.TryGetValue(GetCacheKey(currentCulture), out Localization))
        {
            ConstructLocalizationObject(ResourcesRelativePath, currentCulture);
            // Set cache options.
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(MemCacheDuration);

            // Save data in cache.
            MemCache.Set(GetCacheKey(currentCulture), Localization, cacheEntryOptions);
        }
    }

    /// <summary>
    /// Construct localization object from json files
    /// </summary>
    /// <param name="jsonPath">Json file path</param>
    private void ConstructLocalizationObject(string jsonPath, CultureInfo currentCulture)
    {
        //be sure that localization is always initialized
        if (Localization == null)
        {
            Localization = new Dictionary<string, LocalizatedFormat>();
        }

        var myFiles = GetMatchingJsonFiles(jsonPath);

        foreach (string file in myFiles)
        {
            Dictionary<string, JsonLocalizationFormat> tempLocalization = JsonConvert.DeserializeObject<Dictionary<string, JsonLocalizationFormat>>(File.ReadAllText(file, LocalizationOptions.Value.FileEncoding));
            foreach (KeyValuePair<string, JsonLocalizationFormat> temp in tempLocalization)
            {
                LocalizatedFormat localizedValue = GetLocalizedValue(currentCulture, temp);
                if (!(localizedValue.Value is null))
                {
                    if (!Localization.ContainsKey(temp.Key))
                    {
                        Localization.Add(temp.Key, localizedValue);
                    }
                    else if (Localization[temp.Key].IsParent)
                    {
                        Localization[temp.Key] = localizedValue;
                    }
                }
            }
        }
    }

    private IEnumerable<string> GetMatchingJsonFiles(string jsonPath)
    {
        var searchPattern = "*.json";
        var searchOption = SearchOption.AllDirectories;
        var basePath = jsonPath;
        if (LocalizationOptions.Value.UseBaseName && !string.IsNullOrWhiteSpace(BaseName))
        {
            /*
             https://docs.microsoft.com/de-de/aspnet/core/fundamentals/localization?view=aspnetcore-2.2#dataannotations-localization
                Using the option ResourcesPath = "Resources", the error messages in RegisterViewModel can be stored in either of the following paths:
                Resources/ViewModels.Account.RegisterViewModel.fr.resx
                Resources/ViewModels/Account/RegisterViewModel.fr.resx
             */

            searchOption = SearchOption.TopDirectoryOnly;
            var friendlyName = AppDomain.CurrentDomain.FriendlyName;

            var shortName = BaseName.Replace($"{friendlyName}.", "");

            basePath = Path.Combine(jsonPath, TransformNameToPath(shortName));
            if (Directory.Exists(basePath))
            {
                // We can search something like Resources/ViewModels/Account/RegisterViewModel/*.json
                searchPattern = "*.json";
            }
            else
            {  // We search something like Resources/ViewModels/Account/RegisterViewModel.json
                var lastDot = shortName.LastIndexOf('.');
                var className = shortName.Substring(lastDot + 1);
                // Remove class name from shortName so we can use it as folder.
                var baseFolder = shortName.Substring(0, lastDot);
                baseFolder = TransformNameToPath(baseFolder);

                basePath = Path.Combine(jsonPath, baseFolder);

                if (Directory.Exists(basePath))
                {
                    searchPattern = $"{className}?.json";
                }
                else
                { // We search something like Resources/ViewModels.Account.RegisterViewModel.json
                    basePath = jsonPath;
                    searchPattern = $"{shortName}?.json";
                }
            }
        }

        // Get all files ending by json extension
        return Directory.GetFiles(basePath, searchPattern, searchOption);
    }

    private string TransformNameToPath(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            return name.Replace(".", Path.DirectorySeparatorChar.ToString());
        }
        return null;
    }

    private string CleanBaseName(string baseName)
    {
        if (string.IsNullOrWhiteSpace(baseName)) return baseName;
        var plusIdx = baseName.IndexOf('+');
        if (plusIdx == -1)
        {
            return baseName;
        }

        // Nested classes are seperated by + and should use the translation of their parent class.
        return baseName.Substring(0, plusIdx);
    }

    private LocalizatedFormat GetLocalizedValue(CultureInfo currentCulture, KeyValuePair<string, JsonLocalizationFormat> temp)
    {
        bool isParent = false;
        string value = temp.Value.Values.FirstOrDefault(s => string.Equals(s.Key, currentCulture.Name, StringComparison.InvariantCultureIgnoreCase)).Value;
        if (value is null)
        {
            isParent = true;
            value = temp.Value.Values.FirstOrDefault(s => string.Equals(s.Key, currentCulture.Parent.Name, StringComparison.InvariantCultureIgnoreCase)).Value;
            if (value is null)
            {
                value = temp.Value.Values.FirstOrDefault(s => string.IsNullOrWhiteSpace(s.Key)).Value;
                if (value is null && LocalizationOptions.Value.DefaultCulture != null)
                {
                    value = temp.Value.Values.FirstOrDefault(s => string.Equals(s.Key, LocalizationOptions.Value.DefaultCulture.Name, StringComparison.InvariantCultureIgnoreCase)).Value;
                }
            }
        }
        return new LocalizatedFormat()
        {
            IsParent = isParent,
            Value = value
        };
    }



}
}