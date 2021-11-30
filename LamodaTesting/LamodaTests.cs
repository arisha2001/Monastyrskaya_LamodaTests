using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace LamodaTesting
{
    public class LamodaTests
    {
        IWebDriver driver;



        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(7);
            driver.Navigate().GoToUrl("https://www.lamoda.ru/");
        }



        [Test]
        public void TestPriceFilter()
        {
            driver.FindElement(By.XPath("//a[normalize-space()='Новинки']")).Click();
            driver.FindElement(By.XPath("//span[contains(@class,'multifilter__title') and contains(., 'Цена')]")).Click();
            driver.FindElement(By.CssSelector(".range__value_left")).SendKeys("1000");
            driver.FindElement(By.CssSelector(".range__value_right")).SendKeys("10000");
            driver.FindElement(By.CssSelector(".multifilter_price .multifilter-actions__apply")).Click();


            new WebDriverWait(driver, TimeSpan.FromSeconds(5))
                .Until(x => driver.FindElements(By.XPath("//div[@class='product-catalog-main product-catalog-main_innactive']")).Any());

            new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                .Until(x => driver.FindElements(By.XPath("//div[@class='product-catalog-main product-catalog-main_innactive']")).Count == 0);

            int[] actualValues = Array.ConvertAll(driver.FindElements(By.XPath("//span[(contains(@class, 'price__actual') and not(contains(@class, 'parts__price_cd-disabled'))) or contains(@class,'price__action js-cd-discount')]"))
                .Select(price => price.Text.Replace(" ", "").Trim())
                .ToArray<string>(), s => int.Parse(s));
            actualValues.ToList().ForEach(actualPrice => Assert.True(actualPrice >= 1000 && actualPrice <= 10000, "Price filter goes wrong at the test on ", actualPrice, ", actualPrice should be >= 1000 and <= 10000"));

        }



        [Test]
        public void TestToolTipText()
        {
            driver.FindElement(By.XPath("//a[normalize-space()='Новинки']")).Click();

            new Actions(driver).MoveToElement(driver.FindElement(By.CssSelector(".products-list-item"))).Build().Perform();
            Assert.IsFalse(driver.FindElements(By.CssSelector(".products-list-item__qv::before")).Any(), "Tooltip has not appeared.");

        }



        [Test]
        public void TestSignUp()
        {
            driver.FindElement(By.LinkText("Войти")).Click();
            driver.FindElement(By.LinkText("Создать аккаунт")).Click();
            driver.FindElement(By.XPath("//input[@name='Имя']")).SendKeys("Arina");
            driver.FindElement(By.XPath("//div[./div[@class='d-modal__header']/span]//input[@type='email']")).SendKeys("ajkaklkll@mail.ru");
            driver.FindElement(By.XPath("//div[./div[@class='d-modal__header']/span]//input[@type='password' and @name='Пароль']")).SendKeys("Dggh");
            driver.FindElement(By.XPath("//input[@name='Подтверждение пароля']")).SendKeys("Dgghjdkdkd");

            Assert.IsFalse(driver.FindElements(By.XPath("//button[contains(text(), 'Зарегистрироваться') and not(@disabled)]")).Any(),
                "Smth go wrong if it works. We couldn't register. Password missmatch. Button must be disabled.");

        }



        [TearDown]
        public void CleanUp()
        {
            driver.Quit();
        }
    }


}

