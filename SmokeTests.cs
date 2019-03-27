using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PartsUnlimited.WebDriverTests.PageObjects;
using OpenQA.Selenium.Remote;
using System.IO;
using PartsUnlimited.WebDriverTests.TestApi;

namespace PartsUnlimited.WebDriverTests
{
    [TestClass]
    public class SmokeTests
    {

        public TestContext TestContext { get; set; }
        
        [TestMethod]
        public void AddTiresToCart()
        {
            using (var dw = WebDriverProvider.CreateDriverWrapper(TestContext))
            {
                new HomePage(dw.Driver)
                    .GoToCategoryPage(Categories.WheelsAndTires)
                    .SelectArticle(4)
                    .AddToCart();
            }
        }

        [TestMethod]
        public void Search()
        {
            using (var dw = WebDriverProvider.CreateDriverWrapper(TestContext))
            {
                new HomePage(dw.Driver)
                    .EnterSearchTerm("Turn Signal")
                    .SelectFirstSearchResult()
                    .AddToCart();
            }
        }
    }
}
