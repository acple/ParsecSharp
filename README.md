# ParsecSharp
[![Nuget](https://img.shields.io/nuget/v/ParsecSharp)](https://www.nuget.org/packages/ParsecSharp/)

The faster monadic parser combinator library for C#


## What's this
This library provides most useful Text Parsers, Stream Parsers, and Parser Combinators.
All APIs are pure, immutable, can combine any others.
Designed to utilize JIT Compilers optimization, realizes completely immutable, and can parse infinitely recursive data structures.

This project is affected by [parsec](https://hackage.haskell.org/package/parsec), monadic parser library for Haskell.


## Concept
* Easy construction APIs with monad on C#
* Pure/Immutable/Functional framework on C#
* Replace regular expressions on your code (applicable from smallest to largest)
* Most readable APIs source code


## Overview
* Strictly typed parsers/combinators that support natural type inference
* A lot of reasonable built-in parsers/combinators
* Supports parsing infinitely recursive data structures
* Supports full backtracking: Parsing Expression Grammar (PEG) style parsing strategy
* Supports parsing streams with any token type (e.g. string, char stream, byte array, binary stream)
* Supports tokenization
* Supports partial parsing
* Supports nullable reference types (with C# 8.0 or later)
* Faster running
* Just enough error messages
* No left-recursion support
* No packrat parsing support (because it increases parsing time in most cases)


## How to install

### from NuGet
dotnet-cli:

```sh
$ dotnet add package ParsecSharp
```

NuGet Package Manager Console:

```powershell
> Install-Package ParsecSharp
```

Download manually:

> [NuGet Gallery](https://www.nuget.org/packages/ParsecSharp/)


## Supported platform
* netstandard1.6 (compatible with netcoreapp, net461 or later, uap, xamarin and more)
* netstandard2.0 (provides same APIs as netstandard1.6, but less dependencies)
* netstandard2.1 (some performance improvements and some additional APIs that depends new runtime features)

**Requires C# 7.3 or later** for generic overloading resolution.


## Get started
1. Add package reference to your project.
2. Add using directive: `using static ParsecSharp.Parser;` and `using static ParsecSharp.Text;` to your code.
3. Parse your all.


## How to use
* [Starter's tutorial](UnitTest.ParsecSharp/Tutorial.cs)

Implementation examples:

* [JsonParser implementation](ParsecSharp.Examples/JsonParser.cs)
* [CsvParser implementation](ParsecSharp.Examples/CsvParser.cs)

Documents are included in [UnitTest code](UnitTest.ParsecSharp/ParserTest.cs) (jp).

If you want more information, read [APIs source code](ParsecSharp/Parser), all is there.


## License
This software is released under the MIT License, see [LICENSE](LICENSE).

#### Using
* [Fody](https://github.com/Fody/Fody) ([MIT](https://github.com/Fody/Fody/blob/master/License.txt)): Only for internal use, does NOT propagate license acceptance to users.
