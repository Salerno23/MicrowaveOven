using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Integrationtest
{
    [TestFixture]
    class IT5_TimerButtonStartCancelButtonPowerButtonDoorUserInterface
    {
        private IUserInterface _ui;

        private IButton _startCancelButton;
        private IButton _powerButton;
        private IButton _timerButton;

        private IDoor _door;

        private IDisplay _fakeDisplay;
        private ILight _fakeLight;
        private ICookController _fakeCookController;

        [SetUp]
        public void Setup()
        {
            _startCancelButton = new Button();
            _powerButton = new Button();
            _timerButton = new Button();
            _door = new Door();

            _fakeDisplay = Substitute.For<IDisplay>();
            _fakeLight = Substitute.For<ILight>();
            _fakeCookController = Substitute.For<ICookController>();

            _ui = new UserInterface(_powerButton, _timerButton, _startCancelButton, _door, 
                                        _fakeDisplay, _fakeLight, _fakeCookController);
        }

        #region Door

        [Test]
        public void Ready_DoorOpen_LightOn()
        {
            _door.Open();

            _fakeLight.Received().TurnOn();
        }

        [Test]
        public void DoorOpen_DoorClose_LightOff()
        {
            _door.Open();
            _door.Close();

            _fakeLight.Received().TurnOff();
        }

        #endregion

        #region PowerButton

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(14)]
        public void Ready_PowerButtonPresses_Between2_And_14(int powerButtonPresses)
        {
            for(var i = 1; i <= powerButtonPresses; i++)
            {
                _powerButton.Press();
            }

            _fakeDisplay.Received(1).ShowPower(Arg.Is<int>(50 * powerButtonPresses));
        }

        [Test]
        public void Ready_15PowerButton_PowerIs50Again()
        {
            for (int i = 1; i <= 15; i++)
            {
                _powerButton.Press();
            }

            _fakeDisplay.Received(2).ShowPower(50);
        }

        [Test]
        public void SetPower_DoorOpened_DisplayCleared_LightOn()
        {
            _powerButton.Press();
            _door.Open();

            _fakeDisplay.Received(1).Clear();
            _fakeLight.Received(1).TurnOn();
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

            _fakeDisplay.Received(1).ShowTime(Arg.Is<int>(presses), Arg.Is<int>(0));
        }

        [Test]
        public void SetTime_DoorOpened_DisplayCleared_LightOn()
        {
            _powerButton.Press();
            _timerButton.Press();
            _door.Open();

            _fakeDisplay.Received().Clear();
            _fakeLight.Received().TurnOn();
        }

        #endregion

        #region StartCancelButton
        
        [Test]
        public void SetPower_CancelButton_DisplayCleared()
        {
            _powerButton.Press();
            _startCancelButton.Press();

            _fakeDisplay.Received(1).Clear();
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

            _fakeCookController.Received(1).StartCooking(50 * powerPresses, 60 * timePresses);
            _fakeLight.Received(1).TurnOn();
        }

        [Test]
        public void Cooking_CookingIsDone_ClearDisplay_LightOff()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            _ui.CookingIsDone();

            _fakeDisplay.Received(1).Clear();
            _fakeLight.Received(1).TurnOff();
        }

        [Test]
        public void Cooking_DoorIsOpened_CookerCalled()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            _door.Open();

            _fakeCookController.Received(1).Stop();
            _fakeDisplay.Received(1).Clear();
        }

        [Test]
        public void Cooking_CancelButton_CookerCalled_LightCalled_ClearDisplay()
        {
            _powerButton.Press();
            _timerButton.Press();

            _startCancelButton.Press();
            _startCancelButton.Press();

            _fakeCookController.Received(1).Stop();
            _fakeLight.Received(1).TurnOff();
            _fakeDisplay.Received(1).Clear();
        }

        #endregion
    }
}
