using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace PartsUnlimited.WebDriverTests.PageObjects
{
    public class CategoryPage : PageObject
    {
        public CategoryPage(IWebDriver driver) : base(driver)
        {
        }

        public DetailPage SelectArticle(int articleId)
        {
            driver.FindElement(By.XPath($"//a[@href='/Store/Details/{articleId}']")).Click();
            return new DetailPage(driver);
        }

        public DetailPage SelectFirstArticle()
        {
            var dummy = "";
            return SelectFirstArticle(out dummy);
        }

        public DetailPage SelectFirstArticle(out string selectedArticleTitle)
        {
            return ArticleSelector.SelectArticleWithIndex(driver, 0, out selectedArticleTitle);
        }

        public DetailPage SelectArticleWithIndex(int i)
        {
            string selectedArticleTitle = "";
            return ArticleSelector.SelectArticleWithIndex(driver, i, out selectedArticleTitle);
        }

        public DetailPage SelectArticle(string articleTitle)
        {
            return ArticleSelector.SelectArticle(driver, articleTitle);
        }
    }
}
