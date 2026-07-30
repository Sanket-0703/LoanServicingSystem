using CoreData.Dashboard.Models;

namespace CoreData.Dashboard.Interfaces;

public interface IDashboardRepository
{
    Task<AdminDashboardModel> GetAdminDashboardAsync();
    Task<LoanOfficerDashboardModel> GetLoanOfficerDashboardAsync(Guid Id);
    Task<CollectionsDashboardModel> GetCollectionsDashboardAsync();
    Task<AuditorDashboardModel> GetAuditorDashboardAsync();
}