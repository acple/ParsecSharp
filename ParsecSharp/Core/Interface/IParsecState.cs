namespace Parsec
{
    public interface IParsecState<out TToken>
    {
        TToken Current { get; }

        bool HasValue { get; }

        IPosition Position { get; }
    }
}
