using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Elysia.GoogleSheets
{
    public static class GoogleSheetsAPI
    {
        private static readonly SheetsService _service;

        static unsafe GoogleSheetsAPI()
        {
            TextAsset asset = Resources.Load<TextAsset>("google_sheets_credential");
            NativeArray<byte> data = asset.GetData<byte>();
            using System.IO.UnmanagedMemoryStream stream = new System.IO.UnmanagedMemoryStream((byte*)data.GetUnsafePtr(), data.Length);
            GoogleCredential credential = CredentialFactory.FromStream<ServiceAccountCredential>(stream)
                                                           .ToGoogleCredential()
                                                           .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);

            _service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });
        }

        public static ValueRange Load(string id, string range)
        {
            var request = _service.Spreadsheets.Values.Get(id, range);

            return request.Execute();
        }

        public static async Task<ValueRange> LoadAsync(string id, string range)
        {
            var request = _service.Spreadsheets.Values.Get(id, range);

            return await request.ExecuteAsync();
        }
    }
}
