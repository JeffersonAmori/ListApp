using AutoFixture;
using ApiModel = ListApp.Api.Models;

namespace ListApp.UnitTests.Customizations
{
    internal class ListWithUserEmailCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Build<ApiModel.List>()
                .With(x => x.OwnerEmail, "user@server.com");
        }
    }
}
