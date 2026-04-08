using System.Collections;
using System.Collections.Generic;
using Elysia.Tables;
using UnityEngine;

namespace Elysia
{
    public class CSVReader : TextReader
    {
        private long _linePosition;

        private readonly char _delimiter;
        private readonly char _qualifier;
        private readonly System.Text.StringBuilder _buffer = new System.Text.StringBuilder(1024);

        public CSVReader(TextAsset textAsset, char delimiter = ',', char qualifier = '"')
            : base(textAsset)
        {
            _delimiter = delimiter;
            _qualifier = qualifier;

            _linePosition = Position;
        }

        public void SkipLine()
        {
            while (true)
            {
                int num = Read();
                if (num == -1)
                {
                    break;
                }

                char ch = (char)num;
                if (ch == '\r')
                {
                    if (Peek() == '\n')
                    {
                        Read();
                    }

                    break;
                }

                if (ch == '\n')
                {
                    break;
                }
            }

            _linePosition = Position;
        }

        public Field ReadField()
        {
            _buffer.Clear();

            bool meetQualifier = false;

            while (true)
            {
                int num = Read();
                if (num == -1)
                {
                    return new Field(_buffer.ToString());
                }

                char ch = (char)num;
                if (ch == _qualifier)
                {
                    meetQualifier = !meetQualifier;
                    continue;
                }

                if (ch == _delimiter && !meetQualifier)
                {
                    return new Field(_buffer.ToString());
                }

                switch (ch)
                {
                    case '\r':
                    {
                        if (Peek() == '\n')
                        {
                            Read();
                        }

                        _linePosition = Position;

                        return new Field(_buffer.ToString());
                    }

                    case '\n':
                    {
                        _linePosition = Position;

                        return new Field(_buffer.ToString());
                    }

                    default:
                    {
                        _buffer.Append(ch);
                        break;
                    }
                }
            }
        }

        public void SkipField()
        {
            bool meetQualifier = false;

            while (true)
            {
                int num = Read();
                if (num == -1)
                {
                    return;
                }

                char ch = (char)num;
                if (ch == _qualifier)
                {
                    meetQualifier = !meetQualifier;
                    continue;
                }

                if (ch == _delimiter && !meetQualifier)
                {
                    return;
                }

                switch (ch)
                {
                    case '\r':
                    {
                        if (Peek() == '\n')
                        {
                            Read();
                        }

                        _linePosition = Position;

                        return;
                    }

                    case '\n':
                    {
                        _linePosition = Position;

                        return;
                    }
                }
            }
        }

        public Field ReadField(int index)
        {
            SeekFromBegin(_linePosition);

            for (int i = 0; i < index; i++)
            {
                SkipField();
            }

            return ReadField();
        }

        public void SeekCurrentLine()
        {
            SeekFromBegin(_linePosition);
        }

        public int GetLineCount()
        {
            long position = Position;
            int count = 0;

            bool newLine = true;
            while (true)
            {
                int num = Read();
                if (num == -1)
                {
                    break;
                }

                char ch = (char)num;
                switch (ch)
                {
                    case '\r':
                    {
                        if (Peek() == '\n')
                        {
                            Read();
                        }

                        newLine = true;
                        break;
                    }

                    case '\n':
                    {
                        newLine = true;
                        break;
                    }

                    default:
                    {
                        if (newLine)
                        {
                            count++;
                            newLine = false;
                        }

                        break;
                    }
                }
            }

            SeekFromBegin(position);

            return count;
        }
    }
}
