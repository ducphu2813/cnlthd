namespace APIApplication.DTO;

public class SaveProductDTO
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double? Price { get; set; }
    
    //file ảnh
    public IFormFile? Image { get; set; }
}