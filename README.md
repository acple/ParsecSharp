# ParsecSharp
[![NuGet](https://img.shields.io/nuget/v/ParsecSharp)](https://www.nuget.org/packages/ParsecSharp/)

The faster monadic parser combinator library for C#


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
* F12 (Go To Definition) friendly


## Overview
* Strictly typed parsers/combinators that support natural type inference
* A lot of reasonable built-in parsers/combinators
* Supports parsing infinitely recursive data structures
* Supports full backtracking: Parsing Expression Grammar (PEG) style parsing strategy
* Supports parsing streams of any token type (e.g., string, char stream, binary stream, any enumerable)
* Supports tokenization
* Supports partial parsing
* Supports custom derivation for core types
* Supports nullable reference types (with C# 8.0 or later)
* Supports Source Link (that allows to refer to every parser implementation source code)
* No additional dependencies
* Faster running
* Just enough error messages
* No left-recursion support
* No packrat parsing support (because it increases parsing time in most cases)


## How to install

### From NuGet
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


## Supported platforms
* net10.0 (provides full functionality, best performance)
* netstandard2.1 (provides same functionality as net10.0, compatible with net5.0 or later, mono, unity, and more)
* netstandard2.0 (works on legacy environments, some runtime features are disabled, compatible with net461 or later, uap, xamarin, and more)

Recommends C# 14.0 or later to enable all library features.
If you would like to support legacy environments, recommends creating a netstandard2.0 library project with C# 14.0 and referencing it.

* **Requires** C# 7.3 or later: for generic overloading resolution
* C# 8.0 or later: nullable reference types enabled
* C# 13.0 or later: overloading resolution improved via OverloadResolutionPriority
* C# 14.0 or later: extension operators enabled

Note: This library may not work on runtimes with poor optimization capabilities.


## Get started
1. Add the package reference to your project.
2. Add the using directives: `using static ParsecSharp.Parser;` and `using static ParsecSharp.Text;` to your code.
3. Parse everything.


## How to use
* [Starter tutorial](UnitTest.ParsecSharp/ParserTests/Tutorial.cs)

#### Implementation examples
* [JSON parser implementation](ParsecSharp.Examples/JsonParser.cs)
* [CSV parser implementation](ParsecSharp.Examples/CsvParser.cs)
* [Arithmetic expression parser implementation](ParsecSharp.Examples/ExpressionParser.cs)
* [PEG parser generator implementation](ParsecSharp.Examples/PegParser.cs)

Documentation is available in the [UnitTest code](UnitTest.ParsecSharp/ParserTests).

If you want more information, check out the [API source code](ParsecSharp/Parser), it's all there.


## Questions?
Feel free to create an Issue!


## License
This software is released under the MIT License, see [LICENSE](LICENSE).

#### Using
* [Fody](https://github.com/Fody/Fody) ([MIT](https://github.com/Fody/Fody/blob/master/License.txt)): Only for internal use; does NOT propagate license acceptance to users.
