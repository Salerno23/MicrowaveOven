using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Integrationtest
{
    [TestFixture]
    class IT6_UserInterfaceLightDisplayCookController
    {
        private UserInterface _ui;

        private Button _startCancelButton;
        private Button _powerButton;
        private Button _timerButton;

        private Door _door;

        private Display _display;
        private Light _light;
        private CookController _cookController;
        private Output _output;
        private PowerTube _powerTube;

        private ITimer _fakeTimer;

        private StringWriter _stw;

        [SetUp]
        public void Setup()
        {
            _startCancelButton = new Button();
            _powerButton = new Button();
            _timerButton = new Button();
            _door = new Door();
            
            _output = new Output();
            
            _powerTube = new PowerTube(_output);
            _display = new Display(_output);
            _light = new Light(_output);

            _fakeTimer = Substitute.For<ITimer>();

            _cookController = new CookController(_fakeTimer, _display, _powerTube);

            _ui = new UserInterface(_powerButton, _timerButton, _startCancelButton, _door, _display, _light, _cookController);

            _cookController.UI = _ui;

            _stw = new StringWriter();
            Console.SetOut(_stw);
        }

        #region Door

        [Test]
        public void Ready_DoorOpen_LightOn()
        {
            _door.Open();

            Assert.That(_stw.ToString(), Contains.Substring("on"));
        }

        [Test]
        public void DoorOpen_DoorClose_LightOff()
        {
            _door.Open();
            _door.Close();

            Assert.That(_stw.ToString(), Contains.Substring("off"));
        }

        #endregion

        #region PowerButton

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(14)]
        public void Ready_PowerButtonPresses_Between2_And_14(int powerButtonPresses)
        {
            for (var i = 1; i <= powerButtonPresses; i++)
            {
                _powerButton.Press();
            }

            Assert.That(_stw.ToString(), Contains.Substring($"{50 * powerButtonPresses}"));
        }

        [Test]
        public void Ready_15PowerButton_PowerIs50Again()
        {
            for (int i = 1; i <= 15; i++)
            {
                _powerButton.Press();
            }

            Assert.That(_stw.ToString(), Contains.Substring("50").And.Contain("50"));
        }

        [Test]
        public void SetPower_DoorOpened_DisplayCleared_LightOn()
        {
            _powerButton.Press();
            _door.Open();


            Assert.That(_stw.ToString(), Contains.Substring("cleared").And.Contain("on"));
        }
        #endregion

        #region TimerButton

        [TestCase(1)]
        [TestCase(2)]
        public void SetPower_TimeButton_Pressed(int presses)
        {
            _powerButton.Press();

            for (var i = 1; i <= presses; i++)
            {
                _timerButton.Press();
            }

            Assert.That(_stw.ToString(), Contains.Substring($"{presses}").And.Contain("0"));
        }

        [Test]
        public void SetTime_DoorOpened_DisplayCleared_LightOn()
        {
            _powerButton.Press();
            _timerButton.Press();
            _door.Open();


            Assert.That(_stw.ToString(), Contains.Substring("cleared").And.Contain("on"));
        }

        #endregion

        #region StartCancelButton

        [Test]
        public void SetPower_CancelButton_DisplayCleared()
        {
            _powerButton.Press();
            _startCancelButton.Press();

            Assert.That(_stw.ToString(), Contains.Substring("cleared"));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(14, 1)]
        public void Ready_PowerAndTime_CookerIsCalledCorrectly_LightOn(int powerPresses, int timePresses)
        {
            for (var i = 1; i <= powerPresses; i++)
            {
                _powerButton.Press();
            }

            for (var i = 1; i <= timePresses; i++)
            {
                _timerButton.Press();
            }

            _startCancelButton.Press();

            Assert.That(_stw.ToString(), Contains.Substring($"{50 * powerPresses}"));
            Assert.That(_stw.ToString(), Contains.Substring($"{1 * timePresses}")); //shows in mins
            Assert.That(_stw.ToString(), Contains.Substring("on"));
        }

        [Test]
        public void Cooking_CookingIsDone_ClearDisplay_LightOff()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            _ui.CookingIsDone();

            Assert.That(_stw.ToString(), Contains.Substring("cleared"));
            Assert.That(_stw.ToString(), Contains.Substring("PowerTube").And.Contain("off"));
        }

        [Test]
        public void Cooking_DoorIsOpened_CookerCalled()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            _door.Open();

            Assert.That(_stw.ToString(), Contains.Substring("off"));
            Assert.That(_stw.ToString(), Contains.Substring("cleared"));
        }

        [Test]
        public void Cooking_CancelButton_CookerCalled_LightCalled_ClearDisplay()
        {
            _powerButton.Press();
            _timerButton.Press();

            _startCancelButton.Press();
            _startCancelButton.Press();

            Assert.That(_stw.ToString(), Contains.Substring("PowerTube").And.Contain("off"));
            Assert.That(_stw.ToString(), Contains.Substring("Light").And.Contain("off"));
            Assert.That(_stw.ToString(), Contains.Substring("cleared"));
        }

        #endregion

        #region misc

        [Test]
        public void Cooking_TimerExpired_PowerTubeOff()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            _fakeTimer.Expired += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(_stw.ToString(), Contains.Substring("PowerTube").And.Contain("off"));
        }

        [Test]
        public void Cooking_TimerExpired_UICalled()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            _fakeTimer.Expired += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(_stw.ToString(), Contains.Substring("cleared"));
            Assert.That(_stw.ToString(), Contains.Substring("Light").And.Contain("off"));
        }

        #endregion
    }
}
