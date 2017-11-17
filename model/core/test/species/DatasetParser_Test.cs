using Landis.Core;
using Landis.Species;
using Edu.Wisc.Forest.Flel.Util;
using NUnit.Framework;

namespace Landis.Test.Species
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
            Data.InitializeDirectory(ModuleData.GetRelativePath("species"));
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
                ISpeciesDataset dataset = parser.Parse(reader);
#pragma warning restore 0219
            }
            catch (System.Exception e) {
                Data.Output.WriteLine(e.Message.Replace(Data.Directory,
                                                        Data.DirPlaceholder));
                Data.Output.WriteLine();
                LineReaderException lrExc = e as LineReaderException;
                if (lrExc != null)
                    Assert.AreEqual(errorLineNum, lrExc.LineNumber);
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

        private ISpeciesDataset ParseFile(string filename)
        {
            reader = OpenFile(filename);
            ISpeciesDataset dataset = parser.Parse(reader);
            reader.Close();
            return dataset;
        }

        //---------------------------------------------------------------------

        [Test]
        public void EmptyTable()
        {
            ISpeciesDataset dataset = ParseFile("EmptyTable.txt");
            Assert.AreEqual(0, dataset.Count);
        }

        //---------------------------------------------------------------------

        private void CompareDatasetAndFile(ISpeciesDataset dataset,
                                           string filename)
        {
            FileLineReader file = OpenFile(filename);
            InputLine inputLine = new InputLine(file);

            InputVar<string> LandisData = new InputVar<string>(Landis.Data.InputVarName);
            inputLine.ReadVar(LandisData);

            int expectedIndex = 0;
            foreach (ISpecies species in dataset) {
                Assert.AreEqual(expectedIndex, species.Index);
                expectedIndex++;

                Assert.IsTrue(inputLine.GetNext());
                currentLine = new StringReader(inputLine.ToString());

                Assert.AreEqual(ReadValue<string>(),               species.Name);
                Assert.AreEqual(ReadValue<int>(),                  species.Longevity);
                Assert.AreEqual(ReadValue<int>(),                  species.Maturity);
                Assert.AreEqual(ReadValue<byte>(),                 species.ShadeTolerance);
                Assert.AreEqual(ReadValue<byte>(),                 species.FireTolerance);
                Assert.AreEqual(ReadEffectiveSeedDist(),           species.EffectiveSeedDist);
                Assert.AreEqual(ReadValue<int>(),                  species.MaxSeedDist);
                Assert.AreEqual(ReadValue<float>(),                species.VegReprodProb);
                Assert.AreEqual(ReadValue<int>(),                  species.MinSproutAge);
                Assert.AreEqual(ReadValue<int>(),                  species.MaxSproutAge);
                Assert.AreEqual(ReadValue<PostFireRegeneration>(), species.PostFireRegeneration);
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

        private int ReadEffectiveSeedDist()
        {
            int index;
            return EffectiveSeedDist.ReadMethod(currentLine, out index);
        }

        //---------------------------------------------------------------------

        [Test]
        public void FullTable()
        {
            string filename = "FullTable.txt";
            ISpeciesDataset dataset = ParseFile(filename);
            CompareDatasetAndFile(dataset, filename);
        }

        //---------------------------------------------------------------------

        [Test]
        public void NameRepeated()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("NameRepeated.txt", 21); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void LongevityMissing()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("LongevityMissing.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void LongevityInvalid()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("LongevityInvalid.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void LongevityNegative()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("LongevityNegative.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MaturityMissing()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MaturityMissing.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MaturityInvalid()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MaturityInvalid.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MaturityNegative()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MaturityNegative.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MaturityTooBig()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MaturityTooBig.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void ShadeMissing()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("ShadeMissing.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void ShadeInvalid()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("ShadeInvalid.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void ShadeNegative()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("ShadeNegative.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void ShadeZero()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("ShadeZero.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void ShadeTooBig()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("ShadeTooBig.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void FireMissing()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("FireMissing.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void FireInvalid()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("FireInvalid.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void FireNegative()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("FireNegative.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void FireZero()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("FireZero.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void FireTooBig()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("FireTooBig.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void EffSeedMissing()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("EffSeedMissing.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void EffSeedInvalid()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("EffSeedInvalid.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void EffSeedZero()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("EffSeedZero.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void EffSeedNegative()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("EffSeedNegative.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MaxSeedMissing()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MaxSeedMissing.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MaxSeedNegative()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MaxSeedNegative.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MaxSeedLessThanEff()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MaxSeedLessThanEff.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void ReprodProbMissing()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("ReprodProbMissing.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void ReprodProbNegative()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("ReprodProbNegative.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void ReprodProbTooBig()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("ReprodProbTooBig.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MinSproutMissing()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MinSproutMissing.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MinSproutNegative()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MinSproutNegative.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MinSproutMoreThanLongevity()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MinSproutMoreThanLongevity.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MaxSproutMissing()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MaxSproutMissing.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MaxSproutNegative()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MaxSproutNegative.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MaxSproutMoreThanLongevity()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MaxSproutMoreThanLongevity.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void MaxSproutLessThanMin()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("MaxSproutLessThanMin.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void PostFireRegenMissing()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("PostFireRegenMissing.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void PostFireRegenInvalid()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("PostFireRegenInvalid.txt", 9); });
        }

        //---------------------------------------------------------------------

        [Test]
        public void ExtraText()
        {
            Assert.Throws<LineReaderException>(delegate { TryParse("ExtraText.txt", 9); });
        }
    }
}
