using APIApplication.DTO.InvoiceDetail;
using APIApplication.Exception;
using APIApplication.Model;
using APIApplication.Repository.Interface;
using APIApplication.Service.Interfaces;
using AutoMapper;

namespace APIApplication.Service;

public class InvoiceDetailService : IInvoiceDetailService
{
    private readonly IInvoiceDetailRepository _invoiceDetailRepository;
    private readonly IProductRepository _productRepository;
    private readonly IinvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public InvoiceDetailService(
        IInvoiceDetailRepository invoiceDetailRepository,
        IMapper mapper,
        ILogger<InvoiceDetailService> logger,
        IProductRepository productRepository,
        IinvoiceRepository invoiceRepository
    )
    {
        _invoiceDetailRepository = invoiceDetailRepository;
        _mapper = mapper;
        _logger = logger;
        _productRepository = productRepository;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<List<InvoiceDetailDTO>> GetAll()
    {
        var invoiceDetails = await _invoiceDetailRepository.GetAll();

        //chuyển từ List<Model> về List<DTO>
        return _mapper.Map<List<InvoiceDetailDTO>>(invoiceDetails);
    }

    public async Task<InvoiceDetailDTO> GetById(Guid id)
    {
        var invoiceDetail = await _invoiceDetailRepository.GetById(id);

        //chuyển từ Model về DTO
        return _mapper.Map<InvoiceDetailDTO>(invoiceDetail);
    }

    //lấy ra danh sách chi tiết hóa đơn theo id hóa đơn
    public async Task<List<InvoiceDetailDTO>> GetByInvoiceId(Guid invoiceId)
    {
        var invoiceDetails = await _invoiceDetailRepository.GetByInvoiceId(invoiceId);

        //chuyển từ List<Model> về List<DTO>
        return _mapper.Map<List<InvoiceDetailDTO>>(invoiceDetails);
    }

    public async Task<InvoiceDetailDTO> Add(SaveInvoiceDetailDTO invoiceDetailDTO)
    {
        // Lấy Product
        var product = await _productRepository.GetById(invoiceDetailDTO.ProductId);
        if (product == null)
        {
            _logger.LogError("Product not found");
            throw new ProductNotFoundException(invoiceDetailDTO.ProductId);
        }

        // Kiểm tra số lượng tồn kho
        if (product.Quantity <= 0 || product.Quantity < invoiceDetailDTO.Quantity)
        {
            _logger.LogError("Product out of stock");
            throw new ProductNotFoundException(product.Id);
        }

        // Lấy Invoice
        var invoice = await _invoiceRepository.GetById(invoiceDetailDTO.InvoiceId);
        if (invoice == null)
        {
            _logger.LogError("Invoice not found");
            throw new InvoiceNotFoundException(invoiceDetailDTO.InvoiceId);
        }

        // Kiểm tra trạng thái của Invoice
        if (invoice.Status == true)
        {
            _logger.LogError($"Invoice {invoice.Id} has already been paid (Status = true)");
            throw new InvalidOperationException(
                $"Hóa đơn {invoice.Id} đã được thanh toán, không thể thêm sản phẩm"
            );
        }

        // Kiểm tra xem sản phẩm đã tồn tại trong hóa đơn chưa
        var existingInvoiceDetail = await _invoiceDetailRepository.GetByInvoiceIdAndProductId(
            invoiceDetailDTO.InvoiceId,
            invoiceDetailDTO.ProductId
        );

        if (existingInvoiceDetail != null)
        {
            // Nếu sản phẩm đã tồn tại, tăng Quantity
            existingInvoiceDetail.Quantity += invoiceDetailDTO.Quantity;
            existingInvoiceDetail.Total = product.Price * existingInvoiceDetail.Quantity;
            await _invoiceDetailRepository.Update(existingInvoiceDetail.Id, existingInvoiceDetail);

            invoice.TotalAmount += product.Price * invoiceDetailDTO.Quantity;
            await _invoiceRepository.Update(invoice.Id, invoice);

            product.Quantity -= invoiceDetailDTO.Quantity;
            await _productRepository.Update(product.Id, product);

            return _mapper.Map<InvoiceDetailDTO>(existingInvoiceDetail);
        }
        else
        {
            // Nếu sản phẩm chưa tồn tại, tạo mới InvoiceDetail
            var newInvoiceDetail = _mapper.Map<InvoiceDetail>(invoiceDetailDTO);
            newInvoiceDetail.Id = Guid.NewGuid();
            newInvoiceDetail.Total = product.Price * newInvoiceDetail.Quantity;

            invoice.TotalAmount += newInvoiceDetail.Total;
            await _invoiceRepository.Update(invoice.Id, invoice);

            product.Quantity -= invoiceDetailDTO.Quantity;
            await _productRepository.Update(product.Id, product);

            await _invoiceDetailRepository.Add(newInvoiceDetail);

            return _mapper.Map<InvoiceDetailDTO>(newInvoiceDetail);
        }
    }

    public async Task<InvoiceDetailDTO> Update(Guid id, SaveInvoiceDetailDTO invoiceDetail)
    {
        //chuyển từ DTO về Model
        var invoiceDetailModel = _mapper.Map<InvoiceDetail>(invoiceDetail);

        //set id cũ cho obj
        invoiceDetailModel.Id = id;

        await _invoiceDetailRepository.Update(id, invoiceDetailModel);

        //chuyển từ Model về DTO
        return _mapper.Map<InvoiceDetailDTO>(invoiceDetailModel);
    }

    // public async Task<bool> Remove(Guid id)
    // {
    //     return await _invoiceDetailRepository.Remove(id);
    // }

    public async Task<bool> Remove(Guid id)
    {
        // Lấy InvoiceDetail theo id
        var invoiceDetail = await _invoiceDetailRepository.GetById(id);
        if (invoiceDetail == null)
        {
            _logger.LogError($"InvoiceDetail with ID {id} not found");
            throw new NotFoundException("InvoiceDetail not found");
        }

        // Lấy Product theo ProductId
        var product = await _productRepository.GetById(invoiceDetail.ProductId);
        if (product == null)
        {
            _logger.LogError($"Product with ID {invoiceDetail.ProductId} not found");
            throw new ProductNotFoundException(invoiceDetail.ProductId);
        }

        // Lấy Invoice theo InvoiceId
        var invoice = await _invoiceRepository.GetById(invoiceDetail.InvoiceId);
        if (invoice == null)
        {
            _logger.LogError($"Invoice with ID {invoiceDetail.InvoiceId} not found");
            throw new InvoiceNotFoundException(invoiceDetail.InvoiceId);
        }

        // Trả lại số lượng sản phẩm vào kho
        product.Quantity += invoiceDetail.Quantity;
        await _productRepository.Update(product.Id, product);

        // Cập nhật TotalAmount của Invoice (trừ đi giá trị của InvoiceDetail)
        invoice.TotalAmount -= invoiceDetail.Total;
        await _invoiceRepository.Update(invoice.Id, invoice);

        // Xóa InvoiceDetail
        var result = await _invoiceDetailRepository.Remove(id);
        if (result)
        {
            _logger.LogInformation(
                $"Successfully removed InvoiceDetail {id}. Returned {invoiceDetail.Quantity} units to product {product.Id}."
            );
        }
        else
        {
            _logger.LogError($"Failed to remove InvoiceDetail {id}");
        }

        return result;
    }

    // tăng số lượng sản phẩm
    public async Task<InvoiceDetailDTO> IncreaseQuantity(Guid id)
    {
        // Lấy InvoiceDetail
        var invoiceDetail = await _invoiceDetailRepository.GetById(id);
        if (invoiceDetail == null)
        {
            _logger.LogError($"InvoiceDetail with ID {id} not found");
            throw new NotFoundException("InvoiceDetail not found");
        }

        // Lấy Product
        var product = await _productRepository.GetById(invoiceDetail.ProductId);
        if (product == null)
        {
            _logger.LogError($"Product with ID {invoiceDetail.ProductId} not found");
            throw new NotFoundException("InvoiceDetail not found");
        }

        // Lấy Invoice
        var invoice = await _invoiceRepository.GetById(invoiceDetail.InvoiceId);
        if (invoice == null)
        {
            _logger.LogError($"Invoice with ID {invoiceDetail.InvoiceId} not found");
            throw new NotFoundException("InvoiceDetail not found");
        }

        // Kiểm tra số lượng tồn kho
        if (product.Quantity <= 0)
        {
            _logger.LogWarning($"Product {product.Name} is out of stock");
            throw new NotFoundException("InvoiceDetail not found");
        }

        // Tăng quantity
        invoiceDetail.Quantity += 1;

        // Giảm số lượng trong kho
        product.Quantity -= 1;

        // Cập nhật Total của InvoiceDetail
        double unitPrice = product.Price ?? 0; // Giả sử Product có Price
        invoiceDetail.Total = invoiceDetail.Quantity * unitPrice;

        // Cập nhật TotalAmount của Invoice
        invoice.TotalAmount += unitPrice;

        // Cập nhật database
        await _invoiceDetailRepository.Update(invoiceDetail.Id, invoiceDetail);
        await _productRepository.Update(product.Id, product);
        await _invoiceRepository.Update(invoice.Id, invoice);

        _logger.LogInformation(
            $"Increased quantity of InvoiceDetail {id}. New quantity: {invoiceDetail.Quantity}"
        );

        return _mapper.Map<InvoiceDetailDTO>(invoiceDetail);
    }

    public async Task<InvoiceDetailDTO> DecreaseQuantity(Guid id)
    {
        // Lấy InvoiceDetail
        var invoiceDetail = await _invoiceDetailRepository.GetById(id);
        if (invoiceDetail == null)
        {
            _logger.LogError($"InvoiceDetail with ID {id} not found");
            throw new NotFoundException("InvoiceDetail not found");
        }

        // Lấy Product
        var product = await _productRepository.GetById(invoiceDetail.ProductId);
        if (product == null)
        {
            _logger.LogError($"Product with ID {invoiceDetail.ProductId} not found");
            throw new NotFoundException("InvoiceDetail not found");
        }

        // Lấy Invoice
        var invoice = await _invoiceRepository.GetById(invoiceDetail.InvoiceId);
        if (invoice == null)
        {
            _logger.LogError($"Invoice with ID {invoiceDetail.InvoiceId} not found");
            throw new NotFoundException("InvoiceDetail not found");
        }

        // Giảm quantity
        invoiceDetail.Quantity -= 1;

        // Tính UnitPrice
        double unitPrice = product.Price ?? 0; // Giả sử Product có Price

        if (invoiceDetail.Quantity <= 0)
        {
            // Nếu quantity = 0, xóa InvoiceDetail
            await _invoiceDetailRepository.Remove(id);

            // Cập nhật TotalAmount của Invoice
            invoice.TotalAmount -= (invoiceDetail.Quantity + 1) * unitPrice; // Trừ đi giá trị trước khi quantity = 0

            // Tăng số lượng trong kho
            product.Quantity += (invoiceDetail.Quantity + 1); // Hoàn lại số lượng trước khi quantity = 0

            _logger.LogInformation($"Removed InvoiceDetail {id} as quantity reached 0");
        }
        else
        {
            // Cập nhật Total của InvoiceDetail
            invoiceDetail.Total = invoiceDetail.Quantity * unitPrice;

            // Cập nhật TotalAmount của Invoice
            invoice.TotalAmount -= unitPrice;

            // Tăng số lượng trong kho
            product.Quantity += 1;

            // Cập nhật InvoiceDetail
            await _invoiceDetailRepository.Update(invoiceDetail.Id, invoiceDetail);

            _logger.LogInformation(
                $"Decreased quantity of InvoiceDetail {id}. New quantity: {invoiceDetail.Quantity}"
            );
        }

        // Cập nhật Product và Invoice
        await _productRepository.Update(product.Id, product);
        await _invoiceRepository.Update(invoice.Id, invoice);

        return _mapper.Map<InvoiceDetailDTO>(invoiceDetail);
    }
}
