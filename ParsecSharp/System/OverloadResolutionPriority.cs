#if !NET9_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, Inherited = false)]
    internal sealed class OverloadResolutionPriorityAttribute(int priority) : Attribute
    {
        public int Priority => priority;
    }
}
#endif
