using System.Collections;
using OpenQA.Selenium;
using System.Linq;
using System;
using System.Threading;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace PartsUnlimited.WebDriverTests.PageObjects
{
    public class CartPage : PageObject
    {
        public CartPage(IWebDriver driver) : base(driver)
        {
        }

        public ICollection ArticleTitles
        {
            get
            {
                return driver
                    .FindElements(By.XPath($"//div[@id='cart-summary']//a[starts-with(@href,'/Store/Details/')]"))
                    .Select(l => l.Text)
                    .ToList();
            }
        }

        public CartPage RemoveArticleWithTitle(string articleTitle)
        {
            // To develop XPath like the one below, use Chrome.
            //
            // Getting XPath expressions:
            // - In Chrome hit F12
            // - Locate the element in the DOM explorer
            // - Right click the element and choose Copy>Copy XPath
            //
            // Testing XPath expressions:
            // - In Crome hit F12
            // - Open the Console
            // - type "$x("your-xpath-expression")
            // - check whether that results in the desired element(s)

            var a = driver
                .FindElement(By.XPath($"//*[div/div[5]/div[1]/strong/a/text()='{articleTitle}']/div/div/a[contains(text(),'Remove')]"));
            var id = a.GetAttribute("data-id");
            a.Click();

            // The removing of the item is being performed by a script in the 
            // web page. As this runs asynchronously, we need to explicitly wait
            // for it to finish. The best indicator for this is the result message
            // that is displayed at the end of the script.
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until((d) => d.FindElement(By.XPath($"//div[@id='update-message' and contains(text(),'{articleTitle} has been removed')]")));

            return base.GoToCartPage(); // This is a workaround: The app currently does not show the cart correctly, thus we need to refresh it by actively navigating to it.
        }

        public CartPage ClearCart()
        {
            driver.FindElement(By.LinkText("Clear")).Click();
            return base.GoToCartPage(); // This is a workaround: The app currently does not show the cart correctly, thus we need to refresh it by actively navigating to it.
        }
    }
}