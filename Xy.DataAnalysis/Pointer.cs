using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Xy.DataAnalysis
{
    public abstract class PointerBase
    {
        public Core Core { get; }
        public Expression<Func<int>> GetAddress { get; }
        public int Address { get { return GetAddress.Compile().Invoke(); } }

        protected PointerBase(Core core, int baseAddress) :
            this(core, () => baseAddress)
        {
        }
        protected PointerBase(Core core, Expression<Func<int>> getAddress)
        {
            Core = core;
            GetAddress = getAddress;
        }

        /// <summary>
        /// Check if the pointer is static or dynamic
        /// </summary>
        /// <returns>Returns true if static, false if dynamic</returns>
        public bool IsStatic()
        {
            return GetAddress.Body.NodeType != ExpressionType.Add;
        }

        /// <summary>
        /// Calculate how deep the pointer is nested
        /// </summary>
        public int GetDepth()
        {
            var depth = 0;
            var pointer = this as PointerBase;

            // navigating up expression chain until reached an static address
            while (!pointer.IsStatic())
            {
                var binaryExpression = pointer.GetAddress.Body as BinaryExpression;
                var leftOperand = binaryExpression.Left as MemberExpression;

                pointer = Expression.Lambda<Func<PointerBase>>(leftOperand.Expression).Compile().Invoke();
                depth++;
            }

            return depth;
        }


        /// <summary>
        /// Get the offset, or the right-hand operand of the defining expression.
        /// </summary>
        public int GetOffset()
        {
            return !(GetAddress.Body is BinaryExpression) ? 0 :
                Expression.Lambda<Func<int>>((GetAddress.Body as BinaryExpression).Right).Compile().Invoke();
        }

        public object GetValue()
        {
            return this.GetType().GetProperty("Value").GetValue(this, null);
        }


        public override string ToString()
        {
            return string.Format("{0:X3} -> {1}", Address, GetValue());
        }
    }
    public class Pointer : PointerBase
    {
        public int Value { get { return Core.ReadInt(Address); } }

        protected Pointer(Core core, Expression<Func<int>> getAddress) : base(core, getAddress)
        {
        }

        /// <summary>
        /// Create a multi-level pointer
        /// </summary>
        /// <param name="address">Static address</param>
        /// <param name="offsets">Array of offset</param>
        public static Pointer MultiPointer(Core core, int address, params int[] offsets)
        {
            return offsets.Aggregate(Pointer.FromStaticAddress(core, address), (x, offset) => x + offset);
        }

        /// <summary>
        /// Create a static pointer
        /// </summary>
        /// <param name="address">Static address</param>
        /// <returns></returns>
        public static Pointer FromStaticAddress(Core core, int address)
        {
            return new Pointer(core, () => address);
        }

        /// <returns>A pointer pointing to the value of base address offseted</returns>
        public static Pointer operator +(Pointer baseAddress, int offset)
        {
            return new Pointer(baseAddress.Core, () => baseAddress.Value + offset);
        }
    }

    public class BaseAddressAttribute : Attribute { }
    public class HiddenAttribute : Attribute { }
    public class HexadecimalAttribute : Attribute { }

    public class WString
    {
        public readonly string Value;
        public WString(string value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(WString s)
        {
            return s.Value;
        }
    }
    public class GB2312 //variable length encoding
    {
        public readonly string Value;
        public GB2312(string value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(GB2312 s)
        {
            return s.Value;
        }
    }

    public class Pointer<T> : PointerBase
    {
        public T Value
        {
            get
            {
                var type = typeof(T);
                if (type.IsEnum) type = type.GetEnumUnderlyingType();

                if (type == typeof(byte))
                    return (T)(object)Core.ReadBytes(Address, 1).First();
                if (type == typeof(int))
                    return (T)(object)Core.ReadInt(Address);
                else if (type == typeof(float))
                    return (T)(object)Core.ReadFloat(Address);
                else if (type == typeof(string))
                {
                    var buffer = Core.ReadBytes(Core.ReadInt(Address), 255);
                    return (T)(object)Encoding.UTF8.GetString(buffer).Split('\0');
                }
                else if (type == typeof(WString))
                {
                    var buffer = Core.ReadBytes(Core.ReadInt(Address), 255 * 2);
                    return (T)(object)new WString(Encoding.Unicode.GetString(buffer).Split('\0').First());
                }
                else if (type == typeof(GB2312))
                {
                    var buffer = Core.ReadBytes(Core.ReadInt(Address), 255 * 2);
                    return (T)(object)new GB2312(Encoding.GetEncoding("GB2312").GetString(buffer).Split('\0').First());
                }
                else
                    throw new NotImplementedException("Unexpected Type : " + type.Name);
            }
        }

        public void SetValue(T value)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"[+{GetOffset().ToString("X3")}]->{Value}";
        }

        // hide default public constructor
        private Pointer(Pointer pointer) : base(pointer.Core, pointer.GetAddress)
        {
        }
        public static implicit operator Pointer<T>(Pointer pointer)
        {
            return new Pointer<T>(pointer);
        }
        public static implicit operator T(Pointer<T> pointer)
        {
            return pointer.Value;
        }

        public static bool operator ==(Pointer<T> a, Pointer<T> b)
        {
            if (object.ReferenceEquals(a, b))
                return true;

            return Comparer<T>.Default.Compare(a, b) == 0;
        }
        public static bool operator !=(Pointer<T> a, Pointer<T> b)
        {
            return !(a == b);
        }
    }
}


