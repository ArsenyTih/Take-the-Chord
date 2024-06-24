using ChordRandomizer;

namespace ChordRandomizerTests
{
    [TestClass]
    public class ChordTests
    {
        [TestMethod]
        public void ChordConstructorTest()
        {
            Chord chord = new();

            Assert.IsNotNull(chord);
            Assert.AreNotEqual("", chord.ChordName);
            Assert.IsTrue(chord.NumOfNotes > 0);
            Assert.AreEqual(chord.NumOfNotes, chord.ChordNotes.Count);
            Assert.AreEqual(chord.ChordNotes.Count, chord.ChordNotesIndexes.Count);
            Assert.AreEqual(chord.NumOfNotes - 1, chord.addedNotesOctaveIndexes.Count);
            Assert.IsNotNull(chord.GuitarFingerings);
        }

        [TestMethod]
        public void ChordNumOfNotesConstructorTest()
        {
            Chord chord = new(3);

            Assert.IsNotNull(chord);
            Assert.AreNotEqual("", chord.ChordName);
            Assert.AreEqual(3, chord.NumOfNotes);
            Assert.AreEqual(3, chord.ChordNotes.Count);
            Assert.AreEqual(3, chord.ChordNotesIndexes.Count);
            Assert.AreEqual(2, chord.addedNotesOctaveIndexes.Count);
            Assert.IsNotNull(chord.GuitarFingerings);
        }

        [TestMethod]
        public void ChordNotesConstructorTest()
        {
            RootNote root = new("C");
            List<Note> chordNotes = [root, new AddedNote("E", root), new AddedNote("G", root)];
            Chord chord = new(chordNotes);

            Assert.IsNotNull(chord);
            Assert.AreNotEqual("", chord.ChordName);
            Assert.AreEqual(3, chord.NumOfNotes);
            Assert.AreEqual(3, chord.ChordNotes.Count);
            Assert.AreEqual(3, chord.ChordNotesIndexes.Count);
            Assert.AreEqual(2, chord.addedNotesOctaveIndexes.Count);
            Assert.IsNotNull(chord.GuitarFingerings);
        }

        [TestMethod]
        public void NumofNotesExceptionTest()
        {
            Chord chord;

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => chord = new(0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => chord = new(1));
        }

        [TestMethod]
        public void ChordNotesExceptionTest()
        {
            Chord chord = new(3);

            Assert.ThrowsException<ArgumentNullException>(() => chord.ChordNotes = null);
            Assert.ThrowsException<ArgumentException>(() => chord.ChordNotes = new(4));
        }

        [TestMethod]
        public void ChordNotesIndexesExceptionTest()
        {
            Chord chord = new(3);

            Assert.ThrowsException<ArgumentNullException>(() => chord.ChordNotesIndexes = null);
            Assert.ThrowsException<ArgumentException>(() => chord.ChordNotesIndexes = new(4));
        }

        [TestMethod]
        public void GuitarFingeringsExceptionTest()
        {
            Chord chord = new(3);

            Assert.ThrowsException<ArgumentNullException>(() => chord.GuitarFingerings = null);
        }

        [TestMethod]
        public void ChooseRootNoteTest()
        {
            Chord chord = new(2);

            Assert.IsTrue(MusicalNotes.Notes.Contains(chord.ChordNotes[0].NoteName));
        }

        [TestMethod]
        public void ChooseRootNoteExceptionTest()
        {
            List<string> temp = new(MusicalNotes.AllowedRootNotes);
            try
            {
                MusicalNotes.AllowedRootNotes.Clear();
                Chord chord = new(2);
            }
            catch (InvalidOperationException)
            {
                MusicalNotes.AllowedRootNotes = temp;
                Assert.IsTrue(true);
            }         
        }

        [TestMethod]
        public void ChooseAddedNotesTest()
        {
            List<string> temp = new(MusicalNotes.AllowedRootNotes);
            Chord chord = new(4);

            AddedNote[] addedNotes = chord.ChordNotes.Select(x => x)
                .Where(x => x is AddedNote)
                .Select(x => (AddedNote)x).ToArray();

            Assert.AreEqual(3, addedNotes.Count());
            Assert.IsTrue(addedNotes.All(x => x is not null));
        }

        [TestMethod]
        public void ChooseAddedNotesExceptionTest()
        {
            Dictionary<string, int> temp = new(MusicalNotes.AllowedAddedNotes);
            try
            {
                MusicalNotes.AllowedAddedNotes.Clear();
                Chord chord = new(4);
            }
            catch (InvalidOperationException)
            {
                MusicalNotes.AllowedAddedNotes = temp;
                Assert.IsTrue(true);
            }
        }

