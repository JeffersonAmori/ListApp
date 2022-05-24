using NUnit.Framework;
using System.Text.Json;

namespace ListApp.UnitTests.Base
{
    [TestFixture]
    public class BaseTest
    {
        [OneTimeSetUp]
        public virtual void BaseSetup()
        {
            ((JsonSerializerOptions)typeof(JsonSerializerOptions)
               .GetField("s_defaultOptions",
                   System.Reflection.BindingFlags.Static |
                   System.Reflection.BindingFlags.NonPublic).GetValue(null))
               .PropertyNameCaseInsensitive = true;
        }
    }
}
