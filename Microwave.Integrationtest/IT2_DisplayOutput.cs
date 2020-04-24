using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Interfaces;
using NUnit.Framework;

namespace Microwave.Integrationtest
{
    [TestFixture]
    public class IT2_DisplayOutput
    {
        private Display _uut;
        private IOutput _output;
        private StringWriter _stw;

        [SetUp]
        public void Setup()
        {
            _output = new Output();
            _uut = new Display(_output);
            _stw = new StringWriter();
            Console.SetOut(_stw);
        }

        [Test]
        public void ShowTime_ZeroMinuteZeroSeconds_CorrectOutput()
        {
            _uut.ShowTime(0, 0);
            Assert.That(_stw.ToString, Contains.Substring("00:00"));
        }

        [Test]
        public void ShowTime_ZeroMinuteSomeSecond_CorrectOutput()
        {
            _uut.ShowTime(0, 5);
            Assert.That(_stw.ToString, Contains.Substring("00:05"));
        }

        [Test]
        public void ShowTime_SomeMinuteZeroSecond_CorrectOutput()
        {
            _uut.ShowTime(5, 0);
            Assert.That(_stw.ToString(), Contains.Substring("05:00"));
        }

        [Test]
        public void ShowTime_SomeMinuteSomeSecond_CorrectOutput()
        {
            _uut.ShowTime(10, 15);
            Assert.That(_stw.ToString(), Contains.Substring("10:15"));
        }

        [Test]
        public void ShowPower_Zero_CorrectOutput()
        {
            _uut.ShowPower(0);
            Assert.That(_stw.ToString, Contains.Substring("0 W"));
        }

        [Test]
        public void ShowPower_NotZero_CorrectOutput()
        {
            _uut.ShowPower(150);
            Assert.That(_stw.ToString, Contains.Substring("150 W"));
        }

        [Test]
        public void Clear_CorrectOutput()
        {
            _uut.Clear();
            Assert.That(_stw.ToString, Contains.Substring("cleared"));
        }
    }
}
