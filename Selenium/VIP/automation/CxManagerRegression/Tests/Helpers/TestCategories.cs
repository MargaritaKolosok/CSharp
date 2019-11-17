using System;
using NUnit.Framework;

namespace Tests.Helpers
{
    /// Here are additional attributes for custom test runs by NUnit Console command line.
    /// For instance, to customize a set of running tests, just add corresponding attribute(s)
    /// to test method(s) and run NUnit Console app with "where" parameter: 
    /// <example>nunit3-console.exe Tests.dll --where "cat == Regression || cat == Smoke"</example>   
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RegressionAttribute : CategoryAttribute
    { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SmokeAttribute : CategoryAttribute
    { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ScheduledAttribute: CategoryAttribute
    { }
}
