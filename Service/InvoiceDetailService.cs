
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
        //chuyển từ DTO về Model
        var invoiceDetailModel = _mapper.Map<InvoiceDetail>(invoiceDetailDTO);
        
        //lấy id product và id invoice ra kiểm tra xem có tồn tại không
        var product = await _productRepository.GetById(invoiceDetailDTO.ProductId);
        
        if(product == null)
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
        if (invoice.Status != "PENDING")
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
            existingInvoiceDetail.Amount = product.Price * existingInvoiceDetail.Quantity;
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
            newInvoiceDetail.Amount = product.Price * newInvoiceDetail.Quantity;

            invoice.TotalAmount += newInvoiceDetail.Amount;
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

    public async Task<InvoiceDetailDTO> UpdateQuantity(Guid invoiceId, Guid productId, int quantityChange)
    {
        if (quantityChange == 0)
            throw new System.Exception("Quantity change must not be zero");

        var invoiceDetail = await _invoiceDetailRepository.GetByProductIdAndInvoiceId(productId, invoiceId)
                            ?? throw new System.Exception("Invoice detail not found");

        var product = await _productRepository.GetById(productId)
                      ?? throw new ProductNotFoundException(productId);

        var invoice = await _invoiceRepository.GetById(invoiceId);

        if (quantityChange > 0)
        {
            if (product.Quantity < quantityChange)
                throw new System.Exception("Not enough stock to increase quantity");

            invoiceDetail.Quantity += quantityChange;
            invoiceDetail.Amount += (product.Price ?? 0) * quantityChange;

            invoice.TotalAmount += (product.Price ?? 0) * quantityChange;
            product.Quantity -= quantityChange;
        }
        else // quantityChange < 0
        {
            int decreaseBy = -quantityChange;
            if (invoiceDetail.Quantity <= decreaseBy)
                throw new System.Exception("Cannot decrease more than current quantity. Use remove instead.");

            invoiceDetail.Quantity -= decreaseBy;
            invoiceDetail.Amount -= (product.Price ?? 0) * decreaseBy;

            invoice.TotalAmount -= (product.Price ?? 0) * decreaseBy;
            product.Quantity += decreaseBy;
        }

        await _productRepository.Update(product.Id, product);
        await _invoiceRepository.Update(invoice.Id, invoice);
        await _invoiceDetailRepository.Update(invoiceDetail.Id, invoiceDetail);

        return _mapper.Map<InvoiceDetailDTO>(invoiceDetail);
    }

    //xóa sản phẩm trong hóa đơn
    public async Task<bool> RemoveProductFromInvoice(Guid invoiceId, Guid productId)
    {
        var invoiceDetail = await _invoiceDetailRepository.GetByProductIdAndInvoiceId(productId, invoiceId)
                            ?? throw new System.Exception("Invoice detail not found");

        var product = await _productRepository.GetById(productId)
                      ?? throw new ProductNotFoundException(productId);

        var invoice = await _invoiceRepository.GetById(invoiceId);

        // hoàn lại số lượng về kho
        product.Quantity += invoiceDetail.Quantity;
        invoice.TotalAmount -= invoiceDetail.Amount ?? 0;

        await _productRepository.Update(product.Id, product);
        await _invoiceRepository.Update(invoice.Id, invoice);
        return await _invoiceDetailRepository.Remove(invoiceDetail.Id);
    }


    public async Task<bool> Remove(Guid id)
    {
        return await _invoiceDetailRepository.Remove(id);
    }

    
}
