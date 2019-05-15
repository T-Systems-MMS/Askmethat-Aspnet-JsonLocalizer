# Askmethat-Aspnet-JsonLocalizer
Json Localizer library for .NetStandard and .NetCore Asp.net projects

#### Nuget

[![Askmethat.Aspnet.JsonLocalizer package in T-Systems-MMS feed in Azure Artifacts](https://feeds.dev.azure.com/T-Systems-MMS/_apis/public/Packaging/Feeds/8ed18c8e-4f19-4994-be32-0a1f0893af0f/Packages/a2efa605-6110-4bdd-902e-fe9282ad2899/Badge)](https://dev.azure.com/T-Systems-MMS/Askmethat-Aspnet-JsonLocalizer/_packaging?_a=package&feed=8ed18c8e-4f19-4994-be32-0a1f0893af0f&package=a2efa605-6110-4bdd-902e-fe9282ad2899&preferRelease=true)

#### Build

[![Build Status](https://dev.azure.com/T-Systems-MMS/Askmethat-Aspnet-JsonLocalizer/_apis/build/status/Askmethat-Aspnet-JsonLocalizer-CI?branchName=master)](https://dev.azure.com/T-Systems-MMS/Askmethat-Aspnet-JsonLocalizer/_build/latest?definitionId=3&branchName=master)

# Project

This library allows users to use JSON files instead of RESX in an ASP.NET application.
The code tries to be most compliant with Microsoft guidelines.
The library is compatible with NetStandard & NetCore.

# Configuration

An extension method is available for `IServiceCollection`.
You can have a look at the method [here](https://github.com/T-Systens-MMS/Askmethat-Aspnet-JsonLocalizer/blob/development/Askmethat.Aspnet.JsonLocalizer/Extensions/JsonLocalizerServiceExtension.cs)

## Options 

A set of options is available.
You can define them like this : 

``` cs
services.AddJsonLocalization(options => {
        options.CacheDuration = TimeSpan.FromMinutes(15);
        options.ResourcesPath = "mypath";
        options.FileEncoding = Encoding.GetEncoding("ISO-8859-1");
        options.SupportedCultureInfos = new HashSet<CultureInfo>(new[]
        {
          new CultureInfo("en-US"),
          new CultureInfo("fr-FR")
        });
    });
```

### Current Options

- **SupportedCultureInfos** : _Default value : _List containing only default culture_ and CurrentUICulture. Optional array of cultures that you should provide to plugin. _(Like RequestLocalizationOptions)
- **ResourcesPath** : _Default value : `$"{_env.WebRootPath}/Resources/"`_.  Base path of your resources. The plugin will browse the folder and sub-folders and load all present JSON files matching the settings, depending on the "UseBaseName"-property
- **CacheDuration** : _Default value : 30 minutes_. We cache all values to memory to avoid loading files for each request, this parameter defines the time after which the cache is refreshed.
- **FileEncoding** : _default value : UTF8_. Specify the file encoding.
- **IsAbsolutePath** : *_default value : false*. Look for an absolute path instead of project path.
- **UseBaseName** : *_default value : false*. Use base name location for Views and constructors like default Resx localization in **ResourcePathFolder**. Please have a look at the documentation below to see the different possiblities for structuring your translation files.
- **Caching** : *_default value: MemoryCache*. Internal caching can be overwritted by using custom class that extends IMemoryCache.
- **PluralSeparator** : *_default value: |*. Seperator used to get singular or pluralized version of localization. More information in *Pluralization*

#### Search patterns when UseBaseName = true

If UseBaseName is set to true, it will be searched for lingualization files by the following order - skipping the options below if any option before matches.

- If you use a non-typed IStringLocalizer all files in the Resources-directory, including all subdirectories, will be used to find a localization. This can cause unpredictable behavior if the same key is used in multiple files.

- If you use a typed localizer, the following applies - Namespace is the "short namespace" without the root namespace:
  - Nested classes will use the translation file of their parent class.
  - If there is a folder named "Your/Namespace/And/Classname", all contents of this folder will be used.
  - If there is a folder named "Your/Namespace" the folder will be searched for all json-files beginning with your classname.
  - Otherwise there will be searched for a json-file starting with "Your.Namespace.And.Classname" in your Resources-folder.

# Pluralization

In version 2.0.0, Pluralization was introduced.
You are now able to manage a singular (left) and plural (right) version for the same Key. 
*PluralSeparator* is used as separator between the two strings.

For example : User|Users for key Users

To use plural string, use paramters from [IStringLocalizer](https://github.com/aspnet/AspNetCore/blob/def36fab1e45ef7f169dfe7b59604d0002df3b7c/src/Mvc/Mvc.Localization/src/LocalizedHtmlString.cs), if last parameters is a boolean, pluralization will be activated.


Pluralization is available with IStringLocalizer, IViewLocalizer and HtmlStringLocalizer :

**localizer.GetString("Users", true)**;

# Information

**Platform Support**

## 2.0.0+

|Platform|Version|
| -------------------  | :------------------: |
|NetStandard|2.0+|
|NetCore|2.0.0+|

**WithCulture method**

**WhithCulture** method is not implemented and will not be implemented. ASP.NET Team, start to set this method **Obsolete** for version 3 and will be removed in version 4 of asp.net core.

For more information : 
https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/issues/46

# Performances

After talking with others Devs about my package, they asked me about performance.

So I added a benchmark project and here the last results.

## 2.0.0+

``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.437 (1809/October2018Update/Redstone5)
Intel Core i7-6820HQ CPU 2.70GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.202
  [Host]     : .NET Core 2.2.3 (CoreCLR 4.6.27414.05, CoreFX 4.6.27414.05), 64bit RyuJIT
  DefaultJob : .NET Core 2.2.3 (CoreCLR 4.6.27414.05, CoreFX 4.6.27414.05), 64bit RyuJIT


```
|                                          Method |           Mean |         Error |        StdDev |            Min |            Max |     Ratio | RatioSD |   Gen 0 |   Gen 1 |  Gen 2 | Allocated |
|------------------------------------------------ |---------------:|--------------:|--------------:|---------------:|---------------:|----------:|--------:|--------:|--------:|-------:|----------:|
|                                       Localizer |       121.0 ns |      1.585 ns |      1.482 ns |       118.7 ns |       124.2 ns |      1.00 |    0.00 |       - |       - |      - |         - |
|                                   JsonLocalizer |       105.6 ns |      1.236 ns |      1.096 ns |       104.1 ns |       107.1 ns |      0.87 |    0.02 |  0.0113 |       - |      - |      48 B |
|                       JsonLocalizerWithCreation | 1,563,465.3 ns | 32,968.125 ns | 44,011.499 ns | 1,512,751.4 ns | 1,667,846.1 ns | 13,015.77 |  465.56 | 64.4531 | 31.2500 | 3.9063 |  275312 B |
| JsonLocalizerWithCreationAndExternalMemoryCache |     6,182.3 ns |    122.984 ns |    120.787 ns |     5,916.1 ns |     6,381.1 ns |     51.12 |    0.85 |  0.8774 |  0.4349 |      - |    3696 B |
|                JsonLocalizerDefaultCultureValue |       359.4 ns |      7.173 ns |      6.359 ns |       350.2 ns |       370.5 ns |      2.97 |    0.06 |  0.0892 |       - |      - |     376 B |
|                    LocalizerDefaultCultureValue |       387.0 ns |      9.330 ns |      8.727 ns |       378.6 ns |       412.8 ns |      3.20 |    0.09 |  0.0777 |       - |      - |     328 B |


# Contributors

[@lethek](https://github.com/lethek) : 
- [#20](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/pull/20)
- [#17](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/pull/17)

[@lugospod](https://github.com/lugospod) :
- [#43](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/pull/43)
- [#44](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/pull/44)

[@Compufreak345](https://github.com/Compufreak345)

# License

[MIT Licence](https://github.com/T-Systems-MMS/Askmethat-Aspnet-JsonLocalizer/blob/master/LICENSE)
