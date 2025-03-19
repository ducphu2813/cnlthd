
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
        //chuyển từ DTO về Model
        var invoiceDetailModel = _mapper.Map<InvoiceDetail>(invoiceDetail);
        
        //lấy id product và id invoice ra kiểm tra xem có tồn tại không
        var product = await _productRepository.GetById(invoiceDetail.ProductId);
        
        if(product == null)
        {
            _logger.LogError("Product not found");
            throw new ProductNotFoundException(invoiceDetail.ProductId);
        }
        
        var invoice = await _invoiceRepository.GetById(invoiceDetail.InvoiceId);
        
        if(invoice == null)
        {
            _logger.LogError("Invoice not found");
            throw new InvoiceNotFoundException(invoiceDetail.InvoiceId);
        }
        
        await _invoiceDetailRepository.Add(invoiceDetailModel);
        
        //chuyển từ Model về DTO
        return _mapper.Map<InvoiceDetailDTO>(invoiceDetailModel);
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