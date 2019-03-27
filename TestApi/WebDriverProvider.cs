﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PartsUnlimited.WebDriverTests.TestApi
{
    public class WebDriverProvider
    {

        // This class provides WebDriver instances and makes the most important things
        // parameterized in an easy way. The parameter values can be set in a .runsettings
        // file and overridden in a build task. See:
        // https://msdn.microsoft.com/en-us/library/jj635153.aspx
        // https://blogs.msdn.microsoft.com/devops/2015/09/04/supplying-run-time-parameters-to-tests/
        // The settings can be set as well by using environment variables.    
        // To set the env variables on your dev machine use PowerShell like this:
        // [Environment]::SetEnvironmentVariable("Environment_TestScreenSize","512x1080","User")
        // [Environment]::SetEnvironmentVariable("Environment_TestWebDriverName","Chrome","User")
        // [Environment]::SetEnvironmentVariable("Environment_TestTargetUrl","http://mysite.azurewebsites.net/","User")

        // The setting priority is:
        // 1. RunSettings in build task override...
        // 2. RunSettings in .runSettings file override...
        // 3. Process environment variables override...
        // 4. User environment variables override...
        // 5. Machine environment variables.

        public static WebDriverWrapper CreateDriverWrapper(TestContext testContext)
        {
            var driverName = GetDriverName(testContext);
            IWebDriver driver = GetDriverInstance(testContext, driverName);
            driver.Navigate().GoToUrl(GetTargetUrl(testContext));
            driver.Manage().Window.Size = GetDriverSize(testContext);
            DumpDriverInfo(driver);
            return new WebDriverWrapper(driver, testContext);
        }

        public static void DumpLogs(TestContext testContext, IWebDriver driver)
        {
            foreach (var logType in driver.Manage().Logs.AvailableLogTypes)
            {
                var log =
                    driver.Manage().Logs.GetLog(logType)
                    .Select(le => le.Message)
                    .ToArray();
                var path = Path.Combine(Path.GetTempPath(), testContext.TestName + $".Logs.{logType}.log");
                File.WriteAllLines(path, log);
                // https://github.com/Microsoft/testfx/issues/394
                // testContext.AddResultFile(path);
            }
        }

        public static void DumpPageSource(TestContext testContext, IWebDriver driver)
        {
            var pageSource = driver.PageSource;
            var path = Path.Combine(Path.GetTempPath(), testContext.TestName + ".PageSource.xml");
            File.WriteAllText(path, pageSource);
            // https://github.com/Microsoft/testfx/issues/394
            // testContext.AddResultFile(path);
        }

        public static void DumpScreenshot(TestContext testContext, IWebDriver driver)
        {
            var screenshot = ((RemoteWebDriver)driver).GetScreenshot().AsByteArray;
            var path = Path.Combine(Path.GetTempPath(), testContext.TestName + ".png");
            File.WriteAllBytes(path, screenshot);
            // https://github.com/Microsoft/testfx/issues/394
            // testContext.AddResultFile(path);
        }

        private static void DumpDriverInfo(IWebDriver driver)
        {
            var remoteDriver = (RemoteWebDriver)driver;
            Console.WriteLine($"Using {remoteDriver.Capabilities.GetCapability("BrowserName")}, version {remoteDriver.Capabilities.GetCapability("Version")}.");
        }

        private static string GetDriverName(TestContext testContext)
        {
            return GetSetting(testContext, "Environment_TestWebDriverName", "Chrome");
        }

        private static string GetTargetUrl(TestContext testContext)
        {
            return GetSetting(testContext, "Environment_TestTargetUrl", "http://localhost:8000/");
        }

        private static Size GetDriverSize(TestContext testContext)
        {
            var sizeString = GetSetting(testContext, "Environment_TestScreenSize", "1920x1080");
            var sizeMatch = Regex.Match(sizeString, "(?<width>\\d+)x(?<height>\\d+)");
            if (!sizeMatch.Success)
            {
                throw new ArgumentException($"'{sizeString}' is not a valid size String. Valid example is '1920x1080'.");
            }

            return new Size(int.Parse(sizeMatch.Groups["width"].Value), int.Parse(sizeMatch.Groups["height"].Value));
        }

        private static ConcurrentDictionary<string, string> settingsCache = new ConcurrentDictionary<string, string>();

        private static string GetEnvironmentVariableKeyFromKey(string key) 
        {
            const string environmentPrefix = "Environment_";
            int keyPrefixIndex = key.IndexOf(environmentPrefix);
            if (keyPrefixIndex == 0)
            {
                return key.Remove(keyPrefixIndex, environmentPrefix.Length);
            }
            else 
            {
                return key;
            }
        }

        private static string GetSetting(TestContext testContext, string key, string defaultValue)
        {
            string value = null;
            if (settingsCache.TryGetValue(key, out value))
            {
                return value;
            }
            

            value = defaultValue;
            string envValue = null;
            string envKey = GetEnvironmentVariableKeyFromKey(key);

            envValue = Environment.GetEnvironmentVariable(envKey, EnvironmentVariableTarget.Machine);
            if (envValue != null)
            {
                value = envValue;
            }

            envValue = Environment.GetEnvironmentVariable(envKey, EnvironmentVariableTarget.User);
            if (envValue != null)
            {
                value = envValue;
            }

            envValue = Environment.GetEnvironmentVariable(envKey);
            if (envValue != null)
            {
                value = envValue;
            }

            if (testContext.Properties.ContainsKey(key))
            {
                value = (string)testContext.Properties[key];
            }

            testContext.WriteLine($"Using {key}: {value}");
            settingsCache[key] = value;
            return value;
        }

        private static IWebDriver GetDriverInstance(TestContext testContext, string driverName)
        {
            IWebDriver driver;
            if (string.Equals(driverName, "ChromeHeadless", StringComparison.InvariantCultureIgnoreCase))
            {
                // Even though in a non-interactive build process Chrome
                // runs fine without the --headless switch, we make it
                // explicit here. This is partly due to a problem with 
                // the ChromeDriver, which in headless mode apparently 
                // does not respect driver.Manage().Window.Size. Thus
                // we set the size via the command line as well.
                var options = new ChromeOptions();
                options.AddArgument($"--headless");
                options.AddArgument($"--window-size={GetDriverSize(testContext).Width},{GetDriverSize(testContext).Height}");
                options.SetLoggingPreference(LogType.Browser, LogLevel.All);
                driver = new ChromeDriver(".", options);
            }
            else if (string.Equals(driverName, "Chrome", StringComparison.InvariantCultureIgnoreCase))
            {
                driver = new ChromeDriver(".");
            }
            else if (string.Equals(driverName, "Edge", StringComparison.InvariantCultureIgnoreCase))
            {
                driver = new EdgeDriver();
            }
            else if (string.Equals(driverName, "Internet Explorer", StringComparison.InvariantCultureIgnoreCase))
            {
                driver = new InternetExplorerDriver();
            }
            else
            {
                throw new ArgumentException($"Unknown driver {driverName}. Try 'Chrome','ChromeHeadless','Edge','Internet Explorer' or 'PhantomJS'.");
            }

            return driver;
        }

    }
}
