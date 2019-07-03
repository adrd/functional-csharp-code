using System;
using System.Collections.Generic;
using System.Linq;
using Boc.Domain;
using LaYumba.Functional;

namespace Exercises.Chapter1
{
   static class Exercises
   {
      // 1. Write a function that negates a given predicate: whenever the given predicate
      // evaluates to `true`, the resulting function evaluates to `false`, and vice versa.

      // callback function example
      private static IEnumerable<Boolean> NegatePredicate<T>(IEnumerable<T> source, Func<T, Boolean> predicate)
      {
          foreach (T item in source)
          {
              if (predicate(item))
                  yield return false;
              else
                  yield return true;
          }
      }

      // adaptor function example combined factory function example
      private static Func<T, Boolean> NegatePredicate<T>(this Func<T, Boolean> predicate)
      {
          return new Func<T, Boolean>(t =>
          {
              if (predicate(t)) 
                  return false;

              return true;
          });
      }

      // 2. Write a method that uses quicksort to sort a `List<int>` (return a new list,
      // rather than sorting it in place).

      public static List<Int32> NewList(List<Int32> initialList)
      {
          List<Int32> newList = null;

          

          return newList;
      }

      // 3. Generalize your implementation to take a `List<T>`, and additionally a 
      // `Comparison<T>` delegate.

      // 4. In this chapter, you've seen a `Using` function that takes an `IDisposable`
      // and a function of type `Func<TDisp, R>`. Write an overload of `Using` that
      // takes a `Func<IDisposable>` as first
      // parameter, instead of the `IDisposable`. (This can be used to fix warnings
      // given by some code analysis tools about instantiating an `IDisposable` and
      // not disposing it.)

      private static R Using<TDisp, R>(Func<TDisp> createDisposable, Func<TDisp, R> f) where TDisp : IDisposable
      {
          using (TDisp disposable = createDisposable())
          {
              return f(disposable);
          }
      }
   }
}
