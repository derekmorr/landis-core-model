using Edu.Wisc.Forest.Flel.Util;
using Landis.PlugIns;
using Landis.PlugIns.Admin;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Landis.Test.PlugIns.Admin
{
    [TestFixture]
    public class ExtensionParser_Test
    {
    	private Dataset dataset;
        private ExtensionParser parser;
        private LineReader reader;

        //---------------------------------------------------------------------

        [SetUp]
        public void Init()
        {
        	dataset = new Dataset();
        	EditableExtensionInfo.Dataset = dataset;
            parser = new ExtensionParser();
            App.InstallingExtension = false;
        }

        //---------------------------------------------------------------------

        private FileLineReader OpenFile(string filename)
        {
            string path = System.IO.Path.Combine(Data.Directory, filename);
            return Landis.Data.OpenTextFile(path);
        }

        //---------------------------------------------------------------------

        private void TryParse(string filename)
        {
            int? errorLineNum = Testing.FindErrorMarker(Data.MakeInputPath(filename));
            try {
                reader = OpenFile(filename);
                ExtensionInfo extension = parser.Parse(reader);
            }
            catch (System.Exception e) {
                Data.Output.WriteLine();
                Data.Output.WriteLine(e.Message.Replace(Data.Directory, Data.DirPlaceholder));
                LineReaderException lrExc = e as LineReaderException;
                if (lrExc != null && errorLineNum.HasValue)
                    Assert.AreEqual(errorLineNum.Value, lrExc.LineNumber);
                throw;
            }
            finally {
                reader.Close();
            }
        }

        //---------------------------------------------------------------------

        [Test]
        public void Empty()
        {
            Assert.Throws<LineReaderException>(() => TryParse("empty.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void LandisData_WrongValue()
        {
            Assert.Throws<LineReaderException>(() => TryParse("LandisData-WrongValue.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Name_Missing()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Name-Missing.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Name_Empty()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Name-Empty.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Name_Whitespace()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Name-Whitespace.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Name_InUse()
        {
        	ExtensionInfo fooBarExtension = new ExtensionInfo(
				"Foo Bar",  // name
        	    null,       // version
        	    null,       // type
        	    null,       // assembly name
        	    null,       // class name
        	    null,       // description
        	    null,       // user guide path
        	    null        // core version
            );
        	dataset.Add(fooBarExtension);
        	try {
                Assert.Throws<LineReaderException>(() => TryParse("Foo-Bar.txt"));
        	}
        	finally {
        		dataset.Remove(fooBarExtension.Name);
        	}
        }

        //---------------------------------------------------------------------

        [Test]
        public void Version_Empty()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Version-Empty.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Version_Whitespace()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Version-Whitespace.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Type_Missing()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Type-Missing.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Type_Empty()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Type-Empty.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Type_Whitespace()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Type-Whitespace.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Description_Missing()
        {
            ExtensionInfo extension = ParseFile("Description-Missing.txt");
            Assert.IsNotNull(extension);
            Assert.AreEqual("Foo Bar", extension.Name);
            Assert.AreEqual("v3.8 (beta 4)", extension.Version);
            Assert.AreEqual("disturbance:foo-bar", extension.Type);
            Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBar",
                            extension.AssemblyName);
            Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBarPlugIn",
                            extension.ClassName);
            Assert.IsNull(extension.Description);
            Assert.AreEqual("Foo-Bar_User-Guide.txt",
                            extension.UserGuidePath);
            Assert.AreEqual(new System.Version("5.0"),
                            extension.CoreVersion);
        }

        //---------------------------------------------------------------------

        [Test]
        public void Description_Empty()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Description-Empty.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Description_Whitespace()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Description-Whitespace.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void UserGuide_Missing()
        {
            ExtensionInfo extension = ParseFile("UserGuide-Missing.txt");
            Assert.IsNotNull(extension);
            Assert.AreEqual("Foo Bar", extension.Name);
            Assert.AreEqual("v3.8 (beta 4)", extension.Version);
            Assert.AreEqual("disturbance:foo-bar", extension.Type);
            Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBar",
                            extension.AssemblyName);
            Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBarPlugIn",
                            extension.ClassName);
            Assert.AreEqual("Compute foo-bar metrics across landscape",
                            extension.Description);
            Assert.IsNull(extension.UserGuidePath);
            Assert.AreEqual(new System.Version("5.0"),
                            extension.CoreVersion);
        }

        //---------------------------------------------------------------------

        [Test]
        public void UserGuide_Empty()
        {
            Assert.Throws<LineReaderException>(() => TryParse("UserGuide-Empty.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void UserGuide_Whitespace()
        {
            Assert.Throws<LineReaderException>(() => TryParse("UserGuide-Whitespace.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Assembly_Missing()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Assembly-Missing.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Assembly_Empty()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Assembly-Empty.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Assembly_Whitespace()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Assembly-Whitespace.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Class_Missing()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Class-Missing.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Class_Empty()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Class-Empty.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Class_Whitespace()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Class-Whitespace.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void CoreVersion_Missing()
        {
            ExtensionInfo extension = ParseFile("CoreVersion-Missing.txt");
            Assert.IsNotNull(extension);
            Assert.AreEqual("Foo Bar", extension.Name);
            Assert.AreEqual("v3.8 (beta 4)", extension.Version);
            Assert.AreEqual("disturbance:foo-bar", extension.Type);
            Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBar",
                            extension.AssemblyName);
            Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBarPlugIn",
                            extension.ClassName);
            Assert.AreEqual("Compute foo-bar metrics across landscape",
                            extension.Description);
            Assert.AreEqual("Foo-Bar_User-Guide.txt",
                            extension.UserGuidePath);
            Assert.IsNull(extension.CoreVersion);
        }

        //---------------------------------------------------------------------

        [Test]
        public void CoreVersion_NoMinor()
        {
            Assert.Throws<LineReaderException>(() => TryParse("CoreVersion-NoMinor.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void CoreVersion_BadValue()
        {
            Assert.Throws<LineReaderException>(() => TryParse("CoreVersion-BadValue.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void CoreVersion_TooBig()
        {
            Assert.Throws<LineReaderException>(() => TryParse("CoreVersion-TooBig.txt"));
        }

        //---------------------------------------------------------------------

        private ExtensionInfo ParseFile(string filename)
        {
        	try {
	            reader = OpenFile(filename);
	            return parser.Parse(reader);
        	}
        	finally {
	            reader.Close();
        	}
        }

        //---------------------------------------------------------------------

        [Test]
        public void FooBar()
        {
            ExtensionInfo extension = ParseFile("Foo-Bar.txt");
            Assert.IsNotNull(extension);
            Assert.AreEqual("Foo Bar", extension.Name);
            Assert.AreEqual("v3.8 (beta 4)", extension.Version);
            Assert.AreEqual("disturbance:foo-bar", extension.Type);
            Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBar",
                            extension.AssemblyName);
            Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBarPlugIn",
                            extension.ClassName);
            Assert.AreEqual("Compute foo-bar metrics across landscape",
                            extension.Description);
            Assert.AreEqual("Foo-Bar_User-Guide.txt",
                            extension.UserGuidePath);
            Assert.AreEqual(new System.Version("5.0"),
                            extension.CoreVersion);
        }

        //---------------------------------------------------------------------

        [Test]
        public void Extra()
        {
            Assert.Throws<LineReaderException>(() => TryParse("Extra.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void WrongName_CoreVersion()
        {
            Assert.Throws<LineReaderException>(() => TryParse("WrongName-CoreVersion.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void WrongName_UserGuide_CoreVer()
        {
            Assert.Throws<LineReaderException>(() => TryParse("WrongName-UserGuide-CoreVer.txt"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void WrongName_Desc_UGuide_CoreVer()
        {
            Assert.Throws<LineReaderException>(() => TryParse("WrongName-Desc-UGuide-CoreVer.txt"));
        }
    }
}
