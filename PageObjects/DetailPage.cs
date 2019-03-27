using OpenQA.Selenium;

namespace PartsUnlimited.WebDriverTests.PageObjects
{
    public class DetailPage : PageObject
    {
        public DetailPage(IWebDriver driver) : base(driver)
        {
        }

        public CartPage AddToCart()
        {
            driver
                .FindElement(By.XPath("//a[starts-with(@href,'/ShoppingCart/AddToCart/')]"))
                .Click();
            return new CartPage(driver);
        }
    }
}