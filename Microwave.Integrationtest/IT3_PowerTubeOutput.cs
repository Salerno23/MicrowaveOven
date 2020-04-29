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
    public class IT3_PowerTubeOutput
    {
        private PowerTube _uut;
        private IOutput _output;
        private StringWriter _stw;

        [SetUp]
        public void SetUp()
        {
            _output = new Output();
            _uut = new PowerTube(_output);
            _stw = new StringWriter();
            Console.SetOut(_stw);
        }

        //Power must be within 1 and 700, not 1 and 100
        //Unit test corrected
        [TestCase(1)]
        [TestCase(50)]
        [TestCase(700)]
        public void TurnOn_WasOffCorrectPower_CorrectOutput(int power)
        {
            _uut.TurnOn(power);
            Assert.That(_stw.ToString, Contains.Substring($"{power}"));
           
        }

        //Power must be within 1 and 700, not 1 and 100
        //Unit test corrected
        [TestCase(-5)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(701)]
        [TestCase(750)]
        public void TurnOn_WasOffOutOfRangePower_ThrowsException(int power)
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => _uut.TurnOn(power));
        }

        [Test]
        public void TurnOff_WasOn_CorrectOutput()
        {
            _uut.TurnOn(50);
            _uut.TurnOff();
            Assert.That(_stw.ToString, Contains.Substring("off"));
        }

        [Test]
        public void TurnOff_WasOff_NoOutput()
        {
            _uut.TurnOff();
            Assert.That(_stw.ToString, Is.Empty);
            
        }

        [Test]
        public void TurnOn_WasOn_ThrowsException()
        {
            _uut.TurnOn(50);
            Assert.Throws<System.ApplicationException>(() => _uut.TurnOn(60));
        }
    }
    
}
