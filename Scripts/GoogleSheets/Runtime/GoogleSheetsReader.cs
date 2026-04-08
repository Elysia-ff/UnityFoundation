using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4.Data;
using UnityEngine;

namespace Elysia.GoogleSheets
{
    public class GoogleSheetsReader
    {
        public class Header
        {
            public int Index { get; }

            public string FieldName { get; }
            public string ValidatedFieldName { get; }

            public string BaseType { get; }
            public string FieldTypeName { get; }
            public EVariableType VariableType { get; }

            public bool IsComment { get; }
            public bool IsArray { get; }

            private static CodeDomProvider codeDomProvider;

            private static string GetValidatedFieldName(string fieldName)
            {
                codeDomProvider ??= CodeDomProvider.CreateProvider("C#");

                return codeDomProvider.IsValidIdentifier(fieldName) ? fieldName : $"@{fieldName}";
            }

            private static EVariableType GetVariableType(string typeName)
            {
                switch (typeName)
                {
                    case "bool":
                    case "byte":
                    case "sbyte":
                    case "char":
                    case "decimal":
                    case "double":
                    case "float":
                    case "int":
                    case "uint":
                    case "long":
                    case "ulong":
                    case "short":
                    case "ushort":
                        return EVariableType.Primitive;

                    case "string":
                        return EVariableType.String;
                }

                return !string.IsNullOrEmpty(typeName) && System.AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetType(typeName)?.IsEnum ?? false)
                    ? EVariableType.Enum
                    : EVariableType.Custom;
            }

            public Header(int index, string fieldName, string typeName)
            {
                IsComment = fieldName.StartsWith("#");
                IsArray = typeName.EndsWith("[]");

                Index = index;

                FieldName = fieldName.TrimStart('#');
                ValidatedFieldName = GetValidatedFieldName(fieldName);

                string qualifiedTypeName = typeName.TrimEnd('[', ']');
                BaseType = qualifiedTypeName.Replace('+', '.');
                FieldTypeName = IsArray ? $"IReadOnlyList<{BaseType}>" : BaseType;
                VariableType = GetVariableType(qualifiedTypeName);
            }
        }

        public int LineCount => _data.Count - _lineStartAt;

        private readonly string _id;
        private readonly string _range;

        private IList<IList<object>> _data;
        private int _lineIndex;
        private readonly int _lineStartAt;

        public GoogleSheetsReader(string id, string range, int lineStartAt)
        {
            _id = id;
            _range = range;
            _lineIndex = lineStartAt;
            _lineStartAt = lineStartAt;
        }

        public void Load()
        {
            _data = GoogleSheetsAPI.Load(_id, _range).Values;

            Debug.Assert(_data.Count > _lineStartAt);
            Debug.Assert(_data[0].Count > 0);
        }

        public async Task LoadAsync()
        {
            ValueRange result = await GoogleSheetsAPI.LoadAsync(_id, _range);
            _data = result.Values;

            Debug.Assert(_data.Count > _lineStartAt);
            Debug.Assert(_data[0].Count > 0);
        }

        public Header[] GetHeaders()
        {
            int columns = _data[0].Count;

            GoogleSheetsRow fieldNames = new GoogleSheetsRow(_data[0], columns);
            GoogleSheetsRow typeNames = new GoogleSheetsRow(_data[1], columns);

            Header[] headers = new Header[columns];
            for (int i = 0; i < columns; i++)
            {
                headers[i] = new Header(i, fieldNames[i], typeNames[i]);
            }

            return headers;
        }

        public GoogleSheetsRow GetDefaultValues()
        {
            return new GoogleSheetsRow(_data[2], _data[0].Count);
        }

        public GoogleSheetsRow ReadLine()
        {
            return new GoogleSheetsRow(_data[_lineIndex++], _data[0].Count);
        }

        public void SkipLine()
        {
            _lineIndex++;
        }
    }
}
