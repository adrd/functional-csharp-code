using LaYumba.Functional;
using static LaYumba.Functional.F;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Examples.Chapter3;

namespace Exercises.Chapter5
{
   public static class Exercises
   {
      // 1. Without looking at any code or documentation (or intellisense), write the function signatures of
      // `OrderByDescending`, `Take` and `Average`, which we used to implement `AverageEarningsOfRichestQuartile`:
      static decimal AverageEarningsOfRichestQuartile(List<Person> population)
         => population
            .OrderByDescending(p => p.Earnings)
            .Take(population.Count/4)
            .Select(p => p.Earnings)
            .Average();

        // OrderByDescending : (IEnumerable<T>, f : (T -> TKey)) -> IOrderedEnumerable<T>
        // Take : (IEnumerable<T>, Int32) -> IEnumerable<T>
        // Average : (IEnumerable<T>) -> T

        // 2 Check your answer with the MSDN documentation: https://docs.microsoft.com/
        // en-us/dotnet/api/system.linq.enumerable. How is Average different?

        // 3 Implement a general purpose Compose function that takes two unary functions
        // and returns the composition of the two.

        // h(x) = (f * g)(x) = f(g(x))

        // f(t) : t -> t1
        // g(t1) : t1 -> t2
        // h(t) -> t2 = (f(g(t)) -> t2

        private static Func<T, T2> Compose<T, T1, T2>(this Func<T1, T2> g, Func<T, T1> f)
        {
            return new Func<T, T2>(t => g(f(t)));
        }
    }
}
