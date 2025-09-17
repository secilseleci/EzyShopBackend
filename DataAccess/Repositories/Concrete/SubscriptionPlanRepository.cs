using DataAccess.Repositories.Abstract;
using Models.Entities.Concrete;

namespace DataAccess.Repositories.Concrete;
public class SubscriptionPlanRepository(ApplicationDbContext context) : BaseRepository<SubscriptionPlan>(context), ISubscriptionPlanRepository
{
}
