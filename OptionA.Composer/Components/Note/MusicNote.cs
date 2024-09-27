using OptionA.Composer.Structs;

namespace OptionA.Composer.Components
{
    public class MusicNote
    {
        public NoteModifier Modifier { get; set; }
        public NoteLength Length { get; set; }
        public char Note { get; set; }     
        public int? FingerPosition { get; set; }
        public bool IsFlageolet { get; set; }
        public int Octave { get; set; }
        public bool IsExtended { get; set; }
        public bool IsRest { get; set; }
        public bool IsStaccato { get; set; }
    }
}
