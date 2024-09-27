namespace OptionA.Composer.Components.Sheet
{
    // helper class for getting all the required js keydown event properties in .Net
    public class KeyEvent
    {
        public bool AltKey { get; set; }
        public bool CtrlKey { get; set; }
        public int KeyCode { get; set; }
        public bool ShiftKey { get; set; }
        public string Key { get; set; } = string.Empty;
    }
}
