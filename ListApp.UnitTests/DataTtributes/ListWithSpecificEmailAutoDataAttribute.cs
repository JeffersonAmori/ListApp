using AutoFixture;
using ListApp.UnitTests.Customizations;
using ListApp.UnitTests.DataTtributes.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListApp.UnitTests.DataTtributes
{
    internal class ListWithSpecificEmailAutoDataAttribute : BaseAutoDataAttribute
    {
        internal ListWithSpecificEmailAutoDataAttribute() : base(() => CreateFixture()) { }

        new private static IFixture CreateFixture()
        {
            var fixture = BaseAutoDataAttribute.CreateFixture();

            fixture.Customize(new ListWithUserEmailCustomization());

            return fixture;
        }
    }
}
