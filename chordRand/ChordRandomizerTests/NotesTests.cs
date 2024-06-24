using ChordRandomizer;

namespace ChordRandomizerTests
{
    [TestClass]
    public class NotesTests
    {
        #region Note class tests
        [TestMethod]
        public void NoteConstructorTest()
        {
            Note note = new();

            Assert.IsNotNull(note);
            Assert.AreEqual("", note.NoteName);
            Assert.AreEqual(0, note.Index);
        }

        [TestMethod]
        public void NoteNameConstructorTest()
        {
            Note note = new("C#");

            Assert.IsNotNull(note);
            Assert.AreEqual("C#", note.NoteName);
            Assert.AreEqual(1, note.Index);
        }

        [TestMethod]
        public void NoteIndexConstructorTest3()
        {
            Note note = new(4);

            Assert.IsNotNull(note);
            Assert.AreEqual("E", note.NoteName);
            Assert.AreEqual(4, note.Index);
        }

        [TestMethod]
        public void NoteNameConstructorExceptionTest()
        {
            Note note = new();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => note = new("Not a note"));
        }

        [TestMethod]
        public void NoteIndexConstructorExceptionTest()
        {
            Note note = new();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => note = new(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => note = new(12));
        }

        [TestMethod]
        public void NoteToStringTest()
        {
            Note note = new("A#");

            Assert.IsNotNull(note);
            Assert.AreEqual("A#", note.NoteName);
            Assert.AreEqual("A#", note.ToString());
        }
        #endregion

        #region RootNote class tests
        [TestMethod]
        public void RootNoteNameConstructorTest()
        {
            RootNote root = new("D");

            string[] octave = ["D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B", "C", "C#"];
            bool areEqual = octave.SequenceEqual(root.OctaveFromRoot);

            Assert.IsNotNull(root);
            Assert.AreEqual("D", root.NoteName);
            Assert.AreEqual(2, root.Index);
            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void RootNoteIndexConstructorTest3()
        {
            RootNote root = new(6);

            string[] expected = ["F#", "G", "G#", "A", "A#", "B", "C", "C#", "D", "D#", "E", "F"];
            bool areEqual = expected.SequenceEqual(root.OctaveFromRoot);

            Assert.IsNotNull(root);
            Assert.AreEqual("F#", root.NoteName);
            Assert.AreEqual(6, root.Index);
            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void OctaveFromRootExceptionTest()
        {
            RootNote root = new(5);

            Assert.AreEqual(12, root.OctaveFromRoot.Length);
            Assert.ThrowsException<ArgumentException>(() => root.OctaveFromRoot = new string[13]);
            Assert.ThrowsException<ArgumentException>(() => root.OctaveFromRoot = new string[11]);
        }
        #endregion

        #region AddedNote class tests
        [TestMethod]
        public void AddedNoteNameConstructorTest()
        {
            RootNote root = new("F#");
            AddedNote addedNote = new("D", root);

            Assert.IsNotNull(addedNote);
            Assert.AreEqual("D", addedNote.NoteName);
            Assert.AreEqual(2, addedNote.Index);
            Assert.IsNotNull(addedNote.RelatedRootNote);
            Assert.AreEqual("F#", addedNote.RelatedRootNote.NoteName);
            Assert.AreEqual(8, addedNote.OctaveIndex);
        }

        [TestMethod]
        public void RelatedRootNoteConstructorTest3()
        {
            RootNote root = new("F#");
            AddedNote addedNote = new("D", root);

            bool isRemoved = MusicalNotes.AllowedRootNotes.Remove("F");

            Assert.IsNotNull(addedNote.RelatedRootNote);
            Assert.ThrowsException<ArgumentNullException>(() => addedNote.RelatedRootNote = null);
            Assert.IsTrue(isRemoved);
            Assert.ThrowsException<ArgumentException>(() => addedNote.RelatedRootNote = new("F"));
        }

        [TestMethod]
        public void OctaveIndexExceptionTest()
        {
            AddedNote addedNote = new("G", new RootNote("F#"));

            Assert.IsNotNull(addedNote.RelatedRootNote);
            Assert.AreEqual(12, addedNote.RelatedRootNote.OctaveFromRoot.Length);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => addedNote.OctaveIndex = -1);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => addedNote.OctaveIndex = 0);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => addedNote.OctaveIndex = 12);
        }
        #endregion
    }
}