        #region Тесты для отдельных видов аккордов
        [TestMethod]
        public void MajorChordNameTest()
        {
            RootNote root = new("C");
            List<Note> chordNotes = [root, 
                new AddedNote("E", root), 
                new AddedNote("G", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("C", chord.ChordName);
        }

        [TestMethod]
        public void MinorChordNameTest()
        {
            RootNote root = new("C");
            List<Note> chordNotes = [root, 
                new AddedNote("D#", root), 
                new AddedNote("G", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("Cm", chord.ChordName);
        }

        [TestMethod]
        public void AugmentedChordNameTest()
        {
            RootNote root = new("C");
            List<Note> chordNotes = [root, 
                new AddedNote("E", root), 
                new AddedNote("G#", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("Caug", chord.ChordName);
        }

        [TestMethod]
        public void DiminishedChordNameTest()
        {
            RootNote root = new("C");
            List<Note> chordNotes = [root, 
                new AddedNote("D#", root), 
                new AddedNote("F#", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("Cdim", chord.ChordName);
        }

        [TestMethod]
        public void Sus2ChordNameTest()
        {
            RootNote root = new("D");
            List<Note> chordNotes = [root, 
                new AddedNote("E", root), 
                new AddedNote("A", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("Dsus2", chord.ChordName);
        }

        [TestMethod]
        public void Sus4ChordNameTest()
        {
            RootNote root = new("E");
            List<Note> chordNotes = [root, 
                new AddedNote("A", root), 
                new AddedNote("B", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("Esus4", chord.ChordName);
        }

        [TestMethod]
        public void Added9MajorChordNameTest()
        {
            RootNote root = new("D");
            List<Note> chordNotes = [root,
                new AddedNote("F#", root),
                new AddedNote("A", root),
                new AddedNote("E", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("Dadd9", chord.ChordName);
        }

        [TestMethod]
        public void Added11MajorChordNameTest()
        {
            RootNote root = new("D");
            List<Note> chordNotes = [root,
                new AddedNote("F#", root),
                new AddedNote("A", root),
                new AddedNote("G", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("Dadd11", chord.ChordName);
        }

        [TestMethod]
        public void No3ChordNameTest()
        {
            RootNote root = new("E");
            List<Note> chordNotes = [root, new AddedNote("B", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("E(no3)", chord.ChordName);
        }

        [TestMethod]
        public void No5ChordNameTest()
        {
            RootNote root = new("E");
            List<Note> chordNotes = [root, new AddedNote("G#", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("E(no5)", chord.ChordName);
        }

        [TestMethod]
        public void Added6thChordNameTest()
        {
            RootNote root = new("G");
            List<Note> chordNotes = [root,
                new AddedNote("B", root),
                new AddedNote("D", root),
                new AddedNote("E", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("G6", chord.ChordName);
        }


        [TestMethod]
        public void Added7thChordNameTest()
        {
            RootNote root = new("G");
            List<Note> chordNotes = [root,
                new AddedNote("B", root),
                new AddedNote("D", root),
                new AddedNote("F", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("G7", chord.ChordName);
        }

        [TestMethod]
        public void ChordWithMaj7thNameTest()
        {
            RootNote root = new("G");
            List<Note> chordNotes = [root,
                new AddedNote("B", root),
                new AddedNote("D", root),
                new AddedNote("F#", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("Gmaj7", chord.ChordName);
        }

        [TestMethod]
        public void ChordWith9thNameTest()
        {
            RootNote root = new("C#");
            List<Note> chordNotes = [root,
                new AddedNote("D#", root),
                new AddedNote("F", root),
                new AddedNote("G#", root),
                new AddedNote("B", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("C#9", chord.ChordName);
        }

        [TestMethod]
        public void ChordWith11thNameTest()
        {
            RootNote root = new("C#");
            List<Note> chordNotes = [root,
                new AddedNote("F", root),
                new AddedNote("F#", root),
                new AddedNote("G#", root),
                new AddedNote("B", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("C#11", chord.ChordName);
        }

        [TestMethod]
        public void ChordWith13thNameTest()
        {
            RootNote root = new("D");
            List<Note> chordNotes = [root,
                new AddedNote("F", root),
                new AddedNote("A", root),
                new AddedNote("B", root),
                new AddedNote("C", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("Dm13", chord.ChordName);
        }

        [TestMethod]
        public void ChordWithMaj13thNameTest()
        {
            RootNote root = new("D");
            List<Note> chordNotes = [root,
                new AddedNote("F", root),
                new AddedNote("A", root),
                new AddedNote("C", root),
                new AddedNote("C#", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("Dmmaj13", chord.ChordName);
        }

        [TestMethod]
        public void Addedb2ChordNameTest()
        {
            RootNote root = new("D");
            List<Note> chordNotes = [root,
                new AddedNote("F", root),
                new AddedNote("A", root),
                new AddedNote("D#", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("Dmadd(2-)", chord.ChordName);
        }

        [TestMethod]
        public void Added4plusChordNameTest()
        {
            RootNote root = new("A");
            List<Note> chordNotes = [root,
                new AddedNote("C", root),
                new AddedNote("E", root),
                new AddedNote("D#", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("Amadd(4+)", chord.ChordName);
        }

        [TestMethod]
        public void Addedb5ChordNameTest()
        {
            RootNote root = new("A");
            List<Note> chordNotes = [root,
                new AddedNote("C#", root),
                new AddedNote("D#", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("A5-", chord.ChordName);
        }

        [TestMethod]
        public void Added5plusChordNameTest()
        {
            RootNote root = new("A");
            List<Note> chordNotes = [root,
                new AddedNote("C", root),
                new AddedNote("F", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("Am5+", chord.ChordName);
        }

        [TestMethod]
        public void Addedb6ChordNameTest()
        {
            RootNote root = new("A");
            List<Note> chordNotes = [root,
                new AddedNote("C", root),
                new AddedNote("E", root),
                new AddedNote("F", root)];
            Chord chord = new(chordNotes);

            Assert.AreEqual("Amadd(6-)", chord.ChordName);
        }
        #endregion
    }
}