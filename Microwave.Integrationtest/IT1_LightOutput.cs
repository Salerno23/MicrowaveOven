using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Interfaces;

namespace Microwave.Integrationtest
{
    [TestFixture]
    public class IT1_LightOutput
    {
        private Light _uut;
        private IOutput _output;
        private StringWriter _stw;

        [SetUp]
        public void Setup()
        {
            _output = new Output();
            _uut = new Light(_output);
            _stw = new StringWriter();
            Console.SetOut(_stw);
        }

        [Test]
        public void TurnOn_WasOff_CorrectOutput()
        {
            _uut.TurnOn();
            Assert.That(_stw.ToString(), Contains.Substring("on"));
        }

        [Test]
        public void TurnOff_WasOn_CorrectOutput()
        {
            _uut.TurnOn();
            _uut.TurnOff();

            Assert.That(_stw.ToString(), Contains.Substring("off"));
        }

        [Test]
        public void TurnOn_WasOn_CorrectOutput()
        {
            _uut.TurnOn();
            _uut.TurnOn();
            Assert.That(_stw.ToString(), Contains.Substring("on"));
        }

        [Test]
        public void TurnOff_WasOff_CorrectOutput()
        {
            _uut.TurnOff();
            Assert.That(_stw.ToString(), Is.Empty);
        }
    }
}
