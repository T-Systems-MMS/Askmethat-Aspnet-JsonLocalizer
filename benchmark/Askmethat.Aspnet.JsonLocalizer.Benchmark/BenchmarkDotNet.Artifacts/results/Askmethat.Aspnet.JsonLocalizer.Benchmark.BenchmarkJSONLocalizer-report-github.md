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
