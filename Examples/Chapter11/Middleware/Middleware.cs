using System;
using System.Collections.Generic;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using NUnit.Framework;
using Unit = System.ValueTuple;

namespace Playground.WithLINQ.DbLogger
{
   // composition of 3 functions 
   // start function : T
   // transformation function: T
   // continuation function: R
   public delegate dynamic Middleware<T>(Func<T, dynamic> cont);

   public static class Middleware
   {
      // from elevated type to regular type (extract T from Middleware<T>)
      public static T Run<T>(this Middleware<T> mw) => mw(t => t);
      //public static T Run<T>(this Middleware<T> mw) => mw.Invoke(t => { return t; });

      public static Middleware<R> Map<T, R>
         (this Middleware<T> mw, Func<T, R> f)
         => Select(mw, f);

      public static Middleware<R> Bind<T, R>
         (this Middleware<T> mw, Func<T, Middleware<R>> f)
         => SelectMany(mw, f);

      //public static Middleware<R> Select<T, R>
      //   (this Middleware<T> mw, Func<T, R> f)
      //   => cont => mw(t => cont(f(t)));

      public static Middleware<R> Select<T, R>
          (this Middleware<T> mw, Func<T, R> f)
      {
          return new Middleware<R>(cont =>
          {
              return mw.Invoke(new Func<T, dynamic>(t => cont.Invoke(f.Invoke(t))));
          });   
      }

        // cand definesc o variabila un delegate Middleware<T> = definesc o metoda care primeste ca parametru un Func<T, dynamic> delegate si corpul executiei ei
        // cand invoc un delegate Middleware<T> = definesc functia care primeste un T si intoarce un dynamic, altfel spus specific parametrii/argumetele lui Middleware<T>,
        // altfel spus instantiez delegat-ul Func<T, dynamic>

        //public static Middleware<R> SelectMany<T, R>
        //   (this Middleware<T> mw, Func<T, Middleware<R>> f)
        //   => cont => mw(t => f(t)(cont));

        public static Middleware<R> SelectMany<T, R>
            (this Middleware<T> mw, Func<T, Middleware<R>> f)
        {
            return new Middleware<R>(cont => { return (dynamic)mw.Invoke(new Func<T, dynamic>(t => f.Invoke(t).Invoke(cont))); });
        }

        //public static Middleware<RR> SelectMany<T, R, RR>
        //   (this Middleware<T> @this, Func<T, Middleware<R>> f, Func<T, R, RR> project)
        //   => cont => @this(t => f(t)(r => cont(project(t, r))));

        public static Middleware<RR> SelectMany<T, R, RR>
          (this Middleware<T> @this, Func<T, Middleware<R>> f, Func<T, R, RR> project)
      {
          return new Middleware<RR>(cont =>
          {
              return (dynamic)@this.Invoke(t => { return (dynamic)f.Invoke(t).Invoke(r => cont.Invoke(project.Invoke(t, r))); });
          });
      }
      
   }

   public class MiddlewreTests
   {
      private List<string> sideEffects;

      Middleware<Unit> MwA => handler =>
      {
         sideEffects.Add("Entering A");
         var result = handler(Unit());
         sideEffects.Add("Exiting A");
         return result;
      };

      Middleware<string> MwB => handler =>
      {
         sideEffects.Add("Entering B");
         var result = handler("b");
         sideEffects.Add("Exiting B");
         return result;
      };

      [SetUp] public void SetUp() 
         => sideEffects = new List<string>();

      [Test] public void TestPipelineWithOneClause()
      {
         var pipeline = from b in MwB
                        select b + "end";

         Assert.AreEqual("bend", pipeline.Run());
         Assert.AreEqual(new List<string> { "Entering B", "Exiting B" }, sideEffects);
      }

      [Test] public void TestPipelineWith2Clauses()
      {
         var pipeline = from _ in MwA
                        from b in MwB
                        select b + "end";

         Assert.AreEqual("bend", pipeline.Run());
         Assert.AreEqual(new List<string> { "Entering A", "Entering B", "Exiting B", "Exiting A" }, sideEffects);
      }

      [Test] public void TestPipelineWith3Clauses()
      {
         var pipeline = from _ in MwA
                        from b in MwB
                        from c in MwB
                        select b + c + "end";

         Assert.AreEqual("bbend", pipeline.Run());
         Assert.AreEqual(new List<string> { "Entering A", "Entering B", "Entering B", "Exiting B", "Exiting B", "Exiting A" }, sideEffects);
      }

      [Test] public void TestPipelineWith4Clauses()
      {
         var pipeline = from _ in MwA
                        from __ in MwA
                        from b in MwB
                        from c in MwB
                        select b + c + "end";

         Assert.AreEqual("bbend", pipeline.Run());
      }
   }
}
