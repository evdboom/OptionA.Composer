using Microsoft.AspNetCore.Components;

namespace OptionA.Composer.Components
{
    public partial class OptAMusicNote
    {
        [Parameter]
        public MusicNote? Note { get; set; }

        private Dictionary<string, object?> GetAttributes()
        {
            var result = new Dictionary<string, object?>();

            if (Note == null)
            {
                return result;
            }
            else 
            {
                if (Note.Selected)
                {
                    result["opta-selected"] = true;
                }
                if (Note.Hover)
                {
                    result["opta-hover"] = true;
                }
            }            
            
            return result;
        }
    }
}