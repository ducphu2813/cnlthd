using CloudinaryDotNet.Actions;

namespace APIApplication.Service.Interfaces;

public interface IPhotoService
{
    //thêm ảnh
    public Task<ImageUploadResult> UploadPhotoAsync(IFormFile file);
    
    //xóa ảnh
    public Task<DeletionResult> DeletePhotoAsync(string publicId);

    //extract public_id từ url
    public string ExtractPublicId(string imageUrl);
}