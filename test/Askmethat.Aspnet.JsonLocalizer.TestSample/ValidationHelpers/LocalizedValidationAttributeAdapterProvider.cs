using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace Askmethat.Aspnet.JsonLocalizer.TestSample.ValidationHelpers
{
    /// <summary>
    /// https://stackoverflow.com/a/50230263/3085985
    /// </summary>
    public class LocalizedValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        private readonly ValidationAttributeAdapterProvider _originalProvider = new ValidationAttributeAdapterProvider();

        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            attribute.ErrorMessage = "Validation_" + attribute.GetType().Name.Replace("Attribute", string.Empty);

            // You might need this if you have custom DataTypeAttribute, for us this just creates
            // "EmailAddress_EmailAddress" instead of "EmailAddress" as key.
            //if (attribute is DataTypeAttribute dataTypeAttribute)
            //    attribute.ErrorMessage += "_" + dataTypeAttribute.DataType;

            return _originalProvider.GetAttributeAdapter(attribute, stringLocalizer);
        }
    }
}
