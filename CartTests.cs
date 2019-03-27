using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartsUnlimited.WebDriverTests.PageObjects;
using PartsUnlimited.WebDriverTests.TestApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsUnlimited.WebDriverTests
{
    [TestClass]
    public class CartTests
    {

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void ItemTitleAppearsInCartViewAfterBeingAdded()
        {
            // This kind of functional test usually should not rely on
            // anything and instead make actively sure that any prerequisites are
            // met. This includes changing the database in the arrange-part of
            // the test, e.g. to insert the article that should be selected afterwards.
            // Yet in this case, we want to be more flexible: The test should run even
            // if we do not have the chance to change the database to meet our needs.
            // Thus we do things like SelectFirstArticle() and remember the title, which
            // will always work as long there is an article in that category... 
            string selectedArticleTitle;
            using (var dw = WebDriverProvider.CreateDriverWrapper(TestContext))
            {
                new HomePage(dw.Driver)
                    .GoToCategoryPage(Categories.Brakes)
                    .SelectFirstArticle(out selectedArticleTitle)
                    .AddToCart();
                var titlesInCart = new CartPage(dw.Driver).ArticleTitles;
                CollectionAssert.Contains(titlesInCart, selectedArticleTitle, $"Article '{selectedArticleTitle}' not in cart as expected.");
            }

        }

        [TestMethod]
        public void ItemTitleDisappearsFromCartViewAfterBeingRemoved()
        {
            string articleToRemoveTitle;
            using (var dw = WebDriverProvider.CreateDriverWrapper(TestContext))
            {
                new HomePage(dw.Driver)
                    .GoToCategoryPage(Categories.Brakes)
                    .SelectFirstArticle()
                    .AddToCart()
                    .GoToCategoryPage(Categories.Lighting)
                    .SelectFirstArticle(out articleToRemoveTitle)
                    .AddToCart()
                    .GoToCategoryPage(Categories.WheelsAndTires)
                    .SelectFirstArticle()
                    .AddToCart()
                    .RemoveArticleWithTitle(articleToRemoveTitle);
                var titlesInCart = new CartPage(dw.Driver).ArticleTitles;
                var titlesFlat = String.Join(',', titlesInCart.Cast<string>().ToArray());
                CollectionAssert
                    .DoesNotContain(
                        titlesInCart,
                        articleToRemoveTitle,
                        $"Title '{articleToRemoveTitle}' was not removed from cart as expected. Titles in cart are: {titlesFlat}.");
                Assert.AreEqual(2, titlesInCart.Count, $"Unexpected number of items in cart. Title to remove was '{articleToRemoveTitle}'. Titles in cart are: {titlesFlat}.");
            }
        }
    }
}
