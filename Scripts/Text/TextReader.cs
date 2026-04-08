using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public abstract class TextReader : IDisposable
    {
        public bool EndOfStream => _stream.Position == _stream.Length;

        protected long OriginPosition { get; }
        protected long Position => _stream.Position;

        private readonly System.IO.MemoryStream _stream;
        private readonly System.Text.Decoder _decoder;
        private readonly byte[] _streamBuffer = new byte[4];
        private readonly char[] _charBuffer = new char[1];

        private static readonly byte[] UTF8_BYTES_COUNT = new byte[256] {
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, // 0x00-0x0F: ASCII
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, // 0x10-0x1F: ASCII
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, // 0x20-0x2F: ASCII
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, // 0x30-0x3F: ASCII
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, // 0x40-0x4F: ASCII
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, // 0x50-0x5F: ASCII
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, // 0x60-0x6F: ASCII
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, // 0x70-0x7F: ASCII
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, // 0x80-0x8F: 잘못된 UTF-8
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, // 0x90-0x9F: 잘못된 UTF-8
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, // 0xA0-0xAF: 잘못된 UTF-8
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, // 0xB0-0xBF: 잘못된 UTF-8
            2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2, // 0xC0-0xCF: 2바이트 시작
            2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2, // 0xD0-0xDF: 2바이트 시작
            3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3, // 0xE0-0xEF: 3바이트 시작
            4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0  // 0xF0-0xF7: 4바이트 시작, 0xF8-0xFF: 잘못된 UTF-8
        };

        public TextReader(TextAsset textAsset)
            : this(textAsset.bytes)
        {
        }

        public TextReader(byte[] bytes)
        {
            _stream = new System.IO.MemoryStream(bytes, false);
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            _decoder = encoding.GetDecoder();

            bool hasBOM = _stream.ReadByte() == 0xEF && _stream.ReadByte() == 0xBB && _stream.ReadByte() == 0xBF;
            if (!hasBOM)
            {
                OriginPosition = 0;
                _stream.Seek(0, System.IO.SeekOrigin.Begin);
            }
            else
            {
                OriginPosition = 3;
            }
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        protected void SeekFromBegin(long offset)
        {
            _stream.Seek(offset, System.IO.SeekOrigin.Begin);
        }

        protected void SeekFromCurrent(long offset)
        {
            _stream.Seek(offset, System.IO.SeekOrigin.Current);
        }

        protected void SeekFromEnd(long offset)
        {
            _stream.Seek(offset, System.IO.SeekOrigin.End);
        }

        private bool TryPeek(out int outByteCount)
        {
            int num = _stream.ReadByte();
            if (num == -1)
            {
                outByteCount = 0;
                return false;
            }

            byte b = (byte)num;
            outByteCount = UTF8_BYTES_COUNT[b];

            _stream.Seek(-1, System.IO.SeekOrigin.Current);

            return true;
        }

        protected int Read()
        {
            if (!TryPeek(out int byteCount))
            {
                return -1;
            }

            int count = _stream.Read(_streamBuffer, 0, byteCount);
            Debug.Assert(count == byteCount);

            _decoder.GetChars(_streamBuffer, 0, byteCount, _charBuffer, 0);

            return _charBuffer[0];
        }

        protected int Peek()
        {
            if (!TryPeek(out int byteCount))
            {
                return -1;
            }

            int count = _stream.Read(_streamBuffer, 0, byteCount);
            Debug.Assert(count == byteCount);

            _decoder.GetChars(_streamBuffer, 0, byteCount, _charBuffer, 0);

            _stream.Seek(-byteCount, System.IO.SeekOrigin.Current);

            return _charBuffer[0];
        }

        protected void Rewind()
        {
            while (Position > OriginPosition)
            {
                SeekFromCurrent(-1);

                if (TryPeek(out int byteCount) && byteCount > 0)
                {
                    break;
                }
            }
        }
    }
}
