using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsUnlimited.WebDriverTests.PageObjects
{
    public class PageObject
    {
        protected IWebDriver driver;

        public PageObject(IWebDriver driver)
        {
            this.driver = driver;
        }

        public CategoryPage GoToCategoryPage(string category)
        {
            driver.FindElement(By.LinkText(category)).Click();
            return new CategoryPage(driver);
        }

        public CartPage GoToCartPage()
        {
            driver
                .FindElement(By.XPath("//a[@id='shopping-cart-link']/div[text()='Cart']"))
                .Click();
            return new CartPage(driver);
        }

        public HomePage GoToHomePage()
        {
            driver
                .FindElement(By.Id("home-link"))
                .Click();
            return new HomePage(driver);
        }

        public SearchResultPage EnterSearchTerm(string searchTerm)
        {
            IWebElement searchTermField = driver.FindElement(By.Id("search-box"));
            searchTermField.Click();
            searchTermField.SendKeys(searchTerm);
            driver.FindElement(By.Id("search-link")).Click();
            return new SearchResultPage(driver);
        }
    }
}
