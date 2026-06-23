using Findly.Domain.Entities;
using Findly.Domain.Enums;

namespace Findly.Domain.Repositories;



public interface IListingRepository {
   Task<Listing> GetByIdAsync(int id,CancellationToken cancelationtoken);

      Task <IEnumerable<Listing> >GetAllAsync(CancellationToken cancellationToken);

   Task <Listing> UpdateAsync(Listing listing , CancellationToken cancellationToken);

   Task <Listing> CreateAync(Listing listing ,CancellationToken cancellationToken);

   Task <bool> DeleteAsync(int id , CancellationToken cancellationToken);
}
