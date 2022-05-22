using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListApp.UnitTests.DataTtributes
{
    public class BaseInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        public BaseInlineAutoDataAttribute(params object[] arguments) : base(() => CreateFixture(), arguments)
        {
        }

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true, GenerateDelegates = true });

            return fixture;
        }
    }
}
