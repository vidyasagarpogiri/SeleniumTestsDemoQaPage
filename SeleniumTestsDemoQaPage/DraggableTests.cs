using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using SeleniumTestsDemoQaPage.Models;
using SeleniumTestsDemoQaPage.Pages.AutomationPracticePage;
using SeleniumTestsDemoQaPage.Pages.DraggablePage;
using SeleniumTestsDemoQaPage.Pages.DroppablePage;
using SeleniumTestsDemoQaPage.Pages.ResizablePage;
using SeleniumTestsDemoQaPage.Pages.ToolsQaHomePage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTestsDemoQaPage
{
    [TestFixture]
    public class DraggableTests
    {
        private IWebDriver driver;

        [SetUp]
        public void Init()
        {
            this.driver = new FirefoxDriver();
        }

        [TearDown]
        public void CleanUp()
        {
            // Add logger for failed tests
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                string filename = ConfigurationManager.AppSettings["Logs"] + TestContext.CurrentContext.Test.Name + ".txt";
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
                File.WriteAllText(filename,
                    "Test full name:\t" + TestContext.CurrentContext.Test.FullName + "\r\n\r\n"
                    + "Work directory:\t" + TestContext.CurrentContext.WorkDirectory + "\r\n\r\n"
                    + "Pass count:\t" + TestContext.CurrentContext.Result.PassCount + "\r\n\r\n"
                    + "Result:\t" + TestContext.CurrentContext.Result.Outcome.ToString() + "\r\n\r\n"
                    + "Message:\t" + TestContext.CurrentContext.Result.Message);

                var screenshot = ((ITakesScreenshot)this.driver).GetScreenshot();
                screenshot.SaveAsFile(ConfigurationManager.AppSettings["Logs"] + TestContext.CurrentContext.Test.Name + ".jpg", ScreenshotImageFormat.Jpeg);
            }

          //  driver.Quit(); // causes Firefox to crash
        }

        [Test]
        [Property("Draggable", 1), Property("Draggable test tab Number:", 1)] // Default functionality = tab no 1
        [Description("Default functionality: Drag a draggable element to the diagonally opposite corner of the containing window, check if element is dragged to the new location")]
        [Author("vankatabe")]
        public void DefaultFunctionality_DragToOppositeCorner_ElementMovedToOppositeCorner()
        {
            var draggablePage = new DraggablePage(this.driver);
            InteractionPages drag = AccessExcelData.GetInteractionTestsData(TestContext.CurrentContext.Test.Name); // Get the current test method name (TestContext.CurrentContext.Test.Name = DefaultFunctionality_DragToOppositeCorner_ElementMovedToOppositeCorner) and use it as a Key in the xlsx file
            // Get the tab number (e.g. "Default functionality", Constrain movement") from the test property above and give it to the URL
            draggablePage.tabNo = TestContext.CurrentContext.Test.Properties.Get("Draggable test tab Number:").ToString();
            draggablePage.NavigateTo(draggablePage.URL);

            draggablePage.DragObject(int.Parse(drag.HorizontalOffset), int.Parse(drag.VerticalOffset), draggablePage.DraggableElement);

            draggablePage.AssertElementIsMoved(int.Parse(drag.HorizontalOffset), int.Parse(drag.VerticalOffset), draggablePage.DraggableElement);
        }

        [Test]
        [Property("Draggable", 2), Property("Draggable test tab Number:", 3)] // Constrain movement = tab no 3
        [Description("Constrain movement: Drag diagonally a horizontally-only-draggable element, check if element position changed only horizontally")]
        [Author("vankatabe")]
        public void ConstrainMovementHorizontal_DragDiagonally_ElementMovedHorizontallyOnly()
        {
            var draggablePage = new DraggablePage(this.driver);
            InteractionPages drag = AccessExcelData.GetInteractionTestsData(TestContext.CurrentContext.Test.Name); // Get the current test method name (TestContext.CurrentContext.Test.Name = ConstrainMovementHorizontal_DragDiagonally_ElementMovedHorizontallyOnly) and use it as a Key in the xlsx file
            // Get the tab number (e.g. "Default functionality", Constrain movement") from the test property above and give it to the URL
            draggablePage.tabNo = TestContext.CurrentContext.Test.Properties.Get("Draggable test tab Number:").ToString();
            draggablePage.NavigateTo(draggablePage.URL);
            // Scroll page Up so the element is into view. Because when Firefox opens the desired page/tab, somehow the page is scrolled down
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", draggablePage.DraggableElementConstraint);

            draggablePage.DragObject(int.Parse(drag.HorizontalOffset), int.Parse(drag.VerticalOffset), draggablePage.DraggableElementConstraint);

            draggablePage.AssertElementIsMoved(int.Parse(drag.HorizontalOffset), 0, draggablePage.DraggableElementConstraint);
        }

        [Test]
        [Property("Interaction type:", 1), Property("Draggable tests number:", 2)]
        [Description("1 - Draggable: Drag a draggable element and drop it into its target, check if target status is dropped")]
        [Author("vankatabe")]
        public void DraggableElement_DragAndDropToTarget_TargetAttributeChangedToDropped9()
        {
            var droppablePage = new DroppablePage(this.driver);
            droppablePage.NavigateTo(droppablePage.URL);

            droppablePage.DragAndDrop();

            droppablePage.AssertElementIsDroppedAttribute("ui-widget-header ui-droppable ui-state-highlight");
        }

        [Test]
        [Property("Interaction", 3)]
        [Description("Exercise 3 from the lecture - Resize resizable item bith H and W with 100 pixels each")]
        [Author("vankatabe")]
        public void ResizableItem_ResizeSides100PixBigger_ItemSidesAre100PixBigger()
        {
            var resizablePage = new ResizablePage(this.driver);
            resizablePage.NavigateTo(resizablePage.URL);

            resizablePage.IncreaseWidthAndHeightBy(100);

            resizablePage.AssertSizeIncreasedWith(100);
        }

        [Test] // This test utilises both Data-driven tests and Log functonality (below)
        [Property("Interaction", 3)]
        [Description("Exercise 4 from the lecture - Add Logger to SoftUni Test")]
        [Author("vankatabe")]
        public void loginSoftUni_ValidCredentials_CorrectLogoDisplayedAfterLogin()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Url = "http://www.softuni.bg";
            IWebElement loginButton = driver.FindElement(By.XPath("//nav[@id='header-nav']/div[2]/ul/li[2]/span/a"));
            loginButton.Click();

            var softUniUser = AccessExcelData.GetTestData("Login");
            IWebElement userName = driver.FindElement(By.Name("username"));
            userName.Clear();
            userName.SendKeys(softUniUser.Username);
            IWebElement password = driver.FindElement(By.Name("password"));
            password.Clear();
            password.SendKeys(softUniUser.Password);

            IWebElement loginButton2 = driver.FindElement(By.XPath("//form/input[2]"));
            loginButton2.Click();
            IWebElement logo = driver.FindElement(By.XPath("//header[@id='page-header']/div/div/div/div/a/img"));
            Assert.IsTrue(logo.Displayed, "The logo is not displayed properly");
        }


    }
}