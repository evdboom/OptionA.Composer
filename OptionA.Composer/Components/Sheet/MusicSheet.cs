using OptionA.Composer.Structs;

namespace OptionA.Composer.Components.Sheet
{
    public class MusicSheet
    {
        public MusicSheet()
        {
            Lines = [new MusicLine('F', 4, 4, NoteLength.Quarter)];
        }

        public IList<MusicLine> Lines { get; set; }
        public IEnumerable<MusicNote> SelectedMusicNotes => Lines
            .SelectMany(line => line.Bars
                .SelectMany(bar => bar.Notes
                    .Where(note => note.Selected)));

        public (int LineIndex, int BarIndex, int NoteIndex) AddNote(int lineIndex, int barIndex, Note note)
        {
            var line = Lines[lineIndex];
            if (!line.TryAddNote(barIndex, note, out int newBarIndex, out int noteIndex))
            {
                barIndex = 0;
                lineIndex++;
                Lines.Insert(lineIndex, new MusicLine(line));                
                Lines[lineIndex].TryAddNote(barIndex, note, out newBarIndex, out noteIndex);
            }

            return (lineIndex, newBarIndex, noteIndex);
        }

        
    }
}
