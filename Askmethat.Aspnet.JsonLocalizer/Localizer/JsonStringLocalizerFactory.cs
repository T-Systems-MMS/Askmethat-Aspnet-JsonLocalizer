using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    /// <summary>
    /// Factory the create the JsonStringLocalizer
    /// </summary>
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IHostingEnvironment _env;
        private readonly IOptions<JsonLocalizationOptions> _localizationOptions;

        public JsonStringLocalizerFactory(
                IHostingEnvironment env,
                IOptions<JsonLocalizationOptions> localizationOptions = null)
        {
            _env = env;
            _localizationOptions = localizationOptions ?? throw new ArgumentNullException(nameof(localizationOptions));
        }


        public IStringLocalizer Create(Type resourceSource)
        {
            return this.Create(resourceSource.FullName, string.Empty);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            baseName = _localizationOptions.Value.UseBaseName ? baseName : string.Empty;
            return (IStringLocalizer)new JsonStringLocalizer(_localizationOptions, _env, baseName);
        }
    }
}
