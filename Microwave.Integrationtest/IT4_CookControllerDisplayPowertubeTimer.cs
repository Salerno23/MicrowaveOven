using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NUnit.Framework;

namespace Microwave.Integrationtest
{
    [TestFixture]
    public class IT4_CookControllerDisplayPowertubeTimer
    {
        private CookController _uut;
        
        private IDisplay _display;
        private IPowerTube _pt;
        private ITimer _timer;
        private IOutput _output;

        [SetUp]
        public void Setup()
        {
            _output = new Output();
            _display = new Display(_output);
            _pt = new PowerTube(_output);
            _timer = new Timer();

            _uut = new CookController(_timer,  _display, _pt);
        }

        //[Test]
        //public void StartCooking_





    }
}
