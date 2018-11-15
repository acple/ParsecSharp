namespace Parsec
{
    public interface ISuspendedResult<TToken, T>
    {
        void Deconstruct(out Result<TToken, T> result, out IParsecStateStream<TToken> rest);
    }
}
