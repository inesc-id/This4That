#region Copyright (c) 2009-2016 Misakai Ltd.
/*************************************************************************
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as
* published by the Free Software Foundation, either version 3 of the
* License, or(at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with this program.If not, see<http://www.gnu.org/licenses/>.
*************************************************************************/
#endregion Copyright (c) 2009-2016 Misakai Ltd.

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Emitter.Threading
{
    /// <summary>
    /// Represents an atomic unsigned 16 bytes integer value.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct AtomicUInt16 : IAtomicValue<ushort>, IComparable, IComparable<AtomicUInt16>
    {
        private int fValue;

        private AtomicUInt16(ushort value)
        {
            fValue = value;
        }

        /// <summary>
        /// Gets the value of the atomic structure
        /// </summary>
        public ushort Value
        {
            get { return (ushort)fValue; }
        }

        /// <summary>
        /// Atomically pefroms an assignment operation to the atomic value
        /// </summary>
        /// <param name="value">Value to set</param>
        public void Assign(ushort value)
        {
            int oldValue;
            int newValue;

            do
            {
                oldValue = (ushort)fValue;
                newValue = value;
            } while (Interlocked.CompareExchange(ref fValue, newValue, oldValue) != oldValue);
        }

        /// <summary>
        /// Atomically pefroms a computation and assigns it to the atomic value
        /// </summary>
        /// <param name="computation">Computation to execute atomically</param>
        public void Assign(Func<ushort, ushort> computation)
        {
            int oldValue;
            int newValue;

            do
            {
                oldValue = fValue;
                newValue = (ushort)computation((ushort)oldValue);
            } while (Interlocked.CompareExchange(ref fValue, newValue, oldValue) != oldValue);
        }

        /// <summary>
        /// Performs an atomic addition of the given value to the current atomic value.
        /// </summary>
        /// <param name="value">The value to add to the current atomic value.</param>
        public void Add(ushort value)
        {
            Interlocked.Add(ref fValue, value);
        }

        /// <summary>
        /// Performs an atomic subtraction of the given value from the current atomic value.
        /// </summary>
        /// <param name="value">The value to subtract from the current atomic value.</param>
        public void Subtract(ushort value)
        {
            Interlocked.Add(ref fValue, -value);
        }

        /// <summary>
        /// Performs an atomic multiplication of the given value and the current atomic value.
        /// </summary>
        /// <param name="value">The value to multiply the current atomic value.</param>
        public void Multiply(ushort value)
        {
            int oldValue;
            int newValue;

            do
            {
                oldValue = (ushort)fValue;
                newValue = (ushort)fValue * value;
            } while (Interlocked.CompareExchange(ref fValue, newValue, oldValue) != oldValue);
        }

        /// <summary>
        /// Performs an atomic division of the current atomic value by the given value.
        /// </summary>
        /// <param name="value">The value to divide the current atomic value by.</param>
        public void Divide(ushort value)
        {
            int oldValue;
            int newValue;

            do
            {
                oldValue = (ushort)fValue;
                newValue = (ushort)fValue / value;
            } while (Interlocked.CompareExchange(ref fValue, newValue, oldValue) != oldValue);
        }

        /// <summary>
        /// Compares two values for equality and, if they are equal, replaces one of the values.
        /// </summary>
        /// <param name="value1">First value to compare.</param>
        /// <param name="value2">Second value to compare.</param>
        /// <returns>The original value in value1.</returns>
        public bool CompareExchange(ushort value1, ushort value2)
        {
            return Interlocked.CompareExchange(ref fValue, value1, value2) == value2;
        }

        /// <summary>
        /// Atomically decrements current value.
        /// </summary>
        public void Decrement()
        {
            Interlocked.Decrement(ref fValue);
        }

        /// <summary>
        /// Atomically increments the current value.
        /// </summary>
        public void Increment()
        {
            Interlocked.Increment(ref fValue);
        }

        /// <summary>
        /// Atomically performs a left shift operation by the given value.
        /// </summary>
        /// <param name="value">The amount to atomically shift by.</param>
        public void LeftShift(int value)
        {
            int oldValue;
            int newValue;

            do
            {
                oldValue = (ushort)fValue;
                newValue = (ushort)fValue << value;
            } while (Interlocked.CompareExchange(ref fValue, newValue, oldValue) != oldValue);
        }

        /// <summary>
        /// Atomically performs a right shift operation by the given value.
        /// </summary>
        /// <param name="value">The amount to atomically shift by.</param>
        public void RightShift(int value)
        {
            int oldValue;
            int newValue;

            do
            {
                oldValue = (ushort)fValue;
                newValue = (ushort)fValue >> value;
            } while (Interlocked.CompareExchange(ref fValue, newValue, oldValue) != oldValue);
        }

        /// <summary>
        /// Atomically computes one's completement of the current value.
        /// </summary>
        public void OneComplement()
        {
            int oldValue;
            int newValue;

            do
            {
                oldValue = (ushort)fValue;
                newValue = ~(ushort)fValue;
            } while (Interlocked.CompareExchange(ref fValue, newValue, oldValue) != oldValue);
        }

        /// <summary>
        /// Atomically computes bitewise AND on the current value.
        /// </summary>
        /// <param name="value">Value to compute the AND operation with.</param>
        public void BitwiseAnd(ushort value)
        {
            int oldValue;
            int newValue;

            do
            {
                oldValue = (ushort)fValue;
                newValue = (ushort)fValue & value;
            } while (Interlocked.CompareExchange(ref fValue, newValue, oldValue) != oldValue);
        }

        /// <summary>
        /// Atomically computes bitewise OR on the current value.
        /// </summary>
        /// <param name="value">Value to compute the OR operation with.</param>
        public void BitwiseInclusiveOr(ushort value)
        {
            int oldValue;
            int newValue;

            do
            {
                oldValue = (ushort)fValue;
                newValue = (ushort)fValue | value;
            } while (Interlocked.CompareExchange(ref fValue, newValue, oldValue) != oldValue);
        }

        /// <summary>
        /// Atomically computes exclusive bitewise OR on the current value.
        /// </summary>
        /// <param name="value">Value to compute the exclusive OR operation with.</param>
        public void BitwiseExclusiveOr(ushort value)
        {
            int oldValue;
            int newValue;

            do
            {
                oldValue = (ushort)fValue;
                newValue = (ushort)fValue ^ value;
            } while (Interlocked.CompareExchange(ref fValue, newValue, oldValue) != oldValue);
        }

        /// <summary>
        /// Performs a bitwise shift and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The amount of bytes to shift.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator >>(AtomicUInt16 left, int right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue >> right));
        }

        /// <summary>
        /// Performs a bitwise shift and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The amount of bytes to shift.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator <<(AtomicUInt16 left, int right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue << right));
        }

        /// <summary>
        /// Performs an addition and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The value to add.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator +(AtomicUInt16 left, ushort right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue + right));
        }

        /// <summary>
        /// Performs an addition and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The value to add.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator +(AtomicUInt16 left, AtomicUInt16 right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue + (ushort)right.fValue));
        }

        /// <summary>
        /// Performs a subtraction and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The value to subtract.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator -(AtomicUInt16 left, ushort right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue - right));
        }

        /// <summary>
        /// Performs a subtraction and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The value to subtract.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator -(AtomicUInt16 left, AtomicUInt16 right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue - (ushort)right.fValue));
        }

        /// <summary>
        /// Performs a multiplication and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The value to multiply by.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator *(AtomicUInt16 left, ushort right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue * right));
        }

        /// <summary>
        /// Performs a multiplication and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The value to multiply by.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator *(AtomicUInt16 left, AtomicUInt16 right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue * (ushort)right.fValue));
        }

        /// <summary>
        /// Performs a division and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The value to divide by.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator /(AtomicUInt16 left, ushort right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue / right));
        }

        /// <summary>
        /// Performs a division and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The value to divide by.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator /(AtomicUInt16 left, AtomicUInt16 right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue / (ushort)right.fValue));
        }

        /// <summary>
        /// Performs a modulo operation and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The value to divide by.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator %(AtomicUInt16 left, ushort right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue % right));
        }

        /// <summary>
        /// Performs a modulo operation and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The value to divide by.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator %(AtomicUInt16 left, AtomicUInt16 right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue % (ushort)right.fValue));
        }

        /// <summary>
        /// Performs a bitwise and operation and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator &(AtomicUInt16 left, ushort right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue & right));
        }

        /// <summary>
        /// Performs a bitwise and operation and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator &(AtomicUInt16 left, AtomicUInt16 right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue & (ushort)right.fValue));
        }

        /// <summary>
        /// Performs a bitwise OR operation and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator |(AtomicUInt16 left, ushort right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue | right));
        }

        /// <summary>
        /// Performs a bitwise OR operation and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator |(AtomicUInt16 left, AtomicUInt16 right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue | (ushort)right.fValue));
        }

        /// <summary>
        /// Performs a bitwise exclusive OR operation and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator ^(AtomicUInt16 left, ushort right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue ^ right));
        }

        /// <summary>
        /// Performs a bitwise exclusive OR operation and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator ^(AtomicUInt16 left, AtomicUInt16 right)
        {
            return new AtomicUInt16((ushort)((ushort)left.fValue ^ (ushort)right.fValue));
        }

        /// <summary>
        /// Performs a bitwise complement operation and returns a new atomic value for the result of the operation.
        /// </summary>
        /// <param name="value">The value to compute the complement of.</param>
        /// <returns>A new atomic value that represents the result of the operation.</returns>
        public static AtomicUInt16 operator ~(AtomicUInt16 value)
        {
            return new AtomicUInt16((ushort)~(ushort)value.fValue);
        }

        /// <summary>
        /// Atomically increments the current value.
        /// </summary>
        /// <param name="value">The value to increment.</param>
        /// <returns>Incremented value</returns>
        public static AtomicUInt16 operator ++(AtomicUInt16 value)
        {
            value.Increment();
            return value;
        }

        /// <summary>
        /// Atomically decrements the current value.
        /// </summary>
        /// <param name="value">The value to decrements.</param>
        /// <returns>Decremented value</returns>
        public static AtomicUInt16 operator --(AtomicUInt16 value)
        {
            value.Decrement();
            return value;
        }

        /// <summary>
        /// Compares the equality of two values.
        /// </summary>
        /// <param name="left">Left parameter to compare.</param>
        /// <param name="right">Right parameter to compare.</param>
        /// <returns>Returns whether left and right parameters are equals or not.</returns>
        public static bool operator ==(AtomicUInt16 left, AtomicUInt16 right)
        {
            return (ushort)left.fValue == (ushort)right.fValue;
        }

        /// <summary>
        /// Compares the equality of two values.
        /// </summary>
        /// <param name="left">Left parameter to compare.</param>
        /// <param name="right">Right parameter to compare.</param>
        /// <returns>Returns whether left and right parameters are equals or not.</returns>
        public static bool operator ==(AtomicUInt16 left, ushort right)
        {
            return (ushort)left.fValue == right;
        }

        /// <summary>
        /// Checks for inequality of two values.
        /// </summary>
        /// <param name="left">Left parameter to compare.</param>
        /// <param name="right">Right parameter to compare.</param>
        /// <returns>Returns true whether left and right parameters are not equals, otherwise false.</returns>
        public static bool operator !=(AtomicUInt16 left, AtomicUInt16 right)
        {
            return (ushort)left.fValue != (ushort)right.fValue;
        }

        /// <summary>
        /// Checks for inequality of two values.
        /// </summary>
        /// <param name="left">Left parameter to compare.</param>
        /// <param name="right">Right parameter to compare.</param>
        /// <returns>Returns true whether left and right parameters are not equals, otherwise false.</returns>
        public static bool operator !=(AtomicUInt16 left, ushort right)
        {
            return (ushort)left.fValue != right;
        }

        /// <summary>
        /// Checks whether the left argument is smaller than the right one.
        /// </summary>
        /// <param name="left">Left parameter to compare.</param>
        /// <param name="right">Right parameter to compare.</param>
        /// <returns>True if the left argument is smaller than the right one, false otherwise.</returns>
        public static bool operator <(AtomicUInt16 left, AtomicUInt16 right)
        {
            return (ushort)left.fValue < (ushort)right.fValue;
        }

        /// <summary>
        /// Checks whether the left argument is smaller than the right one.
        /// </summary>
        /// <param name="left">Left parameter to compare.</param>
        /// <param name="right">Right parameter to compare.</param>
        /// <returns>True if the left argument is smaller than the right one, false otherwise.</returns>
        public static bool operator <(AtomicUInt16 left, ushort right)
        {
            return (ushort)left.fValue < right;
        }

        /// <summary>
        /// Checks whether the left argument is bigger than the right one.
        /// </summary>
        /// <param name="left">Left parameter to compare.</param>
        /// <param name="right">Right parameter to compare.</param>
        /// <returns>True if the left argument is bigger than the right one, false otherwise.</returns>
        public static bool operator >(AtomicUInt16 left, AtomicUInt16 right)
        {
            return (ushort)left.fValue > (ushort)right.fValue;
        }

        /// <summary>
        /// Checks whether the left argument is bigger than the right one.
        /// </summary>
        /// <param name="left">Left parameter to compare.</param>
        /// <param name="right">Right parameter to compare.</param>
        /// <returns>True if the left argument is bigger than the right one, false otherwise.</returns>
        public static bool operator >(AtomicUInt16 left, ushort right)
        {
            return (ushort)left.fValue > right;
        }

        /// <summary>
        /// Checks whether the left argument is smaller or equals than the right one.
        /// </summary>
        /// <param name="left">Left parameter to compare.</param>
        /// <param name="right">Right parameter to compare.</param>
        /// <returns>True if the left argument is smaller or equals than the right one, false otherwise.</returns>
        public static bool operator <=(AtomicUInt16 left, AtomicUInt16 right)
        {
            return (ushort)left.fValue <= (ushort)right.fValue;
        }

        /// <summary>
        /// Checks whether the left argument is smaller or equals than the right one.
        /// </summary>
        /// <param name="left">Left parameter to compare.</param>
        /// <param name="right">Right parameter to compare.</param>
        /// <returns>True if the left argument is smaller or equals than the right one, false otherwise.</returns>
        public static bool operator <=(AtomicUInt16 left, ushort right)
        {
            return (ushort)left.fValue <= right;
        }

        /// <summary>
        /// Checks whether the left argument is bigger or equals than the right one.
        /// </summary>
        /// <param name="left">Left parameter to compare.</param>
        /// <param name="right">Right parameter to compare.</param>
        /// <returns>True if the left argument is bigger or equals than the right one, false otherwise.</returns>
        public static bool operator >=(AtomicUInt16 left, AtomicUInt16 right)
        {
            return (ushort)left.fValue >= (ushort)right.fValue;
        }

        /// <summary>
        /// Checks whether the left argument is bigger or equals than the right one.
        /// </summary>
        /// <param name="left">Left parameter to compare.</param>
        /// <param name="right">Right parameter to compare.</param>
        /// <returns>True if the left argument is bigger or equals than the right one, false otherwise.</returns>
        public static bool operator >=(AtomicUInt16 left, ushort right)
        {
            return (ushort)left.fValue >= right;
        }

        /// <summary>
        /// Converts the atomic value to a non-atomic one.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static explicit operator ushort(AtomicUInt16 value)
        {
            return (ushort)value.fValue;
        }

        /// <summary>
        /// Converts the non-atomic value to an atomic one.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static implicit operator AtomicUInt16(ushort value)
        {
            return new AtomicUInt16(value);
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        public override string ToString()
        {
            return fValue.ToString();
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified value
        /// </summary>
        public override bool Equals(object obj)
        {
            AtomicUInt16 atom = (AtomicUInt16)obj;
            if (atom == default(AtomicUInt16))
                return false;

            return (ushort)atom.fValue == (ushort)fValue;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return fValue.GetHashCode();
        }

        /// <summary>
        /// Compares this instance to a specified value and returns an indication of their relative values.
        /// </summary>
        /// <param name="value">A value to compare.</param>
        /// <returns>A signed integer that indicates the relative order of this instance and value.Return
        /// Value Description Less than zero This instance is less than value. Zero This
        /// instance is equal to value. Greater than zero This instance is greater than value.
        /// </returns>
        public int CompareTo(AtomicUInt16 value)
        {
            return ((ushort)fValue).CompareTo((ushort)value.fValue);
        }

        /// <summary>
        /// Compares this instance to a specified value and returns an indication of their relative values.
        /// </summary>
        /// <param name="target">A value to compare.</param>
        /// <returns>A signed integer that indicates the relative order of this instance and value.Return
        /// Value Description Less than zero This instance is less than value. Zero This
        /// instance is equal to value. Greater than zero This instance is greater than value.
        /// </returns>
        public int CompareTo(object target)
        {
            AtomicUInt16 value = (AtomicUInt16)target;
            if (value == default(AtomicUInt16))
                return 1;
            else
                return CompareTo(value);
        }
    }
}