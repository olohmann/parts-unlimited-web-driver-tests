using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsUnlimited.WebDriverTests.TestApi
{
    public class WebDriverWrapper : IDisposable
    {

        private IWebDriver wrappedDriver;
        // this creates a dependency on the MS Test framework. It is only used for the screenshot attachment. When needed, replace that mechanism with the mechanism of your choice.
        private TestContext testContext;
        
        public WebDriverWrapper(IWebDriver wrappedDriver, TestContext testContext)
        {
            this.wrappedDriver = wrappedDriver;
            this.testContext = testContext;
        }

        public IWebDriver Driver { get { return this.wrappedDriver; } }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                }

                WebDriverProvider.DumpScreenshot(testContext, wrappedDriver);
                WebDriverProvider.DumpPageSource(testContext, wrappedDriver);
                WebDriverProvider.DumpLogs(testContext, wrappedDriver);
                // TODO: Let them pass themselves, then check for TestContext
                // Free unmanaged resources (unmanaged objects) and override a finalizer below.
                this.wrappedDriver?.Dispose();

                disposedValue = true;
            }
        }

        // Overriding a finalizer because Dispose(bool disposing) above has code to free unmanaged resources.
        ~WebDriverWrapper()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // Suppressing, because the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
