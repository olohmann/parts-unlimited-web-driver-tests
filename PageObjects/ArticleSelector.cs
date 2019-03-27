using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace PartsUnlimited.WebDriverTests.PageObjects
{
    public class ArticleSelector
    {
        public static DetailPage SelectArticleWithIndex(IWebDriver driver, int index, out string selectedArticleTitle)
        {
            var a = driver
                .FindElements(By.XPath($"//a[starts-with(@href,'/Store/Details/')][@title]"))
                .ElementAt(index);
            selectedArticleTitle = a.GetAttribute("title");
            a.Click();
            return new DetailPage(driver);
        }

        public static DetailPage SelectArticle(IWebDriver driver, string articleTitle)
        {
            driver
              .FindElements(By.XPath($"//a[@title='{articleTitle}']"))
              .Single()
              .Click();
            return new DetailPage(driver);
        }
    }
}
