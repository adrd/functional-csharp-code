using LaYumba.Functional;
using NUnit.Framework;
using System;
using String = System.String;

namespace Exercises.Chapter7
{
    static class Exercises
    {
        // 1. Partial application with a binary arithmetic function:
        // Write a function `Remainder`, that calculates the remainder of 
        // integer division (and works for negative input values!). 

        // f : (T1, T2) -> R
        public static Int32 Remainder(Int32 a, Int32 b)
        {
            Int32 remainder = a % b;

            return remainder;
        }

        [TestCase(8, 5, ExpectedResult = 3)]
        [TestCase(-8, 5, ExpectedResult = -3)]
        public static Int32 TestRemainder(Int32 dividend, Int32 divisor)
            => Remainder(dividend, divisor);

        // Notice how the expected order of parameters is not the
        // one that is most likely to be required by partial application
        // (you are more likely to partially apply the divisor).

        // Write an `ApplyR` function, that gives the rightmost parameter to
        // a given binary function (try to write it without looking at the implementation for `Apply`).
        // Write the signature of `ApplyR` in arrow notation, both in curried and non-curried form

        // Note: see also solutions

        // ApplyR : (f : (T1, T2) -> R, T2) -> f : T1 -> R 
        static Func<T1, R> ApplyR_NonCurried<T1, T2, R>(this Func<T1, T2, R> func, T2 t2)
        {
            return new Func<T1, R>(t1 => func(t1, t2));
            //return t1 => func(t1, t2);
        }

        // ApplyR : (T1 -> T2 -> R) -> T2 -> T1 -> R
        static Func<T2, Func<T1, R>> ApplyR_Curried<T1, T2, R>(this Func<T1, T2, R> func)
        {
            return new Func<T2, Func<T1, R>>(t2 => new Func<T1, R>(t1 => func(t1, t2)));
            //return t2 => t1 => func(t1, t2);
        }

        // Use `ApplyR` to create a function that returns the
        // remainder of dividing any number by 5. 

        static Func<Int32, Int32, Int32> divideGeneral = Remainder;

        static Func<Int32, Int32> remainderWhenDividingBy5 = divideGeneral.ApplyR_NonCurried(5); // or ApplyR_NonCurried(divideGeneral, 5); // example of specific function

        // Write an overload of `ApplyR` that gives the rightmost argument to a ternary function (non-curried form is the solution, see also solutions)

        // f : (f : (T1, T2, T3) -> R, T3) -> f : (T1, T2) -> R
        static Func<T1, T2, R> ApplyR_NonCurried_Ternary<T1, T2, T3, R>(this Func<T1, T2, T3, R> func, T3 t3)
        {
            return new Func<T1, T2, R>((t1, t2) => func(t1, t2, t3));
        }

        // f : () -> T3 -> T1 -> T2 -> R
        static Func<T3, Func<T1, Func<T2, R>>> ApplyR_Curried_Ternary<T1, T2, T3, R>(this Func<T1, T2, T3, R> func)
        {
            return new Func<T3, Func<T1, Func<T2, R>>>(t3 => new Func<T1, Func<T2, R>>(t1 => new Func<T2, R>(t2 => func(t1, t2, t3))));
            //return t3 => t1 => t2 => func(t1, t2, t3);
        }

        // 2. Let's move on to ternary functions. Define a class `PhoneNumber` with 3
        // fields: number type(home, mobile, ...), country code('it', 'uk', ...), and number.
        // `CountryCode` should be a custom type with implicit conversion to and from string.

        public enum NumberType
        {
            Home,
            Mobile
        }

        public class PhoneNumber
        {
            private NumberType _numberType;
            private CountryCode _countryCode;
            private String _number;

            public PhoneNumber(NumberType numberType, CountryCode countryCode, String number)
            {
                this._numberType = numberType;
                this._countryCode = countryCode;
                this._number = number;
            }

            public NumberType NumberType => this._numberType;

            public CountryCode CountryCode => this._countryCode;

            public String Number
            {
                get { return this._number; }
            }
        }

        public class CountryCode
        {
            private String Value { get; }

            public CountryCode(String value)
            {
                this.Value = value;
            }

            public static implicit operator String(CountryCode countryCode) => countryCode.Value;

            public static implicit operator CountryCode(String s) => new CountryCode(s);

            public override String ToString()
            {
                return this.Value;
            }
        }

        // Now define a ternary function that creates a new number, given values for these fields.
        // What's the signature of your factory function? 

        // f : (NumberType, CountryCode, String) -> PhoneNumber
        static Func<CountryCode, NumberType, String, PhoneNumber> _createPhoneNumberTernary
         = new Func<CountryCode, NumberType, String, PhoneNumber>((countryCode, numberType, number) =>
                                                                                           new PhoneNumber(numberType, countryCode, number));

        // Use partial application to create a binary function that creates a UK number, 
        // and then again to create a unary function that creates a UK mobile

        // f : (NumberType, String) -> PhoneNumber
        private static Func<NumberType, String, PhoneNumber> _createUkPhoneNumberBinary
            = _createPhoneNumberTernary.Apply((CountryCode)"uk");

        // f : (String) -> PhoneNumber
        private static Func<String, PhoneNumber> _createUkMobilePhoneNumberUnary
            = _createUkPhoneNumberBinary.Apply(NumberType.Mobile);

        // 3. Functions everywhere. You may still have a feeling that objects are ultimately 
        // more powerful than functions. Surely, a logger object should expose methods 
        // for related operations such as Debug, Info, Error? 
        // To see that this is not necessarily so, challenge yourself to write 
        // a very simple logging mechanism without defining any classes or structs. 
        // You should still be able to inject a Log value into a consumer class/function, 
        // exposing operations like Debug, Info, and Error, like so:

        static void ConsumeLog(Log log)
           => log.Info("look! no objects!");

        enum Level { Debug, Info, Error }

        delegate void Log(Level level, String message);

        private static readonly Log _consoleLogger = new Log((level, message) => Console.WriteLine(level + message));

        private static void Debug(this Log log, String message)
            => log(Level.Debug, message);

        private static void Info(this Log log, String message)
            => log(Level.Info, message);

        private static void Error(this Log log, String message)
            => log(Level.Error, message);

        public static void _main() 
            => ConsumeLog(_consoleLogger);
    }
}
