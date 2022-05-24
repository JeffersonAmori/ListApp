using FluentAssertions;
using FluentAssertions.Equivalency;
using ListApp.Models.Extensions;
using ListApp.UnitTests.Base;
using ListApp.UnitTests.DataTtributes.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using ApiModel = ListApp.Api.Models;
using LocalModel = ListApp.Models;

namespace ListApp.UnitTests
{
    [TestFixture]
    public class ModelExtensionsTests : BaseTest
    {
        [Test, BaseAutoData]
        public void MethodToLocalModel_Should_ReturnEquivalentLocalList(ApiModel.List cloudList)
        {
            // Arrange
            Func<EquivalencyAssertionOptions<ApiModel.List>, EquivalencyAssertionOptions<ApiModel.List>> listEquivalencyAssertionOptions = (config) =>
                config
                    .WithMapping<LocalModel.List>(c => c.Guid, s => s.ListId)
                    .ExcludingMissingMembers()
                    .Excluding(list => list.LastChangedDate)
                    .Excluding(list => list.ListItems)
                    .For(list => list.ListItems)
                        .Exclude(listItem => listItem.ListId)
                    .For(list => list.ListItems)
                        .Exclude(listItem => listItem.LastChangedDate)
                    .For(list => list.ListItems)
                        .Exclude(listItem => listItem.ListId);

            // Act
            LocalModel.List localList = cloudList.ToLocalModel();

            // Assert
            localList.Should().BeEquivalentTo(cloudList, listEquivalencyAssertionOptions);
        }

        [Test, BaseAutoData]
        public void MethodToLocalModel_Should_ReturnEquivalentLocalListItem(ApiModel.ListItem cloudListItem)
        {
            // Arrange
            Func<EquivalencyAssertionOptions<ApiModel.ListItem>, EquivalencyAssertionOptions<ApiModel.ListItem>> listItemEquivalencyAssertionOptions = (config) =>
                config
                    .ExcludingMissingMembers()
                    .Excluding(listItem => listItem.Id)
                    .Excluding(listItem => listItem.ListId)
                    .Excluding(listItem => listItem.LastChangedDate);

            // Act
            LocalModel.ListItem localListItem = cloudListItem.ToLocalModel();

            // Assert
            localListItem.Should().BeEquivalentTo(cloudListItem, listItemEquivalencyAssertionOptions);
        }

        [Test, BaseAutoData]
        public void MethodToApilModel_Should_ReturnEquivalentApiList(LocalModel.List localList)
        {
            // Arrange
            Func<EquivalencyAssertionOptions<LocalModel.List>, EquivalencyAssertionOptions<LocalModel.List>> listEquivalencyAssertionOptions = (config) =>
                config
                    .WithMapping<ApiModel.List>(c => c.ListId, s => s.Guid)
                    .ExcludingMissingMembers()
                    .Excluding(list => list.LastChangedDate)
                    .Excluding(list => list.ListItems)
                    .For(list => list.ListItems)
                        .Exclude(listItem => listItem.ListId)
                    .For(list => list.ListItems)
                        .Exclude(listItem => listItem.LastChangedDate);

            // Act
            ApiModel.List cloudList = localList.ToApiModel();

            // Assert
            cloudList.Should().BeEquivalentTo(localList, listEquivalencyAssertionOptions);
        }

        [Test, BaseAutoData]
        public void MethodToApiModel_Should_ReturnEquivalentApiListItem(LocalModel.ListItem localListItem, long listId)
        {
            // Arrange
            Func<EquivalencyAssertionOptions<LocalModel.ListItem>, EquivalencyAssertionOptions<LocalModel.ListItem>> listItemEquivalencyAssertionOptions = (config) =>
                config
                    .ExcludingMissingMembers()
                    .Excluding(listItem => listItem.Id)
                    .Excluding(listItem => listItem.ListId)
                    .Excluding(listItem => listItem.LastChangedDate);

            // Act
            ApiModel.ListItem apiListItem = localListItem.ToApiModel(listId);

            // Assert
            apiListItem.Should().BeEquivalentTo(localListItem, listItemEquivalencyAssertionOptions);
        }

        [Test, BaseAutoData, Ignore("Could not implement ContainEquivalentOf yet.")]
        public void MethodCopyFrom_Should_ReturnObjectWithCopiedValues(ApiModel.List cloudList, LocalModel.List localList)
        {
            //// Arrange
            //Func<EquivalencyAssertionOptions<LocalModel.List>, EquivalencyAssertionOptions<LocalModel.List>> listEquivalencyAssertionOptions = (config) =>
            //    config
            //        .ExcludingMissingMembers()
            //        .Excluding(list => list.CreationDate)
            //        .Excluding(list => list.LastChangedDate)
            //        .Excluding(list => list.ListItems);

            //Func<EquivalencyAssertionOptions<List<LocalModel.ListItem>>, EquivalencyAssertionOptions<List<LocalModel.ListItem>>> listItemEquivalencyAssertionOptions =
            //    (config) =>
            //        config
            //            .Excluding(listItem => listItem.Path.Contains("Id"))
            //            .Excluding(listItem => listItem.Path.Contains("Guid"))
            //            .Excluding(listItem => listItem.Path.Contains("ListId"))
            //            .Excluding(listItem => listItem.Path.Contains("LastChangedDate"))
            //            .WithTracing();
            ////Func<EquivalencyAssertionOptions<IEnumerable<ApiModel.ListItem>>, EquivalencyAssertionOptions<IEnumerable<ApiModel.ListItem>>> listItemEquivalencyAssertionOptions = (config) =>
            ////    config
            ////        .WithMapping<LocalModel.ListItem>(t => t.Guid, s => s.Id)
            ////        .ExcludingMissingMembers()
            ////        .Excluding(list => list.CreationDate)
            ////        .Excluding(list => list.LastChangedDate)

            //// Act
            //cloudList.CopyFrom(localList);

            //// Assert
            //cloudList.Should().BeEquivalentTo(localList, listEquivalencyAssertionOptions);
            //cloudList.ListItems.Should().ContainEquivalentOf(localList.ListItems, listItemEquivalencyAssertionOptions);
        }
    }
}
