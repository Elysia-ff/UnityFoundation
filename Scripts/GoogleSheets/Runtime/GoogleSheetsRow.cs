using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.GoogleSheets
{
    public struct GoogleSheetsRow
    {
        private readonly IList<object> _data;

        private int _pos;
        public int Position => _pos;

        public int Count { get; }

        public string this[int i] => GetValue(i);

        public GoogleSheetsRow(IList<object> data, int count)
        {
            Debug.Assert(data != null);

            _data = data;
            _pos = 0;

            Count = count;
        }

        public void SkipField()
        {
            _pos++;
        }

        public Field ReadField()
        {
            string value = GetValue(_pos);
            _pos++;

            return new Field(value);
        }

        public Field ReadField(string defaultValue)
        {
            string value = GetValue(_pos);
            _pos++;

            return new Field(!string.IsNullOrEmpty(value) ? value : defaultValue);
        }

        public Field ReadField(int index, string defaultValue)
        {
            string value = GetValue(index);

            return new Field(!string.IsNullOrEmpty(value) ? value : defaultValue);
        }

        private string GetValue(int pos)
        {
            if (pos >= _data.Count && pos < Count)
            {
                return string.Empty;
            }

            return (string)_data[pos];
        }
    }
}
