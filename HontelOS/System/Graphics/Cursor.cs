﻿namespace HontelOS.System.Graphics
{
    public class Cursors
    {
        //Format
        // 
        //OrginX,OrginY,
        //Width, Height,
        //Colors,,,,,

        public static int[] defaultCursor = new int[]
        {
            0,0,
            12,19,
            1,0,0,0,0,0,0,0,0,0,0,0,
            1,1,0,0,0,0,0,0,0,0,0,0,
            1,2,1,0,0,0,0,0,0,0,0,0,
            1,2,2,1,0,0,0,0,0,0,0,0,
            1,2,2,2,1,0,0,0,0,0,0,0,
            1,2,2,2,2,1,0,0,0,0,0,0,
            1,2,2,2,2,2,1,0,0,0,0,0,
            1,2,2,2,2,2,2,1,0,0,0,0,
            1,2,2,2,2,2,2,2,1,0,0,0,
            1,2,2,2,2,2,2,2,2,1,0,0,
            1,2,2,2,2,2,2,2,2,2,1,0,
            1,2,2,2,2,2,2,2,2,2,2,1,
            1,2,2,2,2,1,1,1,1,1,1,1,
            1,2,2,2,1,0,0,0,0,0,0,0,
            1,2,2,1,0,0,0,0,0,0,0,0,
            1,2,1,0,0,0,0,0,0,0,0,0,
            1,1,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0
        };
        public static int[] textCursor = new int[]
        {
            6,9,
            12,19,
            0,1,1,1,1,1,1,1,1,1,1,0,
            1,2,2,2,2,2,2,2,2,2,2,1,
            1,2,2,2,2,2,2,2,2,2,2,1,
            0,1,1,1,1,2,2,1,1,1,1,0,
            0,0,0,0,1,2,2,1,0,0,0,0,
            0,0,0,0,1,2,2,1,0,0,0,0,
            0,0,0,0,1,2,2,1,0,0,0,0,
            0,0,0,0,1,2,2,1,0,0,0,0,
            0,0,0,0,1,2,2,1,0,0,0,0,
            0,0,0,0,1,2,2,1,0,0,0,0,
            0,0,0,0,1,2,2,1,0,0,0,0,
            0,0,0,0,1,2,2,1,0,0,0,0,
            0,0,0,0,1,2,2,1,0,0,0,0,
            0,0,0,0,1,2,2,1,0,0,0,0,
            0,0,0,0,1,2,2,1,0,0,0,0,
            0,1,1,1,1,2,2,1,1,1,1,0,
            1,2,2,2,2,2,2,2,2,2,2,1,
            1,2,2,2,2,2,2,2,2,2,2,1,
            0,1,1,1,1,1,1,1,1,1,1,0
        };

        public static int[] GetCursorRawData(Cursor cursor)
        {
            switch (cursor)
            {
                default:
                    return defaultCursor;
                case Cursor.Default:
                    return defaultCursor;
                case Cursor.Text:
                    return textCursor;
            }
        }
    }

    public enum Cursor
    {
        Default,
        Text
    }
}
