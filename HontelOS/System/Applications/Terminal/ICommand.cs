namespace HontelOS.System.Applications.Terminal
{
    public interface ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal);
        public string GetHelpText();
        public string GetCMDName();
    }
}
