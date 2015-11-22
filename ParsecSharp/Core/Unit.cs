namespace Parsec
{
    public sealed class Unit
    {
        public static Unit Instance { get; } = new Unit();

        private Unit()
        { }

        public override bool Equals(object obj)
            => obj is Unit;

        public static bool operator ==(Unit _, Unit __)
            => true;

        public static bool operator !=(Unit _, Unit __)
            => false;

        public override int GetHashCode()
            => 0;

        public override string ToString()
            => "()";
    }
}
