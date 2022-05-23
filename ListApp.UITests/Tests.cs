using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using ListApp.UITests.Helpers;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace ListApp.UITests
{
    [TestFixture(Platform.Android)]
    //[TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void CreateNewList_With_SomeItems_UsingTypeHere()
        {
            string newListName = "Brand new list";

            CommonTasks.CreateNewList(app, newListName, true);

            app.WaitForElement(newListName);
            app.Screenshot($"{newListName} created!");

            app.WaitForElement(newListName);
            app.Tap("Brand new list");

            app.WaitForElement("TypeHereEntry");

            List<AppResult[]> results = new List<AppResult[]>();

            for (int i = 1; i <= 3; i++)
            {
                app.Tap("TypeHereEntry");
                app.EnterText($"Item {i}");
                app.PressEnter();
                results.Add(app.WaitForElement($"Item {i}")); 
            }

            results.Should().AllSatisfy(x => x.Any());
        }

        [Test]
        public void CreateNewList_With_SomeItems_UsingEnterOnEachItem()
        {
            string newListName = "Brand new list";

            CommonTasks.CreateNewList(app, newListName, true);

            app.WaitForElement(newListName);
            app.Screenshot($"{newListName} created!");

            app.WaitForElement(newListName);
            app.Tap("Brand new list");

            app.WaitForElement("TypeHereEntry");
            app.Tap("TypeHereEntry");
            app.EnterText("Item 0");
            app.WaitForElement(x => x.Text("Item 0"));
            app.Tap(x => x.Text("Item 0"));
            app.PressEnter();
            app.WaitForElement(x => x.Text("Item 0"));

            List<AppResult[]> results = new List<AppResult[]>();

            for (int i = 1; i <= 3; i++)
            {
                app.Tap(x => x.Text($"Item {i - 1}"));
                app.PressEnter();
                app.WaitForElement(x => x.Text($"Item {i - 1}"));
                app.EnterText($"Item {i}");
                results.Add(app.WaitForElement($"Item {i}"));
            }

            app.PressEnter();
            app.Screenshot("Final result with items added");

            results.Should().AllSatisfy(x => x.Any());
        }
    }
}
