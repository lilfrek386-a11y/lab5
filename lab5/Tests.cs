using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
namespace lab5
{
	public class Tests
	{
		IWebDriver driver;
		WebDriverWait wait;

		[SetUp]
		public void Setup()
		{
			driver = new ChromeDriver();
			driver.Manage().Window.Maximize();

			wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
		}

		/// <summary>
		/// Test головної сторінки
		/// </summary>
		[Test]
		public void Verify_HomePageTitleAndUrl_AreCorrect()
		{
			driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/");
			Assert.That(driver.Url, Is.EqualTo("https://the-internet.herokuapp.com/"));
			Assert.That(driver.Title, Is.EqualTo("The Internet"));
		}

		/// <summary>
		/// Test №1
		/// на додавання та видалення кнопок
		/// </summary>
		[Test]
		public void AddAndRemoveElemnts_WhenClicked_ShouldCreateAndDeleteElement()
		{
			driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");
			var addButton = driver.FindElement(By.CssSelector("button[onclick='addElement()']"));

			addButton.Click();
			addButton.Click();
			addButton.Click();

			driver.FindElement(By.ClassName("added-manually")).Click();


			var addedElements = driver.FindElements(By.ClassName("added-manually"));

			Assert.That(addedElements.Count(), Is.EqualTo(2));
		}


		/// <summary>
		/// Test №2
		/// натискаємо на перший чекбокс та перевіряємо чи натиснуті два чекбокса
		/// </summary>
		[Test]
		public void Checkboxex_WhenClicked_ShouldBeSelected()
		{
			driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/checkboxes");

			var firstCheckbox = driver.FindElement(By.XPath("//*[@id=\"checkboxes\"]/input[1]"));

			firstCheckbox.Click();

			var secondCheckbox = driver.FindElement(By.XPath("//*[@id=\"checkboxes\"]/input[2]"));

			Assert.That(firstCheckbox.Selected, Is.True);
			Assert.That(secondCheckbox.Selected, Is.True);
		}


		/// <summary>
		/// Test №3
		/// обираємо першу опцію, перевіряємо, що вона натиснута
		/// та перевіряємо що перша стартова опція заблокована
		/// </summary>
		[Test]
		public void Dropdown_SelectOption_ShouldBeSelectedStartOptionShouldBeDisabled()
		{
			driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/dropdown");

			var listSelect = new SelectElement(driver.FindElement(By.Id("dropdown")));
			listSelect.SelectByText("Option 1");

			Assert.That(listSelect.SelectedOption.Text, Is.EqualTo("Option 1"));

			var disabledOption = driver.FindElement(By.XPath("//*[@id=\"dropdown\"]/option[1]"));

			Assert.That(disabledOption.Enabled, Is.False);
		}


		/// <summary>
		/// Test №4
		/// пише цифри перевіряємо що вони написані
		/// та перевіряємо форму на вводження тексту
		/// </summary>
		[Test]
		public void Inputs_NumberField_AcceptsNumbersOnly()
		{
			driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/inputs");

			var inputField = driver.FindElement(By.XPath("//*[@id=\"content\"]/div/div/div/input"));

			string num = "12345";

			inputField.SendKeys(num);

			var fieldValue = inputField.GetAttribute("value");

			Assert.That(fieldValue, Is.EqualTo(num));

			inputField.Clear();

			string text = "fjdsklfjsdkl";

			inputField.SendKeys(text);

			fieldValue = inputField.GetAttribute("value");

			Assert.That(fieldValue, Is.Empty);
		}



		/// <summary>
		/// Test №5
		/// перевіряємо кожну з сторінок, та яку відповідь ми отримали
		/// </summary>
		/// <param name="expectedStatus"></param>
		[Test]
		[TestCase(200)]
		[TestCase(301)]
		[TestCase(404)]
		[TestCase(500)]
		public void StatusCodes_StatusCode_WhenLinkIsClicked_ShouldMatchNetworkResponse(int expectedStatus)
		{

			INetwork network = driver.Manage().Network;
			network.StartMonitoring();
			long capturedStatusCode = -1;
			network.NetworkResponseReceived += (sender, e) =>
			{
				if (e.ResponseUrl.Contains($"/status_codes/{expectedStatus}"))
				{
					capturedStatusCode = e.ResponseStatusCode;
				}
			};
			driver.Navigate().GoToUrl($"https://the-internet.herokuapp.com/status_codes");
			driver.FindElement(By.LinkText(expectedStatus.ToString())).Click();

			wait.Until(_ => capturedStatusCode != -1);

			Assert.That(capturedStatusCode, Is.EqualTo(expectedStatus));

		}



