using EMS.Business.Models;
using EMS.Data.Models;

namespace EMS.Business.Interfaces
{
    public interface IVendorService : IGenericCrudService<Vendor, VendorModel>
    {
        Task<int> GetVendorCountAsync();
        Task<VendorModel?> GetVendorWithReviewsAsync(int id);
        Task<bool> UpdateVendorDescriptionAsync(int vendorId, string newDescription);


    }
}
