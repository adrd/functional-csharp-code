using System;
using NUnit.Framework;

namespace Exercises.Chapter2
{
   // 1. Write a console app that calculates a user's Body-Mass Index:
   //   - prompt the user for her height in metres and weight in kg
   //   - calculate the BMI as weight/height^2
   //   - output a message: underweight(bmi<18.5), overweight(bmi>=25) or healthy weight
   // 2. Structure your code so that structure it so that pure and impure parts are separate
   // 3. Unit test the pure parts
   // 4. Unit test the impure parts using the HOF-based approach

   public static class Bmi
   {
       public static void Start()
       {
           // read
           Double height = ReadPromptValue("Enter height in meters: ", Console.WriteLine, Console.ReadLine);
           Double weight = ReadPromptValue("Enter weight in kg: ", Console.WriteLine, Console.ReadLine);

           // compute bmi
           String bmiCategory = ComputeBmi(height, weight).ToBmiCategory();
           
           // write result
           OutputResult(bmiCategory, new Action<String>(Console.WriteLine));
       }

       // impure function
       public static Double ReadPromptValue(String message, Action<String> action, Func<String> func)
       {
           action(message);
           String value = func();

           return Double.Parse(value);
       }

       // pure function
       public static Double ComputeBmi(Double height, Double weight)
       {
           //Double bmi = Math.Round(weight / height * height, 2); // ???
           Double bmi = Math.Round(weight / Math.Pow(height, 2), 2);

           return bmi;
       }

       // pure function
       public static String ToBmiCategory(this Double bmi)
       {
           if (bmi < 18.5)
               return "underweight(bmi<18.5)";
           
           if (bmi >= 25)
               return "overweight(bmi>=25)";
           
           return "healthy weight";
       }

       // impure function
       private static void OutputResult(String outputMessage, Action<String> action)
       {
           action(outputMessage);
       }

       // first attempt
       private static (String height, String weight) ReadPromptValues()
       {
           Console.WriteLine("Enter height in meters: ");
           String height = Console.ReadLine();
           Console.WriteLine("Enter weight in kg: ");
           String weight = Console.ReadLine();

           return (height, weight);
       }
   }

   [TestFixture]
   public class BmiTests
   {
       [TestCase(1.80, 90, 27.78)]
       [TestCase(1.85, 81, 23.67)]
       [TestCase(1.21, 80, 54.64)]
       [TestCase(1.54, 60, 25.30)]
       [TestCase(1.90, 50, 13.85)]
       public void ComputeBmi_WhenHeighAndWeight_ReturnsBmi(Double height, Double weight, Double expectedBmi)
       {
           // Arrange
           //String height = "70";
           //String weight = "90";

           // Act
           Double actualBmi = Bmi.ComputeBmi(height, weight);

           // Assert
           Assert.AreEqual(expectedBmi, Math.Round(actualBmi, 2));
       }

       [TestCase(13.85, ExpectedResult = "underweight(bmi<18.5)")]
       [TestCase(18.5, ExpectedResult = "healthy weight")]
       [TestCase(23.67, ExpectedResult = "healthy weight")]
       [TestCase(25, ExpectedResult = "overweight(bmi>=25)")]
       [TestCase(27.78, ExpectedResult = "overweight(bmi>=25)")]
       public String ToBmiCategory_WhenBmi_ReturnsBmiMessage(Double bmi)
       {
           // Act
           return bmi.ToBmiCategory();


           // Old Style Unit Testing
           // Arrange
           //Double bmi = 15;

           // Act
           //String messageCategory = bmi.ToBmiCategory();

           // Asssert
           //Assert.AreEqual("underweight(bmi<18.5)", messageCategory);

       }

   }
}
