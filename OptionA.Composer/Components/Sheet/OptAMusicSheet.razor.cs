using OptionA.Composer.Structs;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text.Json;

namespace OptionA.Composer.Components.Sheet
{
    [SupportedOSPlatform("browser")]
    public partial class OptAMusicSheet : IDisposable
    {        
        [JSImport("initialize", "MusicSheet")]
        internal static partial Task Initialize();

        [JSImport("dispose", "MusicSheet")]
        internal static partial Task DisposeHandler();

        [JSExport]
        internal static void KeyDown(string keyEvent)
        {
            var key = JsonSerializer.Deserialize<KeyEvent>(keyEvent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            KeyDownPressed?.Invoke(null, key);
        }

        private static event EventHandler<KeyEvent?>? KeyDownPressed;
        private MusicSheet? _musicSheet;

        private int _currentLine;
        private int _currentNote;
        private int _currentBar;
       
        protected override async Task OnInitializedAsync()
        {
            await JSHost.ImportAsync("MusicSheet", "../Components/Sheet/OptaMusicSheet.razor.js");
            await Initialize();
            KeyDownPressed += OnKeyDown;
            _musicSheet = new MusicSheet();
        }

        private void OnKeyDown(object? sender, KeyEvent? e)
        {
            if (e is not null && _musicSheet is not null)
            {
                if (Enum.IsDefined(typeof(Note), e.KeyCode))
                {
                    if (!e.ShiftKey && _musicSheet.SelectedMusicNotes.Any())
                    {
                        foreach (var note in _musicSheet.SelectedMusicNotes.ToList())
                        {
                            note.Selected = false;
                        }
                    }

                    var (lineIndex, barIndex, noteIndex) = _musicSheet.AddNote(_currentLine, _currentBar, (Note)e.KeyCode);
                    SetHover(false);
                    _currentLine = lineIndex;
                    _currentBar = barIndex;
                    _currentNote = noteIndex;
                    SetHover(true);
                    StateHasChanged();
                }
                else if (Enum.IsDefined(typeof(MoveIdentifier), e.KeyCode) && e.CtrlKey)
                {
                    var modifier = (MoveIdentifier)e.KeyCode;
                    Move(modifier);
                    StateHasChanged();
                }
                else if (Enum.IsDefined(typeof(EditModifier), e.KeyCode))
                {
                    var modifier = (EditModifier)e.KeyCode;

                    if (modifier == EditModifier.Select)
                    {
                        if (!e.CtrlKey)
                        {
                            foreach (var note in _musicSheet.SelectedMusicNotes.ToList())
                            {
                                note.Selected = false;
                            }
                        }

                        var current = _musicSheet.Lines[_currentLine].Bars[_currentBar].Notes[_currentNote];
                        current.Selected = !current.Selected;
                    }
                    else if (modifier == EditModifier.Remove || modifier == EditModifier.RemoveDelete)
                    {
                        foreach(var note in _musicSheet.SelectedMusicNotes.ToList())
                        {
                            if (note.Parent is not null)
                            {
                                note.Parent.Notes.Remove(note);
                                if (note.Parent.Notes.Count == 0 && note.Parent.Parent is not null)
                                {
                                    note.Parent.Parent.Bars.Remove(note.Parent);
                                    if (note.Parent.Parent.Bars.Count == 0 && _musicSheet.Lines.Count > 1)
                                    {
                                        _musicSheet.Lines.Remove(note.Parent.Parent);
                                    }
                                    note.Parent.Parent = null;
                                }
                                note.Parent = null;
                            }                                                                                    
                        }
                        if (_musicSheet.Lines.Count <= _currentLine)
                        {
                            _currentLine = 0;
                        }
                        if (_musicSheet.Lines[_currentLine].Bars.Count <= _currentBar)
                        {
                            _currentBar = 0;
                        }
                        if (_musicSheet.Lines[_currentLine].Bars[_currentBar].Notes.Count <= _currentNote)
                        {
                            _currentNote = 0;
                        }
                    }   
                    else
                    {
                        foreach (var note in _musicSheet.SelectedMusicNotes)
                        {
                            note.ApplyEdit(modifier);
                        }
                    }
                    StateHasChanged();
                }

            }
        }

        private void SetHover(bool value)
        {
            _musicSheet!.Lines[_currentLine].Bars[_currentBar].Notes[_currentNote].Hover = value;
        }

        private void Move(MoveIdentifier modifier)
        {
            SetHover(false);
            switch (modifier)
            {
                case MoveIdentifier.Up:
                    if (_currentLine > 0)
                    {
                        _currentLine--;
                    }
                    break;
                case MoveIdentifier.Down:
                    if (_currentLine < _musicSheet!.Lines.Count - 1)
                    {
                        _currentLine++;
                    }
                    break;
                case MoveIdentifier.Left:
                    if (_currentNote > 0)
                    {
                        _currentNote--;
                    }
                    else if (_currentBar > 0)
                    {
                        _currentBar--;
                        _currentNote = _musicSheet!.Lines[_currentLine].Bars[_currentBar].Notes.Count - 1;
                    }
                    else if (_currentLine > 0)
                    {
                        _currentLine--;
                        _currentBar = _musicSheet!.Lines[_currentLine].Bars.Count - 1;
                        _currentNote = _musicSheet.Lines[_currentLine].Bars[_currentBar].Notes.Count - 1;
                    }
                    break;
                case MoveIdentifier.Right:
                    if (_currentNote < _musicSheet!.Lines[_currentLine].Bars[_currentBar].Notes.Count - 1)
                    {
                        _currentNote++;
                    }
                    else if (_currentBar < _musicSheet.Lines[_currentLine].Bars.Count - 1)
                    {
                        _currentBar++;
                        _currentNote = 0;
                    }
                    else if (_currentLine < _musicSheet.Lines.Count - 1)
                    {
                        _currentLine++;
                        _currentBar = 0;
                        _currentNote = 0;
                    }
                    break;
            }
            SetHover(true);
        }        

        public async void Dispose()
        {
            await DisposeHandler();
            KeyDownPressed -= OnKeyDown;
            GC.SuppressFinalize(this);
        }
    }
}