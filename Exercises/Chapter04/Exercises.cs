using System;
using System.Collections.Generic;
using System.Linq;
using LaYumba.Functional;
using Double = System.Double;
using String = System.String;

namespace Exercises.Chapter4
{
    static class Exercises
    {
        // 1 Implement Map for ISet<T> and IDictionary<K, T>. (Tip: start by writing down
        // the signature in arrow notation.)

        // f : (ISet<T>, f : (T -> R)) -> ISet<R>
        private static ISet<R> Map<T, R>(this ISet<T> set, Func<T, R> func)
        {
            // Var 1
            //ISet<R> hashSet = new HashSet<R>();

            //foreach (T t in set)
            //{
            //    hashSet.Add(func(t));
            //}

            //return hashSet;

            // Var 2
            return set.Select(func).ToHashSet();
        }

        // f : (IDictionary<K, T>, f : (T -> R)) -> IDictionary<K, R>
        private static IDictionary<K, R> Map<K, T, R>(this IDictionary<K, T> dictionary, Func<T, R> func)
        {
            IDictionary<K, R> newDictionary = new Dictionary<K, R>();

            foreach (KeyValuePair<K, T> keyValuePair in dictionary)
            {
                newDictionary[keyValuePair.Key] = func(keyValuePair.Value);
            }

            return newDictionary;
        }


        // 2 Implement Map for Option and IEnumerable in terms of Bind and Return.

        // f : (Option<T>, f : (T -> R)) -> Option<R>
        // F.Some = Return; f : R -> Option<R>
        private static Option<R> Map<T, R>(this Option<T> option, Func<T, R> func)
        {
            return option.Bind(t => F.Some(func(t)));
        }

        // f : (IEnumerable<T>, f : (T -> R)) -> IEnumerable<R>
        // F.Some = Return; f : R -> Option<R> after that yields R
        private static IEnumerable<R> Map<T, R>(this IEnumerable<T> ts, Func<T, R> func)
        {
            return ts.Bind(item => F.Some(func(item)).AsEnumerable()); // see also solutions, implementation differs a little bit
        }


        // 3 Use Bind and an Option-returning Lookup function (such as the one we defined
        // in chapter 3) to implement GetWorkPermit, shown below. 

        // Then enrich the implementation so that `GetWorkPermit`
        // returns `None` if the work permit has expired.

        // f : string -> Option<Employee>
        // f : Option<Employee> -> Option<WorkPermit>
        static Option<WorkPermit> GetWorkPermit(Dictionary<string, Employee> people, string employeeId)
        {
            //Employee employee = people[employeeId];

            //foreach (KeyValuePair<String, Employee> person in people)
            //{
            //    if (person.Key == employeeId)
            //        return F.Some(person.Value)
            //            .Bind(employee => employee.WorkPermit);
            //}

            Employee value;
            if (people.TryGetValue(employeeId, out value))
                return F.Some(value)
                        .Bind(employee => employee.WorkPermit);

            return F.None;

            // see solutions file for complete implementation
        }

        // 4 Use Bind to implement AverageYearsWorkedAtTheCompany, shown below (only
        // employees who have left should be included).

        // f : (IEnumerable<Employee>) -> f : (Employee -> Option<Double>) -> IEnumerable<Double>
        // f : (IEnumerable<Double> -> Double
        static double AverageYearsWorkedAtTheCompany(List<Employee> employees)
        {
            return employees.Bind(employee => employee.LeftOn.Map(leftOn => YearsAtCompaby(employee.JoinedOn, leftOn)))
                            .Average();
        }

        private static Double YearsAtCompaby(DateTime employeeJoinedOn, DateTime leftOn)
        {
            return (leftOn - employeeJoinedOn).Days / 365d;
        }
    }

    public struct WorkPermit
    {
        public string Number { get; set; }
        public DateTime Expiry { get; set; }
    }

    public class Employee
    {
        public string Id { get; set; }
        public Option<WorkPermit> WorkPermit { get; set; }

        public DateTime JoinedOn { get; }
        public Option<DateTime> LeftOn { get; }
    }
}