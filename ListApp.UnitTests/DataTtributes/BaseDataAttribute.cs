using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using System;

namespace ListApp.UnitTests.DataTtributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class BaseDataAttribute : AutoDataAttribute
    {
        public BaseDataAttribute() : base(() => CreateFixture()) { }

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true, GenerateDelegates = true });

            return fixture;
        }
    }
}
