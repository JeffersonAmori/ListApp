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

        [Test]
        public void ThemesPage_ShowsThemes()
        {
            // Navigate to the themes page.
            app.WaitForElement(x => x.Text("List Freak"));
            app.TapCoordinates(30, 100);
            app.WaitForElement(x => x.Text("Settings"));
            app.Tap(x => x.Text("Settings"));
            app.WaitForElement(x => x.Text("Themes"));
            app.Tap(x => x.Text("Themes"));

            // Find all themes on screen.
            List<AppResult[]> results = new List<AppResult[]>();
            results.Add(app.WaitForElement(x => x.Text("Forest")));
            results.Add(app.WaitForElement(x => x.Text("River")));
            results.Add(app.WaitForElement(x => x.Text("Bee")));
            results.Add(app.WaitForElement(x => x.Text("Quartzo")));
            results.Add(app.WaitForElement(x => x.Text("Night")));
            results.Add(app.WaitForElement(x => x.Text("Inferno")));
            results.Add(app.WaitForElement(x => x.Text("London")));

            // Check if the themes are displayed correctly.
            results.Should().AllSatisfy(x => x.Any());
        }

        [Test]
        public void LanguagesPage_ShowsLanguages()
        {
            // Navigate to the themes page.
            app.WaitForElement(x => x.Text("List Freak"));
            app.TapCoordinates(30, 100);
            app.WaitForElement(x => x.Text("Settings"));
            app.Tap(x => x.Text("Settings"));
            app.WaitForElement(x => x.Text("Language"));
            app.Tap(x => x.Text("Language"));

            // Find all themes on screen.
            List<AppResult[]> results = new List<AppResult[]>();
            results.Add(app.WaitForElement(x => x.Text("English")));
            results.Add(app.WaitForElement(x => x.Text("Português")));

            // Check if the themes are displayed correctly.
            results.Should().AllSatisfy(x => x.Any());
        }
    }
}
