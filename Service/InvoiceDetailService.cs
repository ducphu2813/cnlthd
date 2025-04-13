
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
        
        var invoice = await _invoiceRepository.GetById(invoiceDetailDTO.InvoiceId);
        
        if(invoice == null)
        {
            _logger.LogError("Invoice not found");
            throw new InvoiceNotFoundException(invoiceDetailDTO.InvoiceId);
        }
        
        //kiểm tra số lượng sản phẩm còn đủ không
        if(product.Quantity < invoiceDetailDTO.Quantity || product.Quantity <= 0)
        {
            _logger.LogError("Product out of stock");
            throw new System.Exception("Product out of stock");
        }
        
        //tìm theo product id và invoice id xem đã tồn tại chưa
        var invoiceDetailToUpdate = await _invoiceDetailRepository.GetByProductIdAndInvoiceId(invoiceDetailDTO.ProductId, invoiceDetailDTO.InvoiceId);
        
        //nếu tồn tại rồi thì cộng thêm số lượng
        if(invoiceDetailToUpdate != null)
        {
            invoiceDetailToUpdate.Quantity += invoiceDetailDTO.Quantity;
            invoiceDetailToUpdate.Amount += invoiceDetailDTO.Quantity * product.Price;
            
            await _invoiceDetailRepository.Update(invoiceDetailToUpdate.Id, invoiceDetailToUpdate);
            
            //cập nhật tổng tiền cho hóa đơn
            invoice.TotalAmount += invoiceDetailDTO.Quantity * product.Price;
            await _invoiceRepository.Update(invoice.Id, invoice);
            
            //cập nhật số lượng sản phẩm
            product.Quantity -= invoiceDetailDTO.Quantity;
            await _productRepository.Update(product.Id, product);
            
            //chuyển từ Model về DTO
            return _mapper.Map<InvoiceDetailDTO>(invoiceDetailToUpdate);
        }
        
        //nếu chưa tồn tại thì thêm mới và tính tổng tiền
        invoiceDetailModel.Amount = invoiceDetailModel.Quantity * product.Price;
        
        //cập nhật tổng tiền cho hóa đơn
        invoice.TotalAmount += invoiceDetailModel.Amount;
        
        //cập nhật số lượng sản phẩm
        product.Quantity -= invoiceDetailModel.Quantity;
        await _productRepository.Update(product.Id, product);
        
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