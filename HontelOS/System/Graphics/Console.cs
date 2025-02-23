/*
* PROJECT:          HontelOS
* CONTENT:          Console class
* PROGRAMMERS:      Jort van Dalen
* 
* USES CODE FROM:
* https://github.com/aura-systems/Aura-Operating-System
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using Cosmos.System;
using HontelOS.System.Input;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HontelOS.System.Graphics
{
    public struct Cell
    {
        public char Char;
        public Color ForegroundColor;
        public Color BackgroundColor;
    }

    public class Console
    {
        Style style = StyleManager.Style;
        public DirectBitmap canvas;

        public bool ScrollMode = false;
        public bool CursorVisible;
        public int mX = 0;
        public int mY = 0;

        public int mCols;
        public int mRows;

        public bool IsDirty = true;
        public bool IsSelected = false;
        public bool IsReadingUserInput = false;
        public string UserInput = string.Empty;

        public List<Action<string>> OnSubmitReadLine = new();

        private Cell[] _text;
        private List<Cell[]> _terminalHistory;
        private int _terminalHistoryIndex = 0;

        public Color ForegroundColor = Color.White;
        public Color BackgroundColor = Color.Black;

        public Color ConsoleBackgroundColor = Color.Black;

        public Console(int width, int height)
        {
            InitConsole(width, height);
            SystemEvents.OnStyleChanged.Add(() => { style = StyleManager.Style; });
        }

        public void InitConsole(int width, int height)
        {
            mCols = width / style.SystemFont.Width - 1;
            mRows = height / style.SystemFont.Height - 2;

            _text = new Cell[mCols * mRows];

            canvas = new DirectBitmap(width, height);

            ClearText();

            CursorVisible = true;
            _terminalHistory = new List<Cell[]>();

            mX = 0;
            mY = 0;
        }

        public void Resize(int width, int height)
        {
            int newCols = width / style.SystemFont.Width - 1;
            int newRows = height / style.SystemFont.Height - 2;

            Cell[] newText = new Cell[newCols * newRows];

            for (int row = 0; row < Math.Min(mRows, newRows); row++)
            {
                for (int col = 0; col < Math.Min(mCols, newCols); col++)
                {
                    int oldIndex = GetIndex(row, col);
                    int newIndex = row * newCols + col;
                    newText[newIndex] = _text[oldIndex];
                }
            }

            mCols = newCols;
            mRows = newRows;
            _text = newText;

            canvas.SetSize(width, height);

            if (mX >= mCols) mX = mCols - 1;
            if (mY >= mRows) mY = mRows - 1;

            IsDirty = true;
        }


        private int GetIndex(int row, int col)
        {
            return row * mCols + col;
        }

        public void Draw()
        {
            canvas.Clear(ConsoleBackgroundColor);

            for (int i = 0; i < mRows; i++)
            {
                for (int j = 0; j < mCols; j++)
                {
                    int index = GetIndex(i, j);
                    if (_text[index].Char == 0 || _text[index].Char == '\n')
                        continue;

                    WriteByte(_text[index].Char, 0 + j * style.SystemFont.Width, 0 + i * style.SystemFont.Height, _text[index].ForegroundColor);
                }
            }

            if (IsReadingUserInput)
            {
                int startX = mX * style.SystemFont.Width;
                int startY = mY * style.SystemFont.Height;

                for (int i = 0; i < UserInput.Length; i++)
                {
                    char inputChar = UserInput[i];
                    WriteByte(inputChar, startX + i * style.SystemFont.Width, startY, ForegroundColor);
                }
            }

            DrawCursor();
        }

        public void Update()
        {
            if (IsReadingUserInput && IsSelected && KeyboardManagerExt.KeyAvailable)
            {
                var key = KeyboardManagerExt.ReadKey();

                if (key == null)
                    return;

                char keyChar = key.KeyChar;

                switch (key.Key)
                {
                    case ConsoleKeyEx.Enter:
                        WriteLine(UserInput);
                        IsReadingUserInput = false;
                        foreach (var a in OnSubmitReadLine) a.Invoke(UserInput);
                        break;

                    case ConsoleKeyEx.Backspace:
                        if (UserInput.Length > 0)
                        {
                            UserInput = UserInput.Remove(UserInput.Length - 1);
                        }
                        break;

                    default:
                        if (!char.IsControl(keyChar))
                        {
                            UserInput += keyChar;
                        }
                        break;
                }

                IsDirty = true;
            }
        }

        public void WriteByte(char ch, int mX, int mY, Color color)
        {
            canvas.DrawChar(ch, style.SystemFont, color, mX, mY);
        }

        public void SetCursorPos(int mX, int mY)
        {
            if (CursorVisible)
            {
                canvas.DrawFilledRectangle(ForegroundColor, 0 + mX * style.SystemFont.Width,
                    0 + mY * style.SystemFont.Height + style.SystemFont.Height, 8, 4);
            }
        }

        public void ClearText()
        {
            canvas.Clear(Color.Black);
            mX = 0;
            mY = 0;

            for (int i = 0; i < _text.Length; i++)
            {
                _text[i].Char = (char)0;
                _text[i].ForegroundColor = ForegroundColor;
                _text[i].BackgroundColor = BackgroundColor;
            }

            IsDirty = true;
        }

        public void DrawCursor()
        {
            SetCursorPos(mX, mY);
        }

        /// <summary>
        /// Scroll the console up and move crusor to the start of the line.
        /// </summary>
        private void DoLineFeed()
        {
            mY++;
            mX = 0;
            if (mY == mRows)
            {
                Scroll();
                mY--;
            }
        }

        private void Scroll()
        {
            canvas.Clear(Color.Black);

            Cell[] lineToHistory = new Cell[mCols];
            Array.Copy(_text, 0, lineToHistory, 0, mCols);
            _terminalHistory.Add(lineToHistory);

            Array.Copy(_text, mCols, _text, 0, (mRows - 1) * mCols);

            int startIndex = (mRows - 1) * mCols;
            for (int i = startIndex; i < startIndex + mCols; i++)
            {
                _text[i].Char = (char)0;
                _text[i].ForegroundColor = ForegroundColor;
                _text[i].BackgroundColor = BackgroundColor;
            }

            _terminalHistoryIndex = _terminalHistory.Count;
        }

        public void ScrollUp()
        {
            if (_terminalHistoryIndex > 0)
            {
                ScrollMode = true;

                _terminalHistoryIndex--;

                Array.Copy(_text, 0, _text, mCols, (mRows - 1) * mCols);

                Cell[] lineFromHistory = _terminalHistory[_terminalHistoryIndex];
                Array.Copy(lineFromHistory, 0, _text, 0, mCols);
            }

            IsDirty = true;
        }

        public void ScrollDown()
        {
            _terminalHistoryIndex = 0;

            _terminalHistory.Clear();

            ScrollMode = false;

            ClearText();
            mX = 0;
            mY = 0;
        }

        private void DoCarriageReturn()
        {
            mX = 0;
        }

        private void DoTab()
        {
            Write(' ');
            Write(' ');
            Write(' ');
            Write(' ');
        }

        /// <summary>
        /// Write char to the console.
        /// </summary>
        /// <param name="aChar">A char to write</param>
        public void Write(char aChar)
        {
            int index = GetIndex(mY, mX);
            _text[index] = new Cell() { Char = aChar, ForegroundColor = ForegroundColor, BackgroundColor = BackgroundColor };

            mX++;
            if (mX == mCols)
            {
                DoLineFeed();
            }
        }

        public void Clear()
        {
            ClearText();
            Draw();
            DrawCursor();
        }

        public void Write(uint aInt) => Write(aInt.ToString());

        public void Write(ulong aLong) => Write(aLong.ToString());

        public void WriteLine() => Write(Environment.NewLine);

        public void WriteLine(string aText) => Write(aText + Environment.NewLine);

        public void Write(string aText)
        {
            IsDirty = true;

            for (int i = 0; i < aText.Length; i++)
            {
                switch (aText[i])
                {
                    case '\n':
                        DoLineFeed();
                        break;

                    case '\r':
                        DoCarriageReturn();
                        break;

                    case '\t':
                        DoTab();
                        break;

                    /* Normal characters, simply write them */
                    default:
                        Write(aText[i]);
                        break;
                }
            }
        }

        public void ReadLine()
        {
            IsReadingUserInput = true;
            UserInput = string.Empty;
        }
    }
}