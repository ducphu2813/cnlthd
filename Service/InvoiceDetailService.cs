
using APIApplication.DTO.InvoiceDetail;
using APIApplication.Exception;
using APIApplication.Model;
using APIApplication.Repository.Interface;
using APIApplication.Service.Interfaces;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace APIApplication.Service;

public class InvoiceDetailService : IInvoiceDetailService
{
    private readonly IInvoiceDetailRepository _invoiceDetailRepository;
    private readonly IProductRepository _productRepository;
    private readonly IinvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    
    public InvoiceDetailService(IInvoiceDetailRepository invoiceDetailRepository
                                , IMapper mapper
                                , ILogger<InvoiceDetailService> logger
                                , IProductRepository productRepository
                                , IinvoiceRepository invoiceRepository)
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

    public async Task<InvoiceDetailDTO> Add(SaveInvoiceDetailDTO invoiceDetail)
    {
        // Map DTO to Model
        var invoiceDetailModel = _mapper.Map<InvoiceDetail>(invoiceDetail);

        // Validate Product and Invoice existence
        var product = await _productRepository.GetById(invoiceDetail.ProductId);
        if (product == null)
        {
            _logger.LogError("Product not found");
            throw new ProductNotFoundException(invoiceDetail.ProductId);
        }

        var invoice = await _invoiceRepository.GetById(invoiceDetail.InvoiceId);
        if (invoice == null)
        {
            _logger.LogError("Invoice not found");
            throw new InvoiceNotFoundException(invoiceDetail.InvoiceId);
        }

        // Check stock availability early
        if (invoiceDetailModel.Quantity > product.Quantity)
        {
            _logger.LogError("Not a valid amount");
            throw new ValidateException("Quantity", invoiceDetailModel.Quantity);
        }

        if (invoice.Status == InvoiceStatus.Cancelled || invoice.Status == InvoiceStatus.Completed)
        {
            _logger.LogError("Invoice is paid or cancelled");
            throw new ValidationException("Cannot modify a cancelled or completed invoice."); // Use ValidationException
        }

        // Check for existing detail and update or add
        var detail = await _invoiceDetailRepository.GetByInvoiceAndProductIdAsync(invoiceDetail.InvoiceId, invoiceDetail.ProductId);
        InvoiceDetail updatedDetail;

        if (detail != null)
        {
            int newQuantity = detail.Quantity + invoiceDetailModel.Quantity;
            if (newQuantity > product.Quantity)
            {
                _logger.LogError("Not a valid amount");
                throw new ValidateException("Quantity", invoiceDetailModel.Quantity);
            }

            // Update existing detail in-place
            detail.Quantity = newQuantity;
            detail.Total += invoiceDetailModel.Quantity * product.Price; // Incremental update
            updatedDetail = await _invoiceDetailRepository.Update(detail.Id, detail);
            _logger.LogInformation("Updated ProductId: {0}, Quantity: {1}", detail.ProductId, detail.Quantity);

            // Update product stock
            product.Quantity -= invoiceDetailModel.Quantity;
            await _productRepository.Update(product.Id, product);
        }
        else
        {
            // Set necessary fields for new detail
            invoiceDetailModel.Id = Guid.NewGuid();
            invoiceDetailModel.InvoiceId = invoice.Id;
            invoiceDetailModel.Total = invoiceDetailModel.Quantity * product.Price;
            updatedDetail = await _invoiceDetailRepository.Add(invoiceDetailModel);
            _logger.LogInformation("Added ProductId: {0}, Quantity: {1}", updatedDetail.ProductId, updatedDetail.Quantity);

            // Update product stock
            product.Quantity -= invoiceDetailModel.Quantity;
            await _productRepository.Update(product.Id, product);
        }

        // Return mapped DTO
        return _mapper.Map<InvoiceDetailDTO>(updatedDetail);
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

    public async Task<bool> Remove(Guid id)
    {
        return await _invoiceDetailRepository.Remove(id);
    }
}