using ChordRandomizer;

namespace ChordRandomizerTests
{
    [TestClass]
    public class GuitarFingeringTests()
    {
        [TestMethod]
        public void GuitarFingeringConstructorTest()
        {
            List<string> notes = ["C", "C#", "", "D", "E", "F"];
            GuitarFingering fingering = new(notes);

            bool areEqual = notes.SequenceEqual(fingering.FingeringNotesNames);

            Assert.IsNotNull(fingering);
            Assert.IsTrue(areEqual);
        }


        [TestMethod]
        public void DistanceBetweenExtremeFretsTest()
        {
            List<string> notes = ["C", "C#", "", "D", "E", "F"];
            int[] frets = [12, 3, 2, 7, 4, -1];
            GuitarFingering fingering = new(notes);
            frets.CopyTo(fingering.Frets, 0);

            Assert.AreEqual(10, fingering.CalculateDistanceBetweenExtremeFrets());
        }

        [TestMethod]
        public void GuitarFingeringCopyConstructorTest()
        {
            List<string> notes = ["C", "C#", "", "D", "E", "F"];
            int[] frets = [8, 2, -1, 0, 7, 1];
            int distanceBetweenExtremeFrets = 7;
            GuitarFingering fingering = new(notes);
            frets.CopyTo(fingering.Frets, 0);
            GuitarFingering copy = new(fingering);

            bool actual = notes.SequenceEqual(copy.FingeringNotesNames)
                && fingering.FingeringNotesNames.SequenceEqual(copy.FingeringNotesNames)
                && frets.SequenceEqual(copy.Frets)
                && distanceBetweenExtremeFrets == copy.DistanceBetweenExtremeFrets;

            Assert.IsNotNull(fingering);
            Assert.IsNotNull(copy);
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void EqualsTest()
        {
            GuitarFingering fingering1 = new(["C", "C#", "", "D", "E", "F"]);
            fingering1.Frets = [8, 2, -1, 0, 7, 1];
            fingering1.DistanceBetweenExtremeFrets = fingering1.CalculateDistanceBetweenExtremeFrets();

            GuitarFingering copy = new(fingering1);
            GuitarFingering fingering2 = new(["C", "C#", "A", "D", "E", "F"]);
            fingering2.Frets = [8, 2, -1, 0, 7, 1];
            GuitarFingering fingering3 = new(["C", "C#", "", "D", "E", "F"]);
            fingering3.Frets = [8, 2, -1, 12, 7, 13];

            Assert.IsTrue(fingering1.Equals(copy));
            Assert.IsFalse(fingering2.Equals(copy));
            Assert.IsFalse(fingering3.Equals(copy));
        }

        [TestMethod]
        public void GetHashCodeTest()
        {
            GuitarFingering fingering1 = new(["C", "C#", "", "D", "E", "F"]);
            fingering1.Frets = [8, 2, -1, 0, 7, 1];
            fingering1.NotesMidiIndexes = [41, 52, 50, 61, 72];
            fingering1.DistanceBetweenExtremeFrets = fingering1.CalculateDistanceBetweenExtremeFrets();

            GuitarFingering copy = new(fingering1);
            GuitarFingering fingering2 = new(fingering1);
            fingering2.Frets = [8, 0, -1, 0, 7, 1];

            Assert.AreEqual(fingering1.GetHashCode(), copy.GetHashCode());
            Assert.AreNotEqual(fingering2.GetHashCode(), copy.GetHashCode());
        }


        #region Тесты для аппликатур отдельных аккордов
        [TestMethod]
        public void MajorChordFingeringTest()
        {
            RootNote root = new("C");
            List<Note> chordNotes = [root,
                new AddedNote("E", root),
                new AddedNote("G", root)];
            Chord chord = new(chordNotes);

            int[] frets = [-1, -1, -1, 2, 10, 8];
            bool areEqual = frets.SequenceEqual(chord.GuitarFingerings.ToArray()[0].Frets);

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void MinorChordFingeringTest()
        {
            RootNote root = new("C");
            List<Note> chordNotes = [root,
                new AddedNote("D#", root),
                new AddedNote("G", root)];
            Chord chord = new(chordNotes);

            int[] frets = [-1, -1, -1, 1, 10, 8];
            bool areEqual = frets.SequenceEqual(chord.GuitarFingerings.ToArray()[0].Frets);

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void AugmentedChordFingeringTest()
        {
            RootNote root = new("C");
            List<Note> chordNotes = [root,
                new AddedNote("E", root),
                new AddedNote("G#", root)];
            Chord chord = new(chordNotes);

            int[] frets = [-1, -1, -1, 2, 11, 8];
            bool areEqual = frets.SequenceEqual(chord.GuitarFingerings.ToArray()[0].Frets);

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void DiminishedChordFingeringTest()
        {
            RootNote root = new("C");
            List<Note> chordNotes = [root,
                new AddedNote("D#", root),
                new AddedNote("F#", root)];
            Chord chord = new(chordNotes);

            int[] frets = [-1, -1, -1, 1, 9, 8];
            bool areEqual = frets.SequenceEqual(chord.GuitarFingerings.ToArray()[0].Frets);

            Assert.IsTrue(areEqual);
        }
        #endregion
    }
}