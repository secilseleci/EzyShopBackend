using DataAccess;
using DataAccess.Repositories.Abstract;
using DataAccess.Repositories.Concrete;
using Models.Entities.Concrete;

namespace Business.Services.Concrete;
public class SubscriptionPlanRepository(ApplicationDbContext context) : BaseRepository<SubscriptionPlan>(context), ISubscriptionPlanRepository
{
}
