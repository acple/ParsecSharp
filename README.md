# ParsecSharp
Monadic parser combinator library for C#

## What's this
This library provides most useful Text Parsers, Stream Parsers, and Parser Combinators.
All APIs are pure, immutable, can combine any others.
Designed to utilize JIT Compilers optimization, realizes completely immutable, and can parse infinitely recursive data structures.

This project is affected by [parsec](https://hackage.haskell.org/package/parsec), monadic parser library for Haskell.


## Concept
* Monad on C#
* Pure/Immutable/Functional programming on C#
* Pursues possibility of .NET JIT Compiler


## How to install

### from NuGet
NuGet Package Manager Console:

```powershell
> Install-Package ParsecSharp
```

dotnet-cli:

```sh
> dotnet add package ParsecSharp
```

Download manually:

> [NuGet Gallery](https://www.nuget.org/packages/ParsecSharp/)


## Supported platform
**Caution** This project is verrry experimental, now targets only x64 runtime, with RyuJit (Next-Gen .NET JIT Compiler).

* netstandard1.0 (compatible with net45 or later, netcoreapp, uap, xamarin, and more)


## Get started
1. Add package reference to your project.
2. Add using directive: `using static ParsecSharp.Parser;` and `using static ParsecSharp.Text;` to your code.
3. Parse your all.


## How to use
Here is examples:

* [JsonParser implementation](ParsecSharp.Examples/JsonParser.cs)
* [CsvParser implementation](ParsecSharp.Examples/CsvParser.cs)

Documents are included in [UnitTest code](UnitTest.ParsecSharp/ParserTest.cs) (jp)

If you want more information, read [APIs source code](ParsecSharp/Parser), all is there.


## License
This software is released under the MIT License, see [LICENSE](LICENSE).
