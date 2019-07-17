using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
//using System.Configuration;
using LaYumba.Functional;
using LaYumba.Functional.Option;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.OData;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using String = System.String;

namespace Exercises.Chapter3
{
    public static class Exercises
    {
        // 1 Write a generic function that takes a string and parses it as a value of an enum. It
        // should be usable as follows:

        // Enum.Parse<DayOfWeek>("Friday") // => Some(DayOfWeek.Friday)
        // Enum.Parse<DayOfWeek>("Freeday") // => None

        // f : String -> Option<TEnum> = None | Some(TEnum)

        public static class Enum
        {
            public static Option<TEnum> Parse<TEnum>(String dayOfWeek) where TEnum : struct
            {
                //String name = System.Enum.GetName(typeof(TEnum), dayOfWeek);
                // does boxing
                Object result;
                Boolean parseSucceded = System.Enum.TryParse(typeof(TEnum), dayOfWeek, out result);

                if (parseSucceded)
                    return F.Some((TEnum)result);

                return F.None;
            }
        }

        // 2 Write a Lookup function that will take an IEnumerable and a predicate, and
        // return the first element in the IEnumerable that matches the predicate, or None
        // if no matching element is found. Write its signature in arrow notation:

        // bool isOdd(int i) => i % 2 == 1;
        // new List<int>().Lookup(isOdd) // => None
        // new List<int> { 1 }.Lookup(isOdd) // => Some(1)

        // f : (IEnumerable<Int32>, f : (Int32 -> Boolean)) -> Option<Int32> = Some(Int32) | None

        public static Option<T> MyLookup<T>(this IEnumerable<T> source, Func<T, Boolean> predicate)
        {
            foreach (T item in source)
            {
                if (predicate(item))
                    return F.Some(item);
            }

            return F.None;
        }
        
        // 3 Write a type Email that wraps an underlying string, enforcing that it’s in a valid
        // format. Ensure that you include the following:
        // - A smart constructor
        // - Implicit conversion to string, so that it can easily be used with the typical API
        // for sending emails

        public class Email
        {
            // f : String -> Email
            private Email(String email)
            {
                if (!IsValid(email))
                    throw new ArgumentException($"{email} is not a valid email");

                this.Value = email;
            }

            private String Value { get; }

            // example of smart constructor
            // f : String -> Option<Email> = Some(Email) | None
            public Option<Email> CreateEmail(String email)
            {
                return Email.IsValid(email) ? F.Some(new Email(email)) : F.None;
            }

            // f : String -> Boolean
            private static Boolean IsValid(String email)
            {
                // need more elaborate validation
                if (String.IsNullOrEmpty(email) || String.IsNullOrWhiteSpace(email))
                    return false;

                return true;
            }

            // f : Email -> String (specific to general)
            public static implicit operator String(Email email)
            {
                return email.ToString();
            }

            // f : String -> Email (general to specific)
            public static explicit operator Email(String strEmail)
            {
                return new Email(strEmail);
            }

            // f : () -> String
            public override String ToString() => this.Value;
        }

        // 4 Take a look at the extension methods defined on IEnumerable in System.LINQ.Enumerable.
        // Which ones could potentially return nothing, or throw some
        // kind of not-found exception, and would therefore be good candidates for
        // returning an Option<T> instead?

        // Cast
        // ElementAtOrDefault
        // First and FirstOrDefault
        // Last and LastOrDefault
        // Single and SingleOrDefault
    }

    // 5.  Write implementations for the methods in the `AppConfig` class
    // below. (For both methods, a reasonable one-line method body is possible.
    // Assume settings are of type string, numeric or date.) Can this
    // implementation help you to test code that relies on settings in a
    // `.config` file?
    public class AppConfig
    {
        NameValueCollection source; // key - value collection

        //public AppConfig() : this(ConfigurationManager.AppSettings) { }

        public AppConfig(NameValueCollection source)
        {
            this.source = source;
        }

        // this method verifies only the key
        public Option<T> Get<T>(String name)
        {
            String value = this.source[name];       // if name is not found value is null

            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                return F.None;

            return F.Some<T>((T)Convert.ChangeType(value, typeof(T)));
        }

        // example of pattern matching
        // this method verifies the value of the key searched
        public T Get<T>(String name, T defaultValue)
        {
            return this.Get<T>(name)
                       .Match(None:() => defaultValue, 
                              Some: (value) => value);
        }
    }

    [TestFixture]
    public class EnumTests
    {
        [Test]
        public void Parse_WhenValidDayOfWeekString_ReturnsEnumDayOfWeek()
        {
            // Arrange
            String dayOfWeek = "Friday";

            // Act
            Option<DayOfWeek> actualResult = Exercises.Enum.Parse<DayOfWeek>(dayOfWeek);

            // Assert
            Assert.AreEqual(F.Some<DayOfWeek>(DayOfWeek.Friday), actualResult);
        }

        [Test]
        public void Parse_WhenInvalidDayOfWeekString_ReturnsNone()
        {
            // Arrange
            String dayOfWeek = "Freeday";

            // Act
            Option<DayOfWeek> actualResult = Exercises.Enum.Parse<DayOfWeek>(dayOfWeek);

            // Assert
            Assert.AreEqual(F.None, actualResult);
        }
    }

    [TestFixture]
    public class LookupTests
    {
        [Test]
        public void Lookup_WhenListContainsOddNumber_ReturnFirstOddNumber()
        {
            // Arrange
            List<Int32> list = new List<Int32>(1) { 1 };
            Boolean IsOdd(Int32 i) => i % 2 == 1;

            // Act
            Option<Int32> actualResult = list.MyLookup(IsOdd);

            // Assert
            Assert.AreEqual(F.Some(1), actualResult);
        }

        [Test]
        public void MyLookup_WhenListIsEmpty_ReturnsNone()
        {
            // Arrange
            List<Int32> list = new List<Int32>() { };
            Boolean IsOdd(Int32 i) => i % 2 == 1;

            // Act
            Option<Int32> actualResult = list.MyLookup(IsOdd);

            // Assert
            Assert.AreEqual(F.None, actualResult);
        }
    }
}
