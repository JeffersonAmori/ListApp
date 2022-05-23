using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.UITest;

namespace ListApp.UITests.Helpers
{
    internal class CommonTasks
    {
        private const string textInputInsidePopup = "custom";

        /// <summary>
        /// Create a new list. Be sure to be on the List Page before calling this method.
        /// </summary>
        /// <param name="app">The IApp currently in exceution.</param>
        /// <param name="listName">The name of the list to be created.</param>
        /// <param name="takeScreenshots">Should this method take screenshots?</param>
        internal static void CreateNewList(IApp app, string listName, bool takeScreenshots = false)
        {
            app.WaitForElement(x => x.Text("+"));
            app.Tap("NewListFloatActionButton");

            app.WaitForElement(textInputInsidePopup);
            app.Tap("custom");

            if (takeScreenshots)
                app.Screenshot("Enter new list name...");

            app.EnterText(textInputInsidePopup, listName);

            app.Tap(x => x.Text("OK"));
        }
    }
}
