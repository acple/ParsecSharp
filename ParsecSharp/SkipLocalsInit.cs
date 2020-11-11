using System.Runtime.CompilerServices;

[module: SkipLocalsInit]

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Module, Inherited = false)]
    internal sealed class SkipLocalsInitAttribute : Attribute
    { }
}
