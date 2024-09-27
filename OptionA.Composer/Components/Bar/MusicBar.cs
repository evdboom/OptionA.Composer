using OptionA.Composer.Structs;

namespace OptionA.Composer.Components
{
    public class MusicBar(MusicLine? parent, int beats, NoteLength beatLength)
    {
        public MusicBar(MusicBar source) : this(source.Parent, source.Beats, source.BeatLength)
        {
        }

        public MusicLine? Parent { get; set; } = parent;

        public int Beats { get; set; } = beats;
        public NoteLength BeatLength { get; set; } = beatLength;
        public IList<MusicNote> Notes { get; set; } = [];
        public bool IsFull => Notes.Sum(note => (note.IsExtended ? (double)note.Length * 1.5d : (double)note.Length)) - (Beats * (double)BeatLength) < double.Epsilon;

        public bool TryAddNote(Note note, NoteLength length, out int noteIndex)
        {
            if (!CanAdd(length, false, 0, out NoteLength maximumLength))
            {
                if (maximumLength == NoteLength.None)
                {
                    noteIndex = 0;
                    return false;
                }

                length = maximumLength;
            }

            var newNote = new MusicNote(this)
            {
                Note = note,
                Length = length,
                Selected = true
            };

            Notes.Add(newNote);
            noteIndex = Notes.Count - 1;
            return true;
        }

        internal bool AllowIncrease(MusicNote musicNote, NoteLength increasedLength, bool extended)
        {
            return CanAdd(increasedLength, extended, (int)musicNote.Length + (musicNote.IsExtended ? (int)musicNote.Length / 2 : 0), out _);
        }

        private bool CanAdd(NoteLength length, bool extended, int subtract, out NoteLength maximumLength)
        {
            if (length == NoteLength.SixtyFourth || length == NoteLength.Whole)
            {
                extended = false;
            }

            var currentLength = Notes.Sum(note => (int)note.Length + (note.IsExtended ? (int)note.Length / 2 : 0)) - subtract;
            var remainingLength = (Beats * (int)BeatLength) - currentLength;

            NoteLength[] validLengths = Enum.GetValues(typeof(NoteLength))
                                            .Cast<NoteLength>()
                                            .Where(l => ((int)l + (extended ? (int)l / 2 : 0)) <= remainingLength && l != NoteLength.None)
                                            .OrderByDescending(l => (int)l)
                                            .ToArray();

            if (validLengths.Length == 0)
            {
                maximumLength = NoteLength.None;
                return false;
            }

            var actualLength = (int)length + (extended ? (int)length / 2 : 0);

            if (actualLength > remainingLength)
            {
                maximumLength = validLengths[0];
                return false;
            }

            maximumLength = length;
            return true;
        }

    }
}
