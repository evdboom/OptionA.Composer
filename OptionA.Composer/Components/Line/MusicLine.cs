using OptionA.Composer.Components.Sheet;
using OptionA.Composer.Structs;
using System.Diagnostics.CodeAnalysis;

namespace OptionA.Composer.Components
{
    public class MusicLine
    {
        public MusicLine(char key, int barsPerLine, int beatsPerBar, NoteLength noteLength)
        {
            Modifiers = [];            
            Key = key;
            BarsPerLine = barsPerLine;
            BeatsPerBar = beatsPerBar;
            DefaultLength = noteLength;
            Bars = [new MusicBar(this, BeatsPerBar, DefaultLength)];
        }

        public MusicLine(MusicLine source) : this(source.Key, source.BarsPerLine, source.BeatsPerBar, source.DefaultLength)
        {
            Modifiers = [.. source.Modifiers];
        }

        public IList<(char Note, NoteModifier Modifier)> Modifiers { get; set; }
        public IList<MusicBar> Bars { get; set; }
        public char Key { get; set; }
        public int BarsPerLine { get; set; }
        public int BeatsPerBar { get; set; }
        public NoteLength DefaultLength { get; set; }

        public bool TryAddNote(int barIndex, Note note, out int newBarIndex, out int noteIndex)
        {
            var bar = Bars[barIndex];
            if (!bar.TryAddNote(note, DefaultLength, out noteIndex))
            {
                if (BarsPerLine == Bars.Count)
                {
                    newBarIndex = 0;
                    return false;
                }

                barIndex++;
                Bars.Insert(barIndex, new MusicBar(bar));
                if (!Bars[barIndex].TryAddNote(note, DefaultLength, out noteIndex))
                {
                    newBarIndex = 0;
                    return false;
                }                
            }

            newBarIndex = barIndex;
            return true;
        }
    }
}
