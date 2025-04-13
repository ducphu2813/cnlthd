using APIApplication.Service.Interfaces;
using APIApplication.Settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace APIApplication.Service;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(acc);
    }

    //hàm upload ảnh lên cloudinary
    public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile file)
    {
        if (file.Length <= 0) return null;

        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "products"
        };
        return await _cloudinary.UploadAsync(uploadParams);
    }
    
    //hàm xóa ảnh trên cloudinary
    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        return await _cloudinary.DestroyAsync(deleteParams);
    }
    
    //hàm extract public_id từ url
    public string ExtractPublicId(string imageUrl)
    {
        //chỉ lấy phần path của url, bỏ qua domain
        var uri = new Uri(imageUrl);
        var path = uri.AbsolutePath;

        // Cắt từ sau 'upload/'
        var afterUpload = path.Substring(path.IndexOf("/upload/") + 8);

        //tách thành các phần theo dấu '/' và lưu thành mảng
        var parts = afterUpload.Split('/');
        
        // Nếu có version (bắt đầu bằng 'v' + số), bỏ qua
        if (parts[0].StartsWith("v") && long.TryParse(parts[0].Substring(1), out _))
            parts = parts.Skip(1).ToArray(); // Bỏ qua phần tử đầu tiên trong mảng

        // Ghép lại và bỏ đuôi mở rộng (.jpg, .png...)
        var publicIdWithExt = string.Join("/", parts);
        return Path.Combine(Path.GetDirectoryName(publicIdWithExt) ?? "", Path.GetFileNameWithoutExtension(publicIdWithExt))
            .Replace("\\", "/");
    }
}