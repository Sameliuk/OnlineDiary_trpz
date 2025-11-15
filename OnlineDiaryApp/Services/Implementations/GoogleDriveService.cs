using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using OnlineDiaryApp.Services.Interfaces;

namespace OnlineDiaryApp.Services.Implementations
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly string[] _scopes = { DriveService.Scope.DriveFile, DriveService.Scope.DriveReadonly };
        private readonly string _applicationName = "OnlineDiaryApp";
        private readonly DriveService _driveService;

        public GoogleDriveService()
        {
            using var stream = new FileStream("wwwroot/credentials/credentials.json", FileMode.Open, FileAccess.Read);
            var credPath = "wwwroot/credentials/token.json";

            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                _scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;

            _driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName,
            });
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = file.FileName
            };

            using var stream = file.OpenReadStream();
            var request = _driveService.Files.Create(fileMetadata, stream, file.ContentType);
            request.Fields = "id, webViewLink, webContentLink";

            await request.UploadAsync();
            var uploadedFile = request.ResponseBody;

            return uploadedFile.WebViewLink;
        }

        public async Task<MemoryStream> DownloadFileAsync(string fileId)
        {
            var request = _driveService.Files.Get(fileId);
            var stream = new MemoryStream();
            await request.DownloadAsync(stream);
            stream.Position = 0;
            return stream;
        }

        public async Task DeleteFileAsync(string fileId)
        {
            await _driveService.Files.Delete(fileId).ExecuteAsync();
        }
    }
}
