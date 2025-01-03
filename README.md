# ParsecSharp
[![NuGet](https://img.shields.io/nuget/v/ParsecSharp)](https://www.nuget.org/packages/ParsecSharp/)

The faster monadic parser combinator library for C#

> [!Important]
>
> ParsecSharp v4 has some major breaking-changes from v3. Please check the following notes.
>
> #### The basic parser type is now an interface `IParser<TToken, T>`, instead of a class `Parser<TToken, T>`
> The main data types like Parser, Result, Success and Failure have been changed to interfaces to enable covariant result types.
> To follow this change, you can simply rename it, and it should work.
> ```diff
> - private static readonly Parser<char, int> integer = Many1(DecDigit()).ToInt();
> + private static readonly IParser<char, int> integer = Many1(DecDigit()).ToInt();
> ```
>
> #### The operator `|` for Choice composition is no longer available in netstandard2.0
> Unfortunately, netstandard2.0 doesn't support binary operator overloading for interfaces.
> Please use netstandard2.1 compatibles, or rewrite it with other combinators that have the same semantics.
> ```csharp
> var textChar = unescapedChar | escapedChar; // no longer available in netstandard2.0
> var textChar = unescapedChar.Or(escapedChar); // use Or combinator
> var textChar = Choice(unescapedChar, escapedChar); // use Choice combinator
> ```


## What's this
This library provides the most useful Text Parsers, Stream Parsers, and Parser Combinators.
All APIs are pure, immutable, and can combine with any others.
Designed to utilize JIT Compiler optimizations, it realizes complete immutability and can parse infinitely recursive data structures.

This project is inspired by [parsec](https://hackage.haskell.org/package/parsec), a monadic parser library for Haskell.


## Concept
* Easy construction APIs with monads in C#
* Pure/Immutable/Functional framework in C#
* Replace regular expressions in your code (applicable from smallest to largest)
* Most readable API source code


## Overview
* Strictly typed parsers/combinators that support natural type inference
* A lot of reasonable built-in parsers/combinators
* Supports parsing infinitely recursive data structures
* Supports full backtracking: Parsing Expression Grammar (PEG) style parsing strategy
* Supports parsing streams with any token type (e.g., string, char stream, byte array, binary stream)
* Supports tokenization
* Supports partial parsing
* Supports custom deriviation for core types
* Supports nullable reference types (with C# 8.0 or later)
* Supports Source Link (that allows to refer every parser implementation source codes)
* No additional dependencies
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

PackageReference:

```csproj
<ItemGroup>
  <PackageReference Include="ParsecSharp" Version="*" />
</ItemGroup>
```

Download manually:

> [NuGet gallery](https://www.nuget.org/packages/ParsecSharp/)


## Supported platform
* netstandard2.1 (compatible with net5.0 or later, netcoreapp, with some performance improvements and additional implementations that depend on new runtime features)
* netstandard2.0 (compatible with net461 or later, uap, xamarin, and more)

**Requires C# 7.3 or later** for generic overloading resolution.
Recommends C# 13.0 or later for better overloading resolution via OverloadResolutionPriority.


## Get started
1. Add the package reference to your project.
2. Add the using directives: `using static ParsecSharp.Parser;` and `using static ParsecSharp.Text;` to your code.
3. Parse your all.


## How to use
* [Starter's tutorial](UnitTest.ParsecSharp/Tutorial.cs)

#### Implementation examples
* [JsonParser implementation](ParsecSharp.Examples/JsonParser.cs)
* [CsvParser implementation](ParsecSharp.Examples/CsvParser.cs)
* [Arithmetic expression parser implementation](ParsecSharp.Examples/ExpressionParser.cs)
* [PEG parser generator implementation](ParsecSharp.Examples/PegParser.cs)

Documentation is included in the [UnitTest code](UnitTest.ParsecSharp/ParserTest.cs).

If you want more information, read the [API source code](ParsecSharp/Parser), all is there.


## Questions?
Feel free to create an Issue!


## License
This software is released under the MIT License, see [LICENSE](LICENSE).

#### Using
* [Fody](https://github.com/Fody/Fody) ([MIT](https://github.com/Fody/Fody/blob/master/License.txt)): Only for internal use; does NOT propagate license acceptance to users.
