
CREATE OR ALTER TRIGGER dbo.Roles_WriteToAuditTrailOnInsert
ON dbo.Roles
AFTER INSERT
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.RolesAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    RoleName,
    UpdatedBy
)
SELECT
    'Insert',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    i.Id,
    i.RoleName,
    i.UpdatedBy
FROM Inserted i;

END
GO



CREATE OR ALTER TRIGGER dbo.Roles_WriteToAuditTrailOnUpdate
ON dbo.Roles
AFTER UPDATE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.RolesAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    RoleName,
    UpdatedBy
)
SELECT
    'Update',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    d.Id,
    d.RoleName,
    d.UpdatedBy
FROM Deleted d
INNER JOIN Inserted i
ON d.Id=i.Id;

END
GO



CREATE OR ALTER TRIGGER dbo.Roles_WriteToAuditTrailOnDelete
ON dbo.Roles
AFTER DELETE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.RolesAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    RoleName,
    UpdatedBy
)
SELECT
    'Delete',
    GETUTCDATE(),
    SYSTEM_USER,
    d.Id,
    d.RoleName,
    d.UpdatedBy
FROM Deleted d;

END
GO



CREATE OR ALTER TRIGGER dbo.Users_WriteToAuditTrailOnInsert
ON dbo.Users
AFTER INSERT
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.UsersAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    RoleId,
    Username,
    PasswordHash,
    Email,
    IsActive,
    UpdatedBy
)
SELECT
    'Insert',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    i.Id,
    i.RoleId,
    i.Username,
    i.PasswordHash,
    i.Email,
    i.IsActive,
    i.UpdatedBy
FROM Inserted i;

END
GO



CREATE OR ALTER TRIGGER dbo.Users_WriteToAuditTrailOnUpdate
ON dbo.Users
AFTER UPDATE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.UsersAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    RoleId,
    Username,
    PasswordHash,
    Email,
    IsActive,
    UpdatedBy
)
SELECT
    'Update',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    d.Id,
    d.RoleId,
    d.Username,
    d.PasswordHash,
    d.Email,
    d.IsActive,
    d.UpdatedBy
FROM Deleted d
INNER JOIN Inserted i
ON d.Id=i.Id;

END
GO



CREATE OR ALTER TRIGGER dbo.Users_WriteToAuditTrailOnDelete
ON dbo.Users
AFTER DELETE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.UsersAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    RoleId,
    Username,
    PasswordHash,
    Email,
    IsActive,
    UpdatedBy
)
SELECT
    'Delete',
    GETUTCDATE(),
    SYSTEM_USER,
    d.Id,
    d.RoleId,
    d.Username,
    d.PasswordHash,
    d.Email,
    d.IsActive,
    d.UpdatedBy
FROM Deleted d;

END
GO



CREATE OR ALTER TRIGGER dbo.Customers_WriteToAuditTrailOnInsert
ON dbo.Customers
AFTER INSERT
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.CustomersAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    Name,
    Email,
    Phone,
    Address,
    PAN,
    Aadhaar,
    BusinessName,
    EmploymentType,
    Income,
    CreditScore,
    UpdatedBy
)
SELECT
    'Insert',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    i.Id,
    i.Name,
    i.Email,
    i.Phone,
    i.Address,
    i.PAN,
    i.Aadhaar,
    i.BusinessName,
    i.EmploymentType,
    i.Income,
    i.CreditScore,
    i.UpdatedBy
FROM Inserted i;

END
GO



CREATE OR ALTER TRIGGER dbo.Customers_WriteToAuditTrailOnUpdate
ON dbo.Customers
AFTER UPDATE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.CustomersAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    Name,
    Email,
    Phone,
    Address,
    PAN,
    Aadhaar,
    BusinessName,
    EmploymentType,
    Income,
    CreditScore,
    UpdatedBy
)
SELECT
    'Update',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    d.Id,
    d.Name,
    d.Email,
    d.Phone,
    d.Address,
    d.PAN,
    d.Aadhaar,
    d.BusinessName,
    d.EmploymentType,
    d.Income,
    d.CreditScore,
    d.UpdatedBy
FROM Deleted d
INNER JOIN Inserted i
ON d.Id=i.Id;

END
GO



CREATE OR ALTER TRIGGER dbo.Customers_WriteToAuditTrailOnDelete
ON dbo.Customers
AFTER DELETE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.CustomersAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    Name,
    Email,
    Phone,
    Address,
    PAN,
    Aadhaar,
    BusinessName,
    EmploymentType,
    Income,
    CreditScore,
    UpdatedBy
)
SELECT
    'Delete',
    GETUTCDATE(),
    SYSTEM_USER,
    d.Id,
    d.Name,
    d.Email,
    d.Phone,
    d.Address,
    d.PAN,
    d.Aadhaar,
    d.BusinessName,
    d.EmploymentType,
    d.Income,
    d.CreditScore,
    d.UpdatedBy
FROM Deleted d;

END
GO



CREATE OR ALTER TRIGGER dbo.LoanProducts_WriteToAuditTrailOnInsert
ON dbo.LoanProducts
AFTER INSERT
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.LoanProductsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    Name,
    InterestRate,
    TenureMonths,
    ProcessingFee,
    PenaltyRules,
    UpdatedBy
)
SELECT
    'Insert',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    i.Id,
    i.Name,
    i.InterestRate,
    i.TenureMonths,
    i.ProcessingFee,
    i.PenaltyRules,
    i.UpdatedBy
FROM Inserted i;

END
GO



CREATE OR ALTER TRIGGER dbo.LoanProducts_WriteToAuditTrailOnUpdate
ON dbo.LoanProducts
AFTER UPDATE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.LoanProductsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    Name,
    InterestRate,
    TenureMonths,
    ProcessingFee,
    PenaltyRules,
    UpdatedBy
)
SELECT
    'Update',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    d.Id,
    d.Name,
    d.InterestRate,
    d.TenureMonths,
    d.ProcessingFee,
    d.PenaltyRules,
    d.UpdatedBy
FROM Deleted d
INNER JOIN Inserted i
ON d.Id=i.Id;

END
GO



CREATE OR ALTER TRIGGER dbo.LoanProducts_WriteToAuditTrailOnDelete
ON dbo.LoanProducts
AFTER DELETE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.LoanProductsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    Name,
    InterestRate,
    TenureMonths,
    ProcessingFee,
    PenaltyRules,
    UpdatedBy
)
SELECT
    'Delete',
    GETUTCDATE(),
    SYSTEM_USER,
    d.Id,
    d.Name,
    d.InterestRate,
    d.TenureMonths,
    d.ProcessingFee,
    d.PenaltyRules,
    d.UpdatedBy
FROM Deleted d;

END
GO



CREATE OR ALTER TRIGGER dbo.Loans_WriteToAuditTrailOnInsert
ON dbo.Loans
AFTER INSERT
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.LoansAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanNumber,
    CustomerId,
    ProductId,
    Principal,
    InterestRate,
    Tenure,
    RepaymentFrequency,
    StartDate,
    EndDate,
    Status,
    UpdatedBy
)
SELECT
    'Insert',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    i.Id,
    i.LoanNumber,
    i.CustomerId,
    i.ProductId,
    i.Principal,
    i.InterestRate,
    i.Tenure,
    i.RepaymentFrequency,
    i.StartDate,
    i.EndDate,
    i.Status,
    i.UpdatedBy
FROM Inserted i;

END
GO



CREATE OR ALTER TRIGGER dbo.Loans_WriteToAuditTrailOnUpdate
ON dbo.Loans
AFTER UPDATE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.LoansAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanNumber,
    CustomerId,
    ProductId,
    Principal,
    InterestRate,
    Tenure,
    RepaymentFrequency,
    StartDate,
    EndDate,
    Status,
    UpdatedBy
)
SELECT
    'Update',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    d.Id,
    d.LoanNumber,
    d.CustomerId,
    d.ProductId,
    d.Principal,
    d.InterestRate,
    d.Tenure,
    d.RepaymentFrequency,
    d.StartDate,
    d.EndDate,
    d.Status,
    d.UpdatedBy
FROM Deleted d
INNER JOIN Inserted i
ON d.Id=i.Id;

END
GO



CREATE OR ALTER TRIGGER dbo.Loans_WriteToAuditTrailOnDelete
ON dbo.Loans
AFTER DELETE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.LoansAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanNumber,
    CustomerId,
    ProductId,
    Principal,
    InterestRate,
    Tenure,
    RepaymentFrequency,
    StartDate,
    EndDate,
    Status,
    UpdatedBy
)
SELECT
    'Delete',
    GETUTCDATE(),
    SYSTEM_USER,
    d.Id,
    d.LoanNumber,
    d.CustomerId,
    d.ProductId,
    d.Principal,
    d.InterestRate,
    d.Tenure,
    d.RepaymentFrequency,
    d.StartDate,
    d.EndDate,
    d.Status,
    d.UpdatedBy
FROM Deleted d;

END
GO



CREATE OR ALTER TRIGGER dbo.Disbursements_WriteToAuditTrailOnInsert
ON dbo.Disbursements
AFTER INSERT
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.DisbursementsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    Principal,
    BankAccount,
    TransactionDate,
    ReferenceNumber,
    UpdatedBy
)
SELECT
    'Insert',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    i.Id,
    i.LoanId,
    i.Principal,
    i.BankAccount,
    i.TransactionDate,
    i.ReferenceNumber,
    i.UpdatedBy
FROM Inserted i;

END
GO



CREATE OR ALTER TRIGGER dbo.Disbursements_WriteToAuditTrailOnUpdate
ON dbo.Disbursements
AFTER UPDATE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.DisbursementsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    Principal,
    BankAccount,
    TransactionDate,
    ReferenceNumber,
    UpdatedBy
)
SELECT
    'Update',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    d.Id,
    d.LoanId,
    d.Principal,
    d.BankAccount,
    d.TransactionDate,
    d.ReferenceNumber,
    d.UpdatedBy
FROM Deleted d
INNER JOIN Inserted i
ON d.Id=i.Id;

END
GO



CREATE OR ALTER TRIGGER dbo.Disbursements_WriteToAuditTrailOnDelete
ON dbo.Disbursements
AFTER DELETE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.DisbursementsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    Principal,
    BankAccount,
    TransactionDate,
    ReferenceNumber,
    UpdatedBy
)
SELECT
    'Delete',
    GETUTCDATE(),
    SYSTEM_USER,
    d.Id,
    d.LoanId,
    d.Principal,
    d.BankAccount,
    d.TransactionDate,
    d.ReferenceNumber,
    d.UpdatedBy
FROM Deleted d;

END
GO



CREATE OR ALTER TRIGGER dbo.RepaymentSchedules_WriteToAuditTrailOnInsert
ON dbo.RepaymentSchedules
AFTER INSERT
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.RepaymentSchedulesAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    EmiNo,
    DueDate,
    Principal,
    Interest,
    Outstanding,
    Status,
    UpdatedBy
)
SELECT
    'Insert',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    i.Id,
    i.LoanId,
    i.EmiNo,
    i.DueDate,
    i.Principal,
    i.Interest,
    i.Outstanding,
    i.Status,
    i.UpdatedBy
FROM Inserted i;

END
GO



CREATE OR ALTER TRIGGER dbo.RepaymentSchedules_WriteToAuditTrailOnUpdate
ON dbo.RepaymentSchedules
AFTER UPDATE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.RepaymentSchedulesAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    EmiNo,
    DueDate,
    Principal,
    Interest,
    Outstanding,
    Status,
    UpdatedBy
)
SELECT
    'Update',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    d.Id,
    d.LoanId,
    d.EmiNo,
    d.DueDate,
    d.Principal,
    d.Interest,
    d.Outstanding,
    d.Status,
    d.UpdatedBy
FROM Deleted d
INNER JOIN Inserted i
ON d.Id=i.Id;

END
GO



CREATE OR ALTER TRIGGER dbo.RepaymentSchedules_WriteToAuditTrailOnDelete
ON dbo.RepaymentSchedules
AFTER DELETE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.RepaymentSchedulesAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    EmiNo,
    DueDate,
    Principal,
    Interest,
    Outstanding,
    Status,
    UpdatedBy
)
SELECT
    'Delete',
    GETUTCDATE(),
    SYSTEM_USER,
    d.Id,
    d.LoanId,
    d.EmiNo,
    d.DueDate,
    d.Principal,
    d.Interest,
    d.Outstanding,
    d.Status,
    d.UpdatedBy
FROM Deleted d;

END
GO



CREATE OR ALTER TRIGGER dbo.Payments_WriteToAuditTrailOnInsert
ON dbo.Payments
AFTER INSERT
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.PaymentsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    PaymentDate,
    Amount,
    Mode,
    ReferenceNumber,
    Remarks,
    PaymentType,
    UpdatedBy
)
SELECT
    'Insert',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    i.Id,
    i.LoanId,
    i.PaymentDate,
    i.Amount,
    i.Mode,
    i.ReferenceNumber,
    i.Remarks,
    i.PaymentType,
    i.UpdatedBy
FROM Inserted i;

END
GO



CREATE OR ALTER TRIGGER dbo.Payments_WriteToAuditTrailOnUpdate
ON dbo.Payments
AFTER UPDATE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.PaymentsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    PaymentDate,
    Amount,
    Mode,
    ReferenceNumber,
    Remarks,
    PaymentType,
    UpdatedBy
)
SELECT
    'Update',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    d.Id,
    d.LoanId,
    d.PaymentDate,
    d.Amount,
    d.Mode,
    d.ReferenceNumber,
    d.Remarks,
    d.PaymentType,
    d.UpdatedBy
