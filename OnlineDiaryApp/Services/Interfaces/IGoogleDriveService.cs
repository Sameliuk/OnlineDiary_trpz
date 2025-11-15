namespace OnlineDiaryApp.Services.Interfaces
{
    public interface IGoogleDriveService
    {
       
        Task<string> UploadFileAsync(IFormFile file);

        Task<MemoryStream> DownloadFileAsync(string fileId);

        Task DeleteFileAsync(string fileId);
    }
}
