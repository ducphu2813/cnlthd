using APIApplication.DTO.Invoice;
using APIApplication.Model;
using APIApplication.Repository.Interface;
using APIApplication.Service.Interfaces;
using AutoMapper;

namespace APIApplication.Service;

public class InvoiceService : IInvoiceService
{
    private readonly IinvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;
    
    public InvoiceService(IinvoiceRepository invoiceRepository
                            , IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _mapper = mapper;
    }
    
    public async Task<List<InvoiceDTO>> GetAll()
    {
        var invoices = await _invoiceRepository.GetAll();
        
        //chuyển từ List<Model> về List<DTO>
        return _mapper.Map<List<InvoiceDTO>>(invoices);
    }

    public async Task<InvoiceDTO> GetById(Guid id)
    {
        var invoice = await _invoiceRepository.GetById(id);
        
        //chuyển từ Model về DTO
        return _mapper.Map<InvoiceDTO>(invoice);
    }

    public async Task<InvoiceDTO> Add(SaveInvoiceDTO invoice)
    {
        //chuyển từ DTO về Model
        var invoiceModel = _mapper.Map<Invoice>(invoice);
        
        await _invoiceRepository.Add(invoiceModel);
        
        //chuyển từ Model về DTO
        return _mapper.Map<InvoiceDTO>(invoiceModel);
    }

    public async Task<InvoiceDTO> Update(Guid id, SaveInvoiceDTO invoice)
    {
        //chuyển từ DTO về Model
        var invoiceModel = _mapper.Map<Invoice>(invoice);
        
        //set id cũ cho obj
        invoiceModel.Id = id;
        
        await _invoiceRepository.Update(id, invoiceModel);
        
        //chuyển từ Model về DTO
        return _mapper.Map<InvoiceDTO>(invoiceModel);
    }

    public async Task<bool> Remove(Guid id)
    {
        return await _invoiceRepository.Remove(id);
    }
}