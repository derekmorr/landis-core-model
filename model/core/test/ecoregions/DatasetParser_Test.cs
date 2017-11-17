using Landis.Core;
using Landis.Ecoregions;
using Edu.Wisc.Forest.Flel.Util;
using NUnit.Framework;

namespace Landis.Test.Ecoregions
{
    [TestFixture]
    public class DatasetParser_Test
    {
        private DatasetParser parser;
        private LineReader reader;
        private StringReader currentLine;

        //---------------------------------------------------------------------

        [OneTimeSetUp]
        public void Init()
        {
            Data.InitializeDirectory(ModuleData.GetRelativePath("ecoregions"));
            parser = new DatasetParser();
        }

        //---------------------------------------------------------------------

        private FileLineReader OpenFile(string filename)
        {
            string path = Data.MakeInputPath(filename);
            return Landis.Data.OpenTextFile(path);
        }

        //---------------------------------------------------------------------

        private void TryParse(string filename,
                              int    errorLineNum)
        {
            try {
                reader = OpenFile(filename);
                // This method is only called on bad files, so we expect the
                // statement below to throw an exception.  Since we knowingly
                // ignore the variable "dataset", disable the CS0219 warning
                // "The variable '...' is assigned but its value is never used'.
#pragma warning disable 0219
                IEcoregionDataset dataset = parser.Parse(reader);
#pragma warning restore 0219
            }
            catch (System.Exception e) {
                Data.Output.WriteLine(e.Message.Replace(Data.Directory, Data.DirPlaceholder));
                LineReaderException lrExc = e as LineReaderException;
                if (lrExc != null)
                    Assert.AreEqual(errorLineNum, lrExc.LineNumber);
                Data.Output.WriteLine();
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
            Assert.Throws<LineReaderException>(delegate { TryParse("empty.txt", LineReader.EndOfInput); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void LandisData_WrongName()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("LandisData-WrongName.txt", 3); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void LandisData_NoValue()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("LandisData-NoValue.txt", 3); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void LandisData_MissingQuote()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("LandisData-MissingQuote.txt", 3); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void LandisData_WrongValue()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("LandisData-WrongValue.txt", 3); });
        }

        //---------------------------------------------------------------------

        private IEcoregionDataset ParseFile(string filename)
        {
            reader = OpenFile(filename);
            IEcoregionDataset dataset = parser.Parse(reader);
            reader.Close();
            return dataset;
        }

        //---------------------------------------------------------------------

        [Test]
        public void EmptyTable()
        {
            IEcoregionDataset dataset = ParseFile("EmptyTable.txt");
            Assert.AreEqual(0, dataset.Count);
        }

        //---------------------------------------------------------------------

        private void CompareDatasetAndFile(IEcoregionDataset dataset,
                                           string filename)
        {
            FileLineReader file = OpenFile(filename);
            InputLine inputLine = new InputLine(file);

            InputVar<string> LandisData = new InputVar<string>(Landis.Data.InputVarName);
            inputLine.ReadVar(LandisData);

            int expectedIndex = 0;
            foreach (IEcoregion ecoregion in dataset) {
                Assert.AreEqual(expectedIndex, ecoregion.Index);
                expectedIndex++;

                Assert.IsTrue(inputLine.GetNext());
                currentLine = new StringReader(inputLine.ToString());

                Assert.AreEqual(ReadValue<bool>(),   ecoregion.Active);
                Assert.AreEqual(ReadValue<byte>(),   ecoregion.MapCode);
                Assert.AreEqual(ReadValue<string>(), ecoregion.Name);
                Assert.AreEqual(ReadValue<string>(), ecoregion.Description);
            }
            Assert.IsFalse(inputLine.GetNext());
            file.Close();
        }

        //---------------------------------------------------------------------

        private T ReadValue<T>()
        {
            ReadMethod<T> method = InputValues.GetReadMethod<T>();
            int index;
            return method(currentLine, out index);
        }

        //---------------------------------------------------------------------

        [Test]
        public void FullTable()
        {
            string filename = "FullTable.txt";
            IEcoregionDataset dataset = ParseFile(filename);
            CompareDatasetAndFile(dataset, filename);
        }

        //---------------------------------------------------------------------

        [Test]
        public void Active_InvalidYes()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("Active-InvalidYes.txt", 12); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void Active_InvalidNo()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("Active-InvalidNo.txt", 12); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MapCode_Missing()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MapCode-Missing.txt", 12); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MapCode_Invalid()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MapCode-Invalid.txt", 12); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MapCode_Negative()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MapCode-Negative.txt", 12); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MapCode_TooBig()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MapCode-TooBig.txt", 12); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MapCode_Repeated()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MapCode-Repeated.txt", 12); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void Name_Missing()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("Name-Missing.txt", 12); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void Name_Repeated()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("Name-Repeated.txt", 12); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void Description_Missing()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("Description-Missing.txt", 12); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void Description_NoEndQuote()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("Description-NoEndQuote.txt", 12); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void ExtraText()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("ExtraText.txt", 12); });
        }
    }
}
