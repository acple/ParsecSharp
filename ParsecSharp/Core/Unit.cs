using System;

namespace Parsec
{
    public sealed class Unit : IEquatable<Unit>
    {
        public static Unit Instance { get; } = new Unit();

        private Unit()
        { }

        public bool Equals(Unit other)
            => other != null;

        public override bool Equals(object obj)
            => obj is Unit;

        public override int GetHashCode()
            => 0;

        public override string ToString()
            => "()";
    }
}
