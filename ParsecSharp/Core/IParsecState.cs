namespace Parsec
{
    public interface IParsecState<out T>
    {
        T Current { get; }

        bool HasValue { get; }

        IPosition Position { get; }
    }
}
