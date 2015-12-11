namespace Parsec
{
    public interface IParsecState<T>
    {
        T Current { get; }

        bool HasValue { get; }

        IPosition Position { get; }
    }
}
