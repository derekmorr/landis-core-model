using Landis.Ecoregions;
using Edu.Wisc.Forest.Flel.Util;
using NUnit.Framework;

namespace Landis.Test.Ecoregions
{
    [TestFixture]
    public class EditableParameters_Test
    {
        private EditableParameters parameters;

        //---------------------------------------------------------------------

        [OneTimeSetUp]
        public void Init()
        {
            parameters = new EditableParameters();
        }

        //---------------------------------------------------------------------

        [Test]
        public void NameEmpty()
        {
            Assert.Throws<InputValueException>(delegate { 
                InputValue<string> name = new InputValue<string>("", "");
                parameters.Name = name;
            });
        }

        //---------------------------------------------------------------------

        [Test]
        public void NameWhitespace()
        {
            Assert.Throws<InputValueException>(delegate {
                InputValue<string> name = new InputValue<string>("   ", "   ");
                parameters.Name = name;
            });
        }
    }
}
