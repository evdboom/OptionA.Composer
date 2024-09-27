using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

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
        internal static void KeyDown(string key)
        {
            KeyDownPressed?.Invoke(null, key);
        }

        private static event EventHandler<string>? KeyDownPressed;
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

        private void OnKeyDown(object? sender, string e)
        {
            if (_musicSheet is not null)
            {
                var (lineIndex, barIndex, noteIndex) = _musicSheet.AddNote(_currentLine, _currentBar, e[0]);
                _currentLine = lineIndex;
                _currentBar = barIndex;
                _currentNote = noteIndex;
                StateHasChanged();
            }            
        }

        public async void Dispose()
        {
            await DisposeHandler();
            KeyDownPressed -= OnKeyDown;
        }
        



    }
}