FROM Deleted d
INNER JOIN Inserted i
ON d.Id=i.Id;

END
GO



CREATE OR ALTER TRIGGER dbo.Payments_WriteToAuditTrailOnDelete
ON dbo.Payments
AFTER DELETE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.PaymentsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    PaymentDate,
    Amount,
    Mode,
    ReferenceNumber,
    Remarks,
    PaymentType,
    UpdatedBy
)
SELECT
    'Delete',
    GETUTCDATE(),
    SYSTEM_USER,
    d.Id,
    d.LoanId,
    d.PaymentDate,
    d.Amount,
    d.Mode,
    d.ReferenceNumber,
    d.Remarks,
    d.PaymentType,
    d.UpdatedBy
FROM Deleted d;

END
GO



CREATE OR ALTER TRIGGER dbo.Penalties_WriteToAuditTrailOnInsert
ON dbo.Penalties
AFTER INSERT
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.PenaltiesAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    Amount,
    AppliedDate,
    Reason,
    Status,
    UpdatedBy
)
SELECT
    'Insert',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    i.Id,
    i.LoanId,
    i.Amount,
    i.AppliedDate,
    i.Reason,
    i.Status,
    i.UpdatedBy
FROM Inserted i;

END
GO



CREATE OR ALTER TRIGGER dbo.Penalties_WriteToAuditTrailOnUpdate
ON dbo.Penalties
AFTER UPDATE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.PenaltiesAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    Amount,
    AppliedDate,
    Reason,
    Status,
    UpdatedBy
)
SELECT
    'Update',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    d.Id,
    d.LoanId,
    d.Amount,
    d.AppliedDate,
    d.Reason,
    d.Status,
    d.UpdatedBy
FROM Deleted d
INNER JOIN Inserted i
ON d.Id=i.Id;

END
GO



CREATE OR ALTER TRIGGER dbo.Penalties_WriteToAuditTrailOnDelete
ON dbo.Penalties
AFTER DELETE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.PenaltiesAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    Amount,
    AppliedDate,
    Reason,
    Status,
    UpdatedBy
)
SELECT
    'Delete',
    GETUTCDATE(),
    SYSTEM_USER,
    d.Id,
    d.LoanId,
    d.Amount,
    d.AppliedDate,
    d.Reason,
    d.Status,
    d.UpdatedBy
FROM Deleted d;

END
GO



CREATE OR ALTER TRIGGER dbo.InterestAccruals_WriteToAuditTrailOnInsert
ON dbo.InterestAccruals
AFTER INSERT
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.InterestAccrualsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    Outstanding,
    Interest,
    AccrualDate,
    UpdatedBy
)
SELECT
    'Insert',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    i.Id,
    i.LoanId,
    i.Outstanding,
    i.Interest,
    i.AccrualDate,
    i.UpdatedBy
FROM Inserted i;

END
GO



CREATE OR ALTER TRIGGER dbo.InterestAccruals_WriteToAuditTrailOnUpdate
ON dbo.InterestAccruals
AFTER UPDATE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.InterestAccrualsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    Outstanding,
    Interest,
    AccrualDate,
    UpdatedBy
)
SELECT
    'Update',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    d.Id,
    d.LoanId,
    d.Outstanding,
    d.Interest,
    d.AccrualDate,
    d.UpdatedBy
FROM Deleted d
INNER JOIN Inserted i
ON d.Id=i.Id;

END
GO



CREATE OR ALTER TRIGGER dbo.InterestAccruals_WriteToAuditTrailOnDelete
ON dbo.InterestAccruals
AFTER DELETE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.InterestAccrualsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    Outstanding,
    Interest,
    AccrualDate,
    UpdatedBy
)
SELECT
    'Delete',
    GETUTCDATE(),
    SYSTEM_USER,
    d.Id,
    d.LoanId,
    d.Outstanding,
    d.Interest,
    d.AccrualDate,
    d.UpdatedBy
FROM Deleted d;

END
GO



CREATE OR ALTER TRIGGER dbo.Statements_WriteToAuditTrailOnInsert
ON dbo.Statements
AFTER INSERT
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.StatementsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    OpeningBalance,
    ClosingBalance,
    GeneratedDate,
    FilePath,
    UpdatedBy
)
SELECT
    'Insert',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    i.Id,
    i.LoanId,
    i.OpeningBalance,
    i.ClosingBalance,
    i.GeneratedDate,
    i.FilePath,
    i.UpdatedBy
FROM Inserted i;

END
GO



