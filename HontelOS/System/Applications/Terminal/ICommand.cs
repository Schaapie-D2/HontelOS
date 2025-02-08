/*
* PROJECT:          HontelOS
* CONTENT:          Terminal command interface
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

namespace HontelOS.System.Applications.Terminal
{
    public interface ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal);
        public string GetHelpText();
        public string GetName();
    }
}
