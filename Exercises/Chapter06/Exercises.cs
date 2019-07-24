using LaYumba.Functional;
using static LaYumba.Functional.F;
using System;
using System.Collections.Generic;
using Exercises.Chapter6.Solutions;

namespace Exercises.Chapter6
{
   static class Exercises
   {
      // 1. Write a `ToOption` extension method to convert an `Either` into an
      // `Option`. Then write a `ToEither` method to convert an `Option` into an
      // `Either`, with a suitable parameter that can be invoked to obtain the
      // appropriate `Left` value, if the `Option` is `None`. (Tip: start by writing
      // the function signatures in arrow notation)

      // ToOption : (Either<L, R>) -> Option<R>
      private static Option<R> ToOption<L, R>(this Either<L, R> either)
      {
          return either.Match(Left:  l => F.None,
                              Right: r => F.Some(r));
      }

      // ToEither : (Option<R>, f : () -> L) -> Either<L, R>
      private static Either<L, R> ToEither<L, R>(this Option<R> option, Func<L> funcL)
      {
          return option.Match<Either<L, R>>(None:() => funcL(),
                                            Some:r => r);
      }

      // 2. Take a workflow where 2 or more functions that return an `Option`
      // are chained using `Bind`.

      // Then change the first one of the functions to return an `Either`.

      // This should cause compilation to fail. Since `Either` can be
      // converted into an `Option` as we have done in the previous exercise,
      // write extension overloads for `Bind`, so that
      // functions returning `Either` and `Option` can be chained with `Bind`,
      // yielding an `Option`.

      class Candidate
      {
          
      }

      class Rejection
      {
          
      }

      static Func<Candidate, bool> IsEligible;
      static Func<Candidate, Either<Rejection, Candidate>> TechTest;
      static Func<Candidate, Either<Rejection, Candidate>> Interview;
      static Option<Candidate> Recruit(Candidate c)
          => Some(c)
              .Where(IsEligible)
              .Bind(TechTest)
              .Bind(Interview);

      private static Option<RR> Bind<L, R, RR>(this Either<L, R> either, Func<R, Option<RR>> func)
      {
          return either.Match(_ => F.None,
                              r => func(r));
      }

      private static Option<RR> Bind<L, R, RR>(this Option<R> option, Func<R, Either<L, RR>> func)
      {
          return option.Match(() => F.None,
                              r => func(r).ToOption());
      }

      // 3. Write a function `Safely` of type ((() → R), (Exception → L)) → Either<L, R> that will
      // run the given function in a `try/catch`, returning an appropriately
      // populated `Either`.

      // Safely : ((() -> R), (Exception -> L)) -> Either<L, R> ; see also solutions
      private static Either<L, R> Safely<R, L>(Func<R> funcR, Func<Exception, L> funcL)
      {
          try
          {
              R r = funcR();
              Either.Right<R> right = Right(r);

              return right;       // here conversion to Either<L, R> is done through the operator overload conversion in Either<L, R>
          }
          catch (Exception e)
          {
              L l = funcL(e);
              Either.Left<L> left = Left(l);
              
              return left;        // here conversion to Either<L, R> is done through the operator overload conversion in Either<L, R>
          }
      }

      // 4. Write a function `Try` of type (() → T) → Exceptional<T> that will
      // run the given function in a `try/catch`, returning an appropriately
      // populated `Exceptional`.

      // TryRun : (() -> T) -> Exceptional<T> ; see also solutions
      private static Exceptional<T> Try<T>(Func<T> func)
      {
          try
          {
              T t = func();

              Exceptional<T> exceptional = Exceptional<T>(t);

              return exceptional;
          }
          catch (Exception e)
          {
                Exceptional<T> exceptional = e;   // here conversion to Exceptional<T> is done through the implicit operator overload conversion in Exceptional<T>

                return exceptional;
            }
      }
   }
}
