﻿using System;
using System.IO;
using System.Threading;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NSubstitute.Extensions;
using NUnit.Framework;

namespace Microwave.Integrationtest
{
    [TestFixture]
    public class IT4_CookControllerDisplayPowertubeTimer
    {
        private CookController _uut;
        
        private IDisplay _display;
        private IPowerTube _pt;
        private ITimer _fakeTimer;
        private IOutput _output;

        private StringWriter _stw;

        [SetUp]
        public void Setup()
        {
            _output = new Output();
            _display = new Display(_output);
            _pt = new PowerTube(_output);

            //From slides: Timers are one of the types of classes
            //it is a good idea to fake as long as possible
            _fakeTimer = Substitute.For<ITimer>();

            _uut = new CookController(_fakeTimer,  _display, _pt);

            _stw = new StringWriter();
            Console.SetOut(_stw);
        }

        //Need to be tested later without fake
        //[Test]
        //public void StartCooking_ValidParameters_TimerStarted()
        //{
        //    _uut.StartCooking(50, 60);

        //    //Is equal to 60k milliseconds
        //    Assert.That(_timer.TimeRemaining, Is.EqualTo(60000));
        //}

        [Test]
        public void StartCooking_ValidParameters_PowerTubeStarted()
        {
            _uut.StartCooking(50, 60);

            Assert.That(_stw.ToString(), Contains.Substring("50"));
        }

        [TestCase(0)]
        [TestCase(101)]
        public void StartCooking_InValidWattage_PowerTubeStarted(int power)
        {
            Assert.That(() => _uut.StartCooking(power, 60), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void StartCooking_PowerTubeStartTwice()
        {
            _uut.StartCooking(50, 60);
            
            Assert.That(() => _uut.StartCooking(50, 60), Throws.TypeOf<ApplicationException>());
        }

        [Test]
        public void StartCooking_PowerTubeStop()
        {
            _uut.StartCooking(50, 60);
            _uut.Stop();

            Assert.That(_stw.ToString(), Contains.Substring("off"));
        }

        [Test]
        public void Cooking_TimerTick_DisplayCalled()
        {
            _uut.StartCooking(50, 60);

            _fakeTimer.TimeRemaining.Returns(60000);
            _fakeTimer.TimerTick += Raise.Event();

            Assert.That(_stw.ToString(), Contains.Substring("01:00"));
        }
    }
}
