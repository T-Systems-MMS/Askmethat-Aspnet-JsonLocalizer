﻿using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Askmethat.Aspnet.JsonLocalizer.Test.Helpers;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using LocalizedString = Microsoft.Extensions.Localization.LocalizedString;

namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{
    [TestClass]
    public class FallbackJsonFileTest
    {
        private JsonStringLocalizer localizer = null;
        public void InitLocalizer(string cultureString)
        {
            SetCurrentCulture(cultureString);

            localizer = JsonStringLocalizerHelperFactory.Create(new JsonLocalizationOptions()
            {
                DefaultCulture = null,
                SupportedCultureInfos = new System.Collections.Generic.HashSet<CultureInfo>()
                {
                     new CultureInfo("fr"),
                     new CultureInfo("en"),
                     new CultureInfo("zh-CN"),
                     new CultureInfo("en-AU")
                },
                ResourcesPath = "fallback",
            });
        }

        [TestMethod]
        public void Should_Read_Color_NoFallback()
        {
            InitLocalizer("en-AU");
            var result = localizer.GetString("Color");
            Assert.AreEqual("Colour (specific)", result);

            InitLocalizer("fr");
            result = localizer.GetString("Color");
            Assert.AreEqual("Couleur (neutral)", result);

            InitLocalizer(CultureInfo.InvariantCulture.ThreeLetterISOLanguageName);
            result = localizer.GetString("Color");
            Assert.AreEqual("Color (invariant)", result);
        }

        [TestMethod]
        public void Should_Read_Color_FallbackToParent()
        {
            InitLocalizer("fr-FR");
            var result = localizer.GetString("Color");
            Assert.AreEqual("Couleur (neutral)", result);
            Assert.IsFalse(result.ResourceNotFound);

            InitLocalizer("en-NZ");
            result = localizer.GetString("Color");
            Assert.AreEqual("Color (neutral)", result);
            Assert.IsFalse(result.ResourceNotFound);

            InitLocalizer("zh-CN");
            result = localizer.GetString("Color");
            Assert.AreEqual("Color (invariant)", result);
            Assert.IsFalse(result.ResourceNotFound);

        }

        [TestMethod]
        public void Should_Read_ResourceMissingCulture_FallbackToResourceName()
        {
            InitLocalizer("zh-CN");
            var result = localizer.GetString("Empty");
            Assert.AreEqual("Empty", result);
            Assert.IsTrue(result.ResourceNotFound);
        }

        [TestMethod]
        public void Should_Read_MissingResource_FallbackToResourceName()
        {
            InitLocalizer("en-AU");
            var result = localizer.GetString("No resource string");
            Assert.AreEqual("No resource string", result);
            Assert.IsTrue(result.ResourceNotFound);
        }

        [TestMethod]
        public void Should_Read_AllStringsWithParentFallback()
        {
            InitLocalizer("en-AU");

            var results = localizer.GetAllStrings(includeParentCultures: true).ToArray();
            var expected = new[] {
                new LocalizedString("Color", "Colour (specific)", false),
                new LocalizedString("Empty", "Empty", false)
            };
            CollectionAssert.AreEqual(expected, results, new LocalizedStringComparer());
        }

        [TestMethod]
        public void Should_Read_AllStringsWithoutParentFallback()
        {

            InitLocalizer("en-AU");

            var results = localizer.GetAllStrings(includeParentCultures: false).ToArray();
            var expected = new[] {
                new LocalizedString("Color", "Colour (specific)", false)
            };
            CollectionAssert.AreEqual(expected, results, new LocalizedStringComparer());
        }

        /// <summary>
        /// LocalizedString doesn't implement the IComparer interface required by CollectionAssert.AreEqual(), so providing one here
        /// </summary>
        private class LocalizedStringComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var lsX = (LocalizedString)x;
                var lsY = (LocalizedString)y;
                if (ReferenceEquals(lsX, lsY))
                {
                    return 0;
                }
                if (lsX.Name == lsY.Name && lsX.Value == lsY.Value && lsX.ResourceNotFound == lsY.ResourceNotFound)
                {
                    return 0;
                }
                int result = StringComparer.CurrentCulture.Compare(lsX.Name, lsY.Name);
                if (result != 0)
                {
                    return result;
                }
                result = StringComparer.CurrentCulture.Compare(lsX.Value, lsY.Value);
                if (result != 0)
                {
                    return result;
                }
                return lsX.ResourceNotFound.CompareTo(lsY.ResourceNotFound);
            }
        }

        private void SetCurrentCulture(string cultureName)
            => SetCurrentCulture(new CultureInfo(cultureName));

        private void SetCurrentCulture(CultureInfo cultureInfo)
            => CultureInfo.CurrentUICulture = cultureInfo;
    }
}
