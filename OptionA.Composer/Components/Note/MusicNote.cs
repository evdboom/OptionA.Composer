using OptionA.Composer.Structs;

namespace OptionA.Composer.Components
{
    public class MusicNote(MusicBar? parent)
    {
        internal static readonly Dictionary<NoteLength, NoteLength> DecreaseLength = new()
        {
            { NoteLength.Whole, NoteLength.Half },
            { NoteLength.Half, NoteLength.Quarter },
            { NoteLength.Quarter, NoteLength.Eighth },
            { NoteLength.Eighth, NoteLength.Sixteenth },
            { NoteLength.Sixteenth, NoteLength.ThirtySecond },
            { NoteLength.ThirtySecond, NoteLength.SixtyFourth },
        };

        internal static readonly Dictionary<NoteLength, NoteLength> IncreaseLength = new()
        {
            { NoteLength.Half, NoteLength.Whole },
            { NoteLength.Quarter, NoteLength.Half },
            { NoteLength.Eighth, NoteLength.Quarter },
            { NoteLength.Sixteenth, NoteLength.Eighth },
            { NoteLength.ThirtySecond, NoteLength.Sixteenth },
            { NoteLength.SixtyFourth, NoteLength.ThirtySecond },
        };

        public MusicBar? Parent { get; set; } = parent;

        public NoteModifier Modifier { get; set; }
        public NoteLength Length { get; set; }
        public Note Note { get; set; }
        public int? FingerPosition { get; set; }
        public bool IsFlageolet { get; set; }
        public int Octave { get; set; }
        public bool IsExtended { get; set; }
        public bool IsStaccato { get; set; }

        public bool Selected { get; set; }
        public bool Hover { get; set; }

        public void ApplyEdit(EditModifier modifier)
        {
            switch (modifier)
            {
                case EditModifier.Sharp:
                case EditModifier.SharpNumpad:
                    Modifier = Modifier switch
                    {
                        NoteModifier.None => NoteModifier.Sharp,
                        NoteModifier.Sharp => NoteModifier.None,
                        NoteModifier.Flat => NoteModifier.Restore,
                        _ => NoteModifier.Sharp
                    };
                    break;
                case EditModifier.Flat:
                case EditModifier.FlatNumpad:
                    Modifier = Modifier switch
                    {
                        NoteModifier.None => NoteModifier.Flat,
                        NoteModifier.Sharp => NoteModifier.Restore,
                        NoteModifier.Flat => NoteModifier.None,
                        _ => NoteModifier.Flat
                    };
                    break;
                case EditModifier.Staccato:
                    IsStaccato = !IsStaccato;
                    break;
                case EditModifier.Extend:
                    if (IsExtended)
                    {
                        IsExtended = false;
                    }
                    else if (Parent!.AllowIncrease(this, Length, !IsExtended))
                    {
                        IsExtended = !IsExtended;
                    }
                    break;
                case EditModifier.IncreaseLength:
                    if (IncreaseLength.TryGetValue(Length, out NoteLength increasedLength) && Parent!.AllowIncrease(this, increasedLength, IsExtended))
                    {
                        Length = increasedLength;
                        if (Length == NoteLength.Whole)
                        {
                            IsExtended = false;
                        }                
                    }
                    break;
                case EditModifier.DecreaseLength:
                    if (DecreaseLength.TryGetValue(Length, out NoteLength decreasedLength))
                    {
                        Length = decreasedLength;
                        if (Length == NoteLength.SixtyFourth)
                        {
                            IsExtended = false;
                        }
                    }
                    break;
                case EditModifier.UpOctave:
                    Octave++;
                    break;
                case EditModifier.DownOctave:
                    Octave--;
                    break;
                case EditModifier.SetFinger1:
                case EditModifier.SetFinger1Numpad:
                    FingerPosition = FingerPosition == 1
                        ? null
                        : 1;                    
                    break;
                case EditModifier.SetFinger2:
                case EditModifier.SetFinger2Numpad:
                    FingerPosition = FingerPosition == 2
                        ? null
                        : 2;
                    break;
                case EditModifier.SetFinger3:
                case EditModifier.SetFinger3Numpad:
                    FingerPosition = FingerPosition == 3
                        ? null
                        : 3;
                    break;
                case EditModifier.SetFinger4:
                case EditModifier.SetFinger4Numpad:
                    FingerPosition = FingerPosition == 4
                        ? null
                        : 4;
                    break;
            }
        }
    }
}
