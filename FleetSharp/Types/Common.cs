using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;

namespace FleetSharp.Types
{
    public enum Network
    {
        Mainnet = 0 << 4,
        Testnet = 1 << 4
    }

    public enum AddressType
    {
        P2PK = 1,
        P2SH = 2,
        P2S = 3
    }

    public enum BuildOutputType
    {
        Default = 0,
        EIP12 = 1
    }

    public enum SortingDirection
    {
        Ascending,
        Descending
    }

    public delegate bool FilterPredicate<T>(T item);
    public delegate dynamic SortingSelector<T>(T item);
    /*
    public class Amount : IEquatable<Amount>
    {
        private readonly long _value;

        public Amount()
        {
            _value = 0;
        }

        public Amount(string value)
        {
            _value = long.Parse(value);
        }

        public Amount(long value)
        {
            _value = value;
        }

        public static implicit operator Amount(string value)
        {
            return new Amount(value);
        }

        public static implicit operator Amount(long value)
        {
            return new Amount(value);
        }

        public static implicit operator long(Amount value)
        {
            return value._value;
        }

        public static implicit operator string(Amount value)
        {
            return value.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is Amount amount && Equals(amount);
        }

        public bool Equals(Amount other)
        {
            return _value.Equals(other._value);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }*/
}