CREATE OR ALTER TRIGGER dbo.Statements_WriteToAuditTrailOnUpdate
ON dbo.Statements
AFTER UPDATE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.StatementsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    OpeningBalance,
    ClosingBalance,
    GeneratedDate,
    FilePath,
    UpdatedBy
)
SELECT
    'Update',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    d.Id,
    d.LoanId,
    d.OpeningBalance,
    d.ClosingBalance,
    d.GeneratedDate,
    d.FilePath,
    d.UpdatedBy
FROM Deleted d
INNER JOIN Inserted i
ON d.Id=i.Id;

END
GO



CREATE OR ALTER TRIGGER dbo.Statements_WriteToAuditTrailOnDelete
ON dbo.Statements
AFTER DELETE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.StatementsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    LoanId,
    OpeningBalance,
    ClosingBalance,
    GeneratedDate,
    FilePath,
    UpdatedBy
)
SELECT
    'Delete',
    GETUTCDATE(),
    SYSTEM_USER,
    d.Id,
    d.LoanId,
    d.OpeningBalance,
    d.ClosingBalance,
    d.GeneratedDate,
    d.FilePath,
    d.UpdatedBy
FROM Deleted d;

END
GO



CREATE OR ALTER TRIGGER dbo.Documents_WriteToAuditTrailOnInsert
ON dbo.Documents
AFTER INSERT
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.DocumentsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    CustomerId,
    LoanId,
    DocumentType,
    FilePath,
    UploadedAt,
    UpdatedBy
)
SELECT
    'Insert',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    i.Id,
    i.CustomerId,
    i.LoanId,
    i.DocumentType,
    i.FilePath,
    i.UploadedAt,
    i.UpdatedBy
FROM Inserted i;

END
GO



CREATE OR ALTER TRIGGER dbo.Documents_WriteToAuditTrailOnUpdate
ON dbo.Documents
AFTER UPDATE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.DocumentsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    CustomerId,
    LoanId,
    DocumentType,
    FilePath,
    UploadedAt,
    UpdatedBy
)
SELECT
    'Update',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    d.Id,
    d.CustomerId,
    d.LoanId,
    d.DocumentType,
    d.FilePath,
    d.UploadedAt,
    d.UpdatedBy
FROM Deleted d
INNER JOIN Inserted i
ON d.Id=i.Id;

END
GO



CREATE OR ALTER TRIGGER dbo.Documents_WriteToAuditTrailOnDelete
ON dbo.Documents
AFTER DELETE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.DocumentsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    CustomerId,
    LoanId,
    DocumentType,
    FilePath,
    UploadedAt,
    UpdatedBy
)
SELECT
    'Delete',
    GETUTCDATE(),
    SYSTEM_USER,
    d.Id,
    d.CustomerId,
    d.LoanId,
    d.DocumentType,
    d.FilePath,
    d.UploadedAt,
    d.UpdatedBy
FROM Deleted d;

END
GO



CREATE OR ALTER TRIGGER dbo.Notifications_WriteToAuditTrailOnInsert
ON dbo.Notifications
AFTER INSERT
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.NotificationsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    UserId,
    Message,
    IsRead,
    CreatedAt,
    UpdatedBy
)
SELECT
    'Insert',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    i.Id,
    i.UserId,
    i.Message,
    i.IsRead,
    i.CreatedAt,
    i.UpdatedBy
FROM Inserted i;

END
GO



CREATE OR ALTER TRIGGER dbo.Notifications_WriteToAuditTrailOnUpdate
ON dbo.Notifications
AFTER UPDATE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.NotificationsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    UserId,
    Message,
    IsRead,
    CreatedAt,
    UpdatedBy
)
SELECT
    'Update',
    GETUTCDATE(),
    ISNULL(i.UpdatedBy,SYSTEM_USER),
    d.Id,
    d.UserId,
    d.Message,
    d.IsRead,
    d.CreatedAt,
    d.UpdatedBy
FROM Deleted d
INNER JOIN Inserted i
ON d.Id=i.Id;

END
GO



CREATE OR ALTER TRIGGER dbo.Notifications_WriteToAuditTrailOnDelete
ON dbo.Notifications
AFTER DELETE
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO dbo.NotificationsAuditTrail
(
    ChangeType,
    ChangedOn,
    ChangedBy,
    Id,
    UserId,
    Message,
    IsRead,
    CreatedAt,
    UpdatedBy
)
SELECT
    'Delete',
    GETUTCDATE(),
    SYSTEM_USER,
    d.Id,
    d.UserId,
    d.Message,
    d.IsRead,
    d.CreatedAt,
    d.UpdatedBy
FROM Deleted d;

END
GO