		/// <summary>
		/// Test №6
		/// переміщюємо елементи та перевіємо їхні значення
		/// </summary>
		[Test]
		public void DragAndDrop_DragColumnAtoB_TextShouldSwap()
		{
			driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/drag_and_drop");

			var columnA = driver.FindElement(By.Id("column-a"));

			var columnB = driver.FindElement(By.Id("column-b"));

			var actions = new Actions(driver);

			actions.DragAndDrop(columnA, columnB).Perform();

			Assert.That(columnA.Text, Is.EqualTo("B"));
			Assert.That(columnB.Text, Is.EqualTo("A"));
		}


		/// <summary>
		/// Test №7
		/// перевіряємо саме через url для уникнення випадку коли Random видав одне і те саме значення
		/// </summary>
		[Test]
		public void ShiftingImage_WithPixelShiftParameter_ShouldMoveExplicitly()
		{
			driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/shifting_content/image?pixel_shift=10");
			var location1 = driver.FindElement(By.CssSelector("img.shift")).Location;

			driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/shifting_content/image?pixel_shift=100");
			var location2 = driver.FindElement(By.CssSelector("img.shift")).Location;

			Assert.That(location1, Is.Not.EqualTo(location2));
		}

		/// <summary>
		/// Test №8
		/// перевіряємо координати для лондона
		/// </summary>
		[Test]
		public void Geolocation_OverrideLocation_ShouldShowLondonCoordinates()
		{
	
			var coordinates = new Dictionary<string, object>
			{
				{ "latitude", 51.507351 },
				{ "longitude", -0.127758 },
				{ "accuracy", 100 }
			};

			((ChromeDriver)driver).ExecuteCdpCommand("Emulation.setGeolocationOverride", coordinates);

			driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/geolocation");

			driver.FindElement(By.XPath("//button[contains(text(),'Where am I')]")).Click();

			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

			var latElement = wait.Until(d => d.FindElement(By.Id("lat-value")));
			var longElement = driver.FindElement(By.Id("long-value"));

			Assert.That(latElement.Text, Does.Contain("51.5073"));
			Assert.That(longElement.Text, Does.Contain("-0.1277"));
		}


		/// <summary>
		/// Test №9
		/// перевіряємо чи були знайдені важливі помилки
		/// </summary>
		[Test]
		public void JSError_PageLoad_ShouldLogConsoleError()
		{
			driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/javascript_error");

			var logs = driver.Manage().Logs.GetLog(LogType.Browser);

			bool errorFound = false;

			foreach (var log in logs)
			{
				if (log.Level == LogLevel.Severe)
				{
					errorFound = true;
				}
			}
			Assert.That(errorFound, Is.True);
		}

		/// <summary>
		/// Test №10
		/// перевіряємо на вспливаюче вікно за допомогою js скрипту
		/// </summary>
		[Test]
		public void ExitIntent_MouseLeave_ShouldShowModal()
		{
			driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/exit_intent");

			var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

			IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

			string jsScript = @"
				var event = new MouseEvent('mouseleave', {
					'view': window,
					'bubbles': true,
					'cancelable': true,
					'clientX': 0,
					'clientY': 0 
				});
				document.documentElement.dispatchEvent(event);
			";

			js.ExecuteScript(jsScript);

			var modal = wait.Until(d => d.FindElement(By.XPath("//*[@id='ouibounce-modal']/div[2]")));

			wait.Until(d => modal.Displayed);

			Assert.That(modal.Displayed, Is.True);
		}

		[TearDown]
		public void TearDown()
		{
			driver.Quit();
		}
	}
}
