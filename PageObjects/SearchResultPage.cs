using System;
using OpenQA.Selenium;

namespace PartsUnlimited.WebDriverTests.PageObjects
{
    public class SearchResultPage : PageObject
    {
        public SearchResultPage(IWebDriver driver) : base(driver)
        {
        }

        public DetailPage SelectFirstSearchResult()
        {
            string selectedArticleTitle = "";
            return ArticleSelector.SelectArticleWithIndex(driver, 0, out selectedArticleTitle);
        }
    }
}