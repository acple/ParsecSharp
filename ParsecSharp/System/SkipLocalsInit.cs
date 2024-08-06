using System.Runtime.CompilerServices;

[module: SkipLocalsInit]

#if !NET
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Module, Inherited = false)]
    internal sealed class SkipLocalsInitAttribute : Attribute
    { }
}
#endif
