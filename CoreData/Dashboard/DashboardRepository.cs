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

    public async Task<LoanOfficerDashboardModel> GetLoanOfficerDashboardAsync()
    {
        const string sql = @"

--------------------------------------------------------
-- KPI CARDS
--------------------------------------------------------

SELECT

COUNT(*) TotalApplications,

COUNT(CASE WHEN Status='Pending' THEN 1 END)
    PendingApprovalCount,

COUNT(CASE WHEN Status='Approved' THEN 1 END)
    ApprovedCount,

COUNT(CASE WHEN Status='Rejected' THEN 1 END)
    RejectedCount

FROM Loans;

--------------------------------------------------------
-- WEEKLY APPLICATIONS
--------------------------------------------------------

SELECT

FORMAT(StartDate,'ddd') Date,

COUNT(*) Amount

FROM Loans

WHERE StartDate>=DATEADD(DAY,-6,CAST(GETDATE() AS DATE))

GROUP BY
FORMAT(StartDate,'ddd'),
DATEPART(WEEKDAY,StartDate)

ORDER BY
DATEPART(WEEKDAY,StartDate);

--------------------------------------------------------
-- APPLICATION STATUS
--------------------------------------------------------

SELECT

Status,

COUNT(*) Count

FROM Loans

GROUP BY Status;

--------------------------------------------------------
-- MY APPLICATIONS
--------------------------------------------------------

SELECT TOP (5)

l.LoanNumber,

c.Name CustomerName,

l.Principal,

l.Status,

l.StartDate

FROM Loans l

INNER JOIN Customers c
ON l.CustomerId=c.Id

ORDER BY l.StartDate DESC;

--------------------------------------------------------
-- PENDING APPROVALS
--------------------------------------------------------

SELECT TOP (5)

l.LoanNumber,

c.Name CustomerName,

l.Principal,

l.Status,

l.StartDate

FROM Loans l

INNER JOIN Customers c
ON l.CustomerId=c.Id

WHERE l.Status='Pending'

ORDER BY l.StartDate DESC;

";

        var dashboard = new LoanOfficerDashboardModel();

        using var multi = await _connection.QueryMultipleAsync(sql);

        dashboard = await multi.ReadSingleAsync<LoanOfficerDashboardModel>();

        dashboard.WeeklyApplications =
            (await multi.ReadAsync<CollectionTrendModel>()).ToList();

        dashboard.ApplicationStatus =
            (await multi.ReadAsync<LoanStatusChartModel>()).ToList();

        dashboard.MyApplications =
            (await multi.ReadAsync<RecentLoanModel>()).ToList();

        dashboard.PendingApprovals =
            (await multi.ReadAsync<RecentLoanModel>()).ToList();

        return dashboard;
    }

    public async Task<CollectionsDashboardModel> GetCollectionsDashboardAsync()
    {
        const string sql = @"

--------------------------------------------------------
-- KPI CARDS
--------------------------------------------------------

SELECT

COUNT(
CASE
WHEN Status='Active'
AND EndDate<GETDATE()
THEN 1
END
) OverdueLoanCount,

(
SELECT COUNT(*)
FROM RepaymentSchedules
WHERE DueDate=CAST(GETDATE() AS DATE)
AND Status='Pending'
) DueTodayCount,

(
SELECT ISNULL(SUM(Amount),0)
FROM Payments
WHERE CAST(PaymentDate AS DATE)=CAST(GETDATE() AS DATE)
) CollectedToday,

(
SELECT ISNULL(SUM(Amount),0)
FROM Penalties
WHERE Status='Unpaid'
) UnpaidPenaltyAmount;

--------------------------------------------------------
-- COLLECTION TREND
--------------------------------------------------------

SELECT

CAST(PaymentDate AS DATE) Date,

SUM(Amount) Amount

FROM Payments

GROUP BY CAST(PaymentDate AS DATE)

ORDER BY Date;

--------------------------------------------------------
-- OVERDUE STATUS
--------------------------------------------------------

SELECT

Status,

COUNT(*) Count

FROM RepaymentSchedules

GROUP BY Status;

--------------------------------------------------------
-- OVERDUE LOANS
--------------------------------------------------------

SELECT TOP (10)

l.LoanNumber,

c.Name CustomerName,

l.Principal,

l.Status,

l.EndDate StartDate

FROM Loans l

INNER JOIN Customers c
ON l.CustomerId=c.Id

WHERE l.Status='Active'
AND l.EndDate<GETDATE()

ORDER BY l.EndDate;

--------------------------------------------------------
-- RECENT PAYMENTS
--------------------------------------------------------

SELECT TOP (10)

LoanNumber,

CustomerName,

Amount,

PaymentDate,

Mode

FROM
(
SELECT

l.LoanNumber,

c.Name CustomerName,

p.Amount,

p.PaymentDate,

p.Mode

FROM Payments p

INNER JOIN Loans l
ON p.LoanId=l.Id

INNER JOIN Customers c
ON l.CustomerId=c.Id

)x

ORDER BY PaymentDate DESC;

";

        var dashboard = new CollectionsDashboardModel();

        using var multi = await _connection.QueryMultipleAsync(sql);

        dashboard =
            await multi.ReadSingleAsync<CollectionsDashboardModel>();

        dashboard.CollectionTrend =
            (await multi.ReadAsync<CollectionTrendModel>()).ToList();

        dashboard.OverdueStatus =
            (await multi.ReadAsync<LoanStatusChartModel>()).ToList();

        dashboard.OverdueLoans =
            (await multi.ReadAsync<RecentLoanModel>()).ToList();

        dashboard.RecentPayments =
            (await multi.ReadAsync<RecentPaymentModel>()).ToList();

        return dashboard;
    }

    public async Task<AuditorDashboardModel> GetAuditorDashboardAsync()
    {
        const string sql = @"------------------------------------------------------------
-- KPI CARDS
------------------------------------------------------------

SELECT
    COUNT(*) AS TotalLoans,

    SUM(CASE
            WHEN Status = 'Active'
            THEN 1
            ELSE 0
        END) AS ActiveLoans,

    ISNULL(
        (
            SELECT SUM(Amount)
            FROM Payments
        ),0
    ) AS TotalCollections,

    ISNULL(
        (
            SELECT COUNT(*)
            FROM vw_AuditTrail
        ),0
    ) AS AuditRecordCount

FROM Loans;

------------------------------------------------------------
-- LOAN STATUS CHART
------------------------------------------------------------

SELECT
    Status,
    COUNT(*) AS Count
FROM Loans
GROUP BY Status
ORDER BY Status;

------------------------------------------------------------
-- COLLECTION TREND CHART
------------------------------------------------------------

SELECT
    CAST(PaymentDate AS DATE) AS Date,
    SUM(Amount) AS Amount
FROM Payments
GROUP BY CAST(PaymentDate AS DATE)
ORDER BY Date;

------------------------------------------------------------
-- RECENT AUDIT LOGS
------------------------------------------------------------

SELECT TOP (10)

    ChangedOn,
    ChangedBy,
    Module,
    ChangeType

FROM vw_AuditTrail

ORDER BY ChangedOn DESC;

------------------------------------------------------------
-- RECENT LOANS
------------------------------------------------------------

SELECT TOP (10)

    l.LoanNumber,
    c.Name AS CustomerName,
    l.Principal,
    l.Status,
    l.StartDate

FROM Loans l
INNER JOIN Customers c
ON l.CustomerId = c.Id

ORDER BY l.StartDate DESC;";

        using var multi = await _connection.QueryMultipleAsync(sql);

        var dashboard = await multi.ReadSingleAsync<AuditorDashboardModel>();

        dashboard.LoanStatus =
            (await multi.ReadAsync<LoanStatusChartModel>()).ToList();

        dashboard.CollectionTrend =
            (await multi.ReadAsync<CollectionTrendModel>()).ToList();

        dashboard.RecentAuditLogs =
            (await multi.ReadAsync<AuditTrailModel>()).ToList();

        dashboard.RecentLoans =
            (await multi.ReadAsync<RecentLoanModel>()).ToList();

        return dashboard;
    }
}