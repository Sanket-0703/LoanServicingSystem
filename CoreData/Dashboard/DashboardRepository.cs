using System.Data;
using CoreData.Dashboard.Interfaces;
using CoreData.Dashboard.Models;
using Dapper;

namespace CoreData.Dashboard;

public class DashboardRepository : IDashboardRepository
{
    private readonly IDbConnection _connection;

    public DashboardRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<AdminDashboardModel> GetAdminDashboardAsync()
    {
        const string sql = @"

--------------------------------------------------------
-- KPI CARDS
--------------------------------------------------------

SELECT
    COUNT(CASE WHEN Status='Active' THEN 1 END) ActiveLoanCount,
    ISNULL(SUM(CASE WHEN Status='Active' THEN Principal END),0) TotalPortfolio,
    (SELECT COUNT(*) FROM Customers) CustomerCount,
    (
        SELECT ISNULL(SUM(Amount),0)
        FROM Payments
        WHERE CAST(PaymentDate AS DATE)=CAST(GETDATE() AS DATE)
    ) CollectionsToday
FROM Loans;

--------------------------------------------------------
-- PORTFOLIO SUMMARY
--------------------------------------------------------

SELECT

COUNT(CASE WHEN Status='Draft' THEN 1 END) DraftLoanCount,

COUNT(CASE WHEN Status='Active' THEN 1 END) ActiveLoanCount,

COUNT(CASE WHEN Status='Closed' THEN 1 END) ClosedLoanCount,

COUNT(
CASE
WHEN Status='Active'
AND EndDate<GETDATE()
THEN 1
END
) OverdueLoanCount

FROM Loans;

--------------------------------------------------------
-- RECENT LOANS
--------------------------------------------------------

SELECT TOP (5)

    l.LoanNumber,

    c.Name AS CustomerName,

    lp.Name AS ProductName,

    l.Principal,

    l.Status,

    l.StartDate

FROM Loans l

INNER JOIN Customers c
    ON l.CustomerId = c.Id

INNER JOIN LoanProducts lp
    ON l.ProductId = lp.Id

ORDER BY l.StartDate DESC;

--------------------------------------------------------
-- RECENT PAYMENTS
--------------------------------------------------------

SELECT TOP (5)

    l.LoanNumber,

    c.Name AS CustomerName,

    p.Amount,

    p.PaymentDate,

    ISNULL(p.Mode,'Cash') AS PaymentMode

FROM Payments p

INNER JOIN Loans l
    ON p.LoanId = l.Id

INNER JOIN Customers c
    ON l.CustomerId = c.Id

ORDER BY p.PaymentDate DESC;

--------------------------------------------------------
-- LOAN STATUS SUMMARY
--------------------------------------------------------

SELECT

Status,

COUNT(*) AS Count

FROM Loans

GROUP BY Status

ORDER BY Status;

--------------------------------------------------------
-- COLLECTION TREND
--------------------------------------------------------

SELECT
    FORMAT(PaymentDate,'dd MMM') AS Date,
    SUM(Amount) AS Amount
FROM Payments
GROUP BY FORMAT(PaymentDate,'dd MMM')
ORDER BY MIN(PaymentDate);

";

        var dashboard = new AdminDashboardModel();

        using var multi = await _connection.QueryMultipleAsync(sql);

        dashboard = await multi.ReadFirstAsync<AdminDashboardModel>();

        dashboard.Portfolio =
            await multi.ReadFirstAsync<PortfolioSummaryModel>();

        dashboard.RecentLoans =
            (await multi.ReadAsync<RecentLoanModel>()).ToList();
        dashboard.RecentPayments =
    (await multi.ReadAsync<RecentPaymentModel>()).ToList();

        dashboard.LoanStatus =
    (await multi.ReadAsync<LoanStatusChartModel>()).ToList();

        dashboard.CollectionTrend =
     (await multi.ReadAsync<CollectionTrendModel>()).ToList();

        return dashboard;
    }
}