using Microsoft.AspNetCore.Components;

namespace OptionA.Composer.Components
{
    public partial class OptAMusicNote
    {
        [Parameter]
        public MusicNote? Note { get; set; }
    }
}