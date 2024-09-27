using OptionA.Composer.Structs;

namespace OptionA.Composer.Components
{
    public class MusicBar
    {
        public MusicBar(int maximumLength, NoteLength length)
        {
            Notes = [];
            MaximumLength = maximumLength;
            Length = length;
        }

        public MusicBar(MusicBar source) : this(source.MaximumLength, source.Length)
        {
        }

        public int MaximumLength { get; set; }
        public NoteLength Length { get; set; }
        public IList<MusicNote> Notes { get; set; }

        public bool TryAddNote(char note, NoteLength length, out int noteIndex)
        {
            if (!CanAddFull(length, out NoteLength maximumLength))
            {
                if (maximumLength == NoteLength.None)
                {
                    noteIndex = 0;
                    return false;
                }

                length = maximumLength;
            }

            var newNote = new MusicNote
            {
                Note = note,
                Length = length
            };

            Notes.Add(newNote);
            noteIndex = Notes.Count - 1;
            return true;
        }

        private bool CanAddFull(NoteLength length, out NoteLength maximumLength)
        {
            int currentLength = Notes.Sum(note => (int)note.Length);
            int remainingLength = (MaximumLength * (int)Length) - currentLength;

            NoteLength[] validLengths = Enum.GetValues(typeof(NoteLength))
                                            .Cast<NoteLength>()
                                            .Where(l => (int)l <= remainingLength && l != NoteLength.None)
                                            .OrderByDescending(l => (int)l)
                                            .ToArray();

            if (!validLengths.Any())
            {
                maximumLength = NoteLength.None;
                return false;
            }

            if ((int)length > remainingLength)
            {
                maximumLength = validLengths.First();
                return false;
            }

            maximumLength = length;
            return true;
        }

    }
}
