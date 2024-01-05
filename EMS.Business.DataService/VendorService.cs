using AutoMapper;
using EMS.Business.Interfaces;
using EMS.Business.Models;
using EMS.Data.Interfaces;
using EMS.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.Business.DataService
{
    public class VendorService : GenericCrudService<Vendor, VendorModel>, IVendorService
    {
        public VendorService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        public async Task<int> GetVendorCountAsync()
        {
            var count = await _unitOfWork.GetRepository<Vendor>().GetAllAsync().CountAsync();
            return count!;
        }
        public async Task<bool> UpdateVendorDescriptionAsync(int vendorId, string newDescription)
        {
            var vendor = await _unitOfWork.GetRepository<Vendor>().GetById(vendorId);

            if (vendor == null)
            {
                return false;
            }

            vendor.Description = newDescription;
            return _unitOfWork.GetRepository<Vendor>().Update(vendor);
        }

        public async Task<VendorModel?> GetVendorWithReviewsAsync(int id)
        {
            var vendors = await _unitOfWork.GetRepository<Vendor>()
               .GetWithIncludeAsync(
                   v => v.Id == id,
                   v => v.VendorEvents,
                   v => v.VendorEvents.Select(ve => ve.Event)
                   );

            var vendor = vendors.SingleOrDefault();

            return _mapper.Map<VendorModel?>(vendor);
        }
    }
}
