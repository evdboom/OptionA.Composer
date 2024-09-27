using Microsoft.AspNetCore.Components;

namespace OptionA.Composer.Components
{
    public partial class OptAMusicLine
    {
        [Parameter]
        public MusicLine? Line { get; set; }
    }
}