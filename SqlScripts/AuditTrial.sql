-- =====================================================================
-- Table: Roles
-- =====================================================================
--remove table if it exists
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'RolesAuditTrail' AND type = 'U')
   DROP table [dbo].[RolesAuditTrail]
GO
--remove triggers if they exist
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Roles_WriteToAuditTrailOnUpdate' AND type = 'TR')
   DROP trigger [dbo].[Roles_WriteToAuditTrailOnUpdate]
GO
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Roles_WriteToAuditTrailOnDelete' AND type = 'TR')
   DROP trigger [dbo].[Roles_WriteToAuditTrailOnDelete]
GO

-- =====================================================================
-- Table: Users
-- =====================================================================
--remove table if it exists
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'UsersAuditTrail' AND type = 'U')
   DROP table [dbo].[UsersAuditTrail]
GO
--remove triggers if they exist
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Users_WriteToAuditTrailOnUpdate' AND type = 'TR')
   DROP trigger [dbo].[Users_WriteToAuditTrailOnUpdate]
GO
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Users_WriteToAuditTrailOnDelete' AND type = 'TR')
   DROP trigger [dbo].[Users_WriteToAuditTrailOnDelete]
GO

-- =====================================================================
-- Table: Customers
-- =====================================================================
--remove table if it exists
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'CustomersAuditTrail' AND type = 'U')
   DROP table [dbo].[CustomersAuditTrail]
GO
--remove triggers if they exist
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Customers_WriteToAuditTrailOnUpdate' AND type = 'TR')
   DROP trigger [dbo].[Customers_WriteToAuditTrailOnUpdate]
GO
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Customers_WriteToAuditTrailOnDelete' AND type = 'TR')
   DROP trigger [dbo].[Customers_WriteToAuditTrailOnDelete]
GO

-- =====================================================================
-- Table: LoanProducts
-- =====================================================================
--remove table if it exists
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'LoanProductsAuditTrail' AND type = 'U')
   DROP table [dbo].[LoanProductsAuditTrail]
GO
--remove triggers if they exist
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'LoanProducts_WriteToAuditTrailOnUpdate' AND type = 'TR')
   DROP trigger [dbo].[LoanProducts_WriteToAuditTrailOnUpdate]
GO
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'LoanProducts_WriteToAuditTrailOnDelete' AND type = 'TR')
   DROP trigger [dbo].[LoanProducts_WriteToAuditTrailOnDelete]
GO

-- =====================================================================
-- Table: Loans
-- =====================================================================
--remove table if it exists
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'LoansAuditTrail' AND type = 'U')
   DROP table [dbo].[LoansAuditTrail]
GO
--remove triggers if they exist
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Loans_WriteToAuditTrailOnUpdate' AND type = 'TR')
   DROP trigger [dbo].[Loans_WriteToAuditTrailOnUpdate]
GO
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Loans_WriteToAuditTrailOnDelete' AND type = 'TR')
   DROP trigger [dbo].[Loans_WriteToAuditTrailOnDelete]
GO

-- =====================================================================
-- Table: Disbursements
-- =====================================================================
--remove table if it exists
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'DisbursementsAuditTrail' AND type = 'U')
   DROP table [dbo].[DisbursementsAuditTrail]
GO
--remove triggers if they exist
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Disbursements_WriteToAuditTrailOnUpdate' AND type = 'TR')
   DROP trigger [dbo].[Disbursements_WriteToAuditTrailOnUpdate]
GO
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Disbursements_WriteToAuditTrailOnDelete' AND type = 'TR')
   DROP trigger [dbo].[Disbursements_WriteToAuditTrailOnDelete]
GO

-- =====================================================================
-- Table: RepaymentSchedules
-- =====================================================================
--remove table if it exists
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'RepaymentSchedulesAuditTrail' AND type = 'U')
   DROP table [dbo].[RepaymentSchedulesAuditTrail]
GO
--remove triggers if they exist
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'RepaymentSchedules_WriteToAuditTrailOnUpdate' AND type = 'TR')
   DROP trigger [dbo].[RepaymentSchedules_WriteToAuditTrailOnUpdate]
GO
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'RepaymentSchedules_WriteToAuditTrailOnDelete' AND type = 'TR')
   DROP trigger [dbo].[RepaymentSchedules_WriteToAuditTrailOnDelete]
GO

-- =====================================================================
-- Table: Payments
-- =====================================================================
--remove table if it exists
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'PaymentsAuditTrail' AND type = 'U')
   DROP table [dbo].[PaymentsAuditTrail]
GO
--remove triggers if they exist
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Payments_WriteToAuditTrailOnUpdate' AND type = 'TR')
   DROP trigger [dbo].[Payments_WriteToAuditTrailOnUpdate]
GO
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Payments_WriteToAuditTrailOnDelete' AND type = 'TR')
   DROP trigger [dbo].[Payments_WriteToAuditTrailOnDelete]
GO

-- =====================================================================
-- Table: Penalties
-- =====================================================================
--remove table if it exists
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'PenaltiesAuditTrail' AND type = 'U')
   DROP table [dbo].[PenaltiesAuditTrail]
GO
--remove triggers if they exist
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Penalties_WriteToAuditTrailOnUpdate' AND type = 'TR')
   DROP trigger [dbo].[Penalties_WriteToAuditTrailOnUpdate]
GO
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Penalties_WriteToAuditTrailOnDelete' AND type = 'TR')
   DROP trigger [dbo].[Penalties_WriteToAuditTrailOnDelete]
GO

-- =====================================================================
-- Table: InterestAccruals
-- =====================================================================
--remove table if it exists
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'InterestAccrualsAuditTrail' AND type = 'U')
   DROP table [dbo].[InterestAccrualsAuditTrail]
GO
--remove triggers if they exist
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'InterestAccruals_WriteToAuditTrailOnUpdate' AND type = 'TR')
   DROP trigger [dbo].[InterestAccruals_WriteToAuditTrailOnUpdate]
GO
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'InterestAccruals_WriteToAuditTrailOnDelete' AND type = 'TR')
   DROP trigger [dbo].[InterestAccruals_WriteToAuditTrailOnDelete]
GO

-- =====================================================================
-- Table: Statements
-- =====================================================================
--remove table if it exists
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'StatementsAuditTrail' AND type = 'U')
   DROP table [dbo].[StatementsAuditTrail]
GO
--remove triggers if they exist
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Statements_WriteToAuditTrailOnUpdate' AND type = 'TR')
   DROP trigger [dbo].[Statements_WriteToAuditTrailOnUpdate]
GO
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Statements_WriteToAuditTrailOnDelete' AND type = 'TR')
   DROP trigger [dbo].[Statements_WriteToAuditTrailOnDelete]
GO

-- =====================================================================
-- Table: Documents
-- =====================================================================
--remove table if it exists
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'DocumentsAuditTrail' AND type = 'U')
   DROP table [dbo].[DocumentsAuditTrail]
GO
--remove triggers if they exist
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Documents_WriteToAuditTrailOnUpdate' AND type = 'TR')
   DROP trigger [dbo].[Documents_WriteToAuditTrailOnUpdate]
GO
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Documents_WriteToAuditTrailOnDelete' AND type = 'TR')
   DROP trigger [dbo].[Documents_WriteToAuditTrailOnDelete]
GO

-- =====================================================================
-- Table: Notifications
-- =====================================================================
--remove table if it exists
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'NotificationsAuditTrail' AND type = 'U')
   DROP table [dbo].[NotificationsAuditTrail]
GO
--remove triggers if they exist
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Notifications_WriteToAuditTrailOnUpdate' AND type = 'TR')
   DROP trigger [dbo].[Notifications_WriteToAuditTrailOnUpdate]
GO
IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'Notifications_WriteToAuditTrailOnDelete' AND type = 'TR')
   DROP trigger [dbo].[Notifications_WriteToAuditTrailOnDelete]
GO

--
--[dbo].[Roles]
--
select top 0
  cast (null as [nvarchar](255)) [ChangeType],
  cast (null as [DateTime2]) [ChangedOn],
  cast (null as [nvarchar](255)) [ChangedBy], d.*
into [dbo].[RolesAuditTrail] from [dbo].[Roles] d
  LEFT JOIN [dbo].[Roles] ON 1 = 0
GO

CREATE TRIGGER [dbo].[Roles_WriteToAuditTrailOnUpdate]
ON [dbo].[Roles]
AFTER Update
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[RolesAuditTrail]
  select 'Update' ChangeType, GETUTCDATE() ChangedOn, i.UpdatedBy ChangedBy, d.*
from Deleted d
inner join inserted i
    on i.Id = d.Id
END
GO

CREATE TRIGGER [dbo].[Roles_WriteToAuditTrailOnDelete]
ON [dbo].[Roles]
AFTER Delete
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[RolesAuditTrail]
  select 'Warning!!! Row was deleted!! Should not happen!' ChangeType,
         GETUTCDATE() ChangedOn,
         SUSER_SNAME() ChangedBy,
         d.*
from Deleted d;

END
GO

--
--[dbo].[Users]
--
select top 0
  cast (null as [nvarchar](255)) [ChangeType],
  cast (null as [DateTime2]) [ChangedOn],
  cast (null as [nvarchar](255)) [ChangedBy], d.*
into [dbo].[UsersAuditTrail] from [dbo].[Users] d
  LEFT JOIN [dbo].[Users] ON 1 = 0
GO

CREATE TRIGGER [dbo].[Users_WriteToAuditTrailOnUpdate]
ON [dbo].[Users]
AFTER Update
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[UsersAuditTrail]
  select 'Update' ChangeType, GETUTCDATE() ChangedOn, i.UpdatedBy ChangedBy, d.*
from Deleted d
inner join inserted i
    on i.Id = d.Id
END
GO

CREATE TRIGGER [dbo].[Users_WriteToAuditTrailOnDelete]
ON [dbo].[Users]
AFTER Delete
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[UsersAuditTrail]
  select 'Warning!!! Row was deleted!! Should not happen!' ChangeType,
         GETUTCDATE() ChangedOn,
         SUSER_SNAME() ChangedBy,
         d.*
from Deleted d;

END
GO

--
--[dbo].[Customers]
--
select top 0
  cast (null as [nvarchar](255)) [ChangeType],
  cast (null as [DateTime2]) [ChangedOn],
  cast (null as [nvarchar](255)) [ChangedBy], d.*
into [dbo].[CustomersAuditTrail] from [dbo].[Customers] d
  LEFT JOIN [dbo].[Customers] ON 1 = 0
GO

CREATE TRIGGER [dbo].[Customers_WriteToAuditTrailOnUpdate]
ON [dbo].[Customers]
AFTER Update
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[CustomersAuditTrail]
  select 'Update' ChangeType, GETUTCDATE() ChangedOn, i.UpdatedBy ChangedBy, d.*
from Deleted d
inner join inserted i
    on i.Id = d.Id
END
GO

CREATE TRIGGER [dbo].[Customers_WriteToAuditTrailOnDelete]
ON [dbo].[Customers]
AFTER Delete
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[CustomersAuditTrail]
  select 'Warning!!! Row was deleted!! Should not happen!' ChangeType,
         GETUTCDATE() ChangedOn,
         SUSER_SNAME() ChangedBy,
         d.*
from Deleted d;

END
GO

--
--[dbo].[LoanProducts]
--
select top 0
  cast (null as [nvarchar](255)) [ChangeType],
  cast (null as [DateTime2]) [ChangedOn],
  cast (null as [nvarchar](255)) [ChangedBy], d.*
into [dbo].[LoanProductsAuditTrail] from [dbo].[LoanProducts] d
  LEFT JOIN [dbo].[LoanProducts] ON 1 = 0
GO

CREATE TRIGGER [dbo].[LoanProducts_WriteToAuditTrailOnUpdate]
ON [dbo].[LoanProducts]
AFTER Update
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[LoanProductsAuditTrail]
  select 'Update' ChangeType, GETUTCDATE() ChangedOn, i.UpdatedBy ChangedBy, d.*
from Deleted d
inner join inserted i
    on i.Id = d.Id
END
GO

CREATE TRIGGER [dbo].[LoanProducts_WriteToAuditTrailOnDelete]
ON [dbo].[LoanProducts]
AFTER Delete
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[LoanProductsAuditTrail]
  select 'Warning!!! Row was deleted!! Should not happen!' ChangeType,
         GETUTCDATE() ChangedOn,
         SUSER_SNAME() ChangedBy,
         d.*
from Deleted d;

END
GO

--
--[dbo].[Loans]
--
select top 0
  cast (null as [nvarchar](255)) [ChangeType],
  cast (null as [DateTime2]) [ChangedOn],
  cast (null as [nvarchar](255)) [ChangedBy], d.*
into [dbo].[LoansAuditTrail] from [dbo].[Loans] d
  LEFT JOIN [dbo].[Loans] ON 1 = 0
GO

CREATE TRIGGER [dbo].[Loans_WriteToAuditTrailOnUpdate]
ON [dbo].[Loans]
AFTER Update
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[LoansAuditTrail]
  select 'Update' ChangeType, GETUTCDATE() ChangedOn, i.UpdatedBy ChangedBy, d.*
from Deleted d
inner join inserted i
    on i.Id = d.Id
END
GO

CREATE TRIGGER [dbo].[Loans_WriteToAuditTrailOnDelete]
ON [dbo].[Loans]
AFTER Delete
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[LoansAuditTrail]
  select 'Warning!!! Row was deleted!! Should not happen!' ChangeType,
         GETUTCDATE() ChangedOn,
         SUSER_SNAME() ChangedBy,
         d.*
from Deleted d;

END
GO

--
--[dbo].[Disbursements]
--
select top 0
  cast (null as [nvarchar](255)) [ChangeType],
  cast (null as [DateTime2]) [ChangedOn],
  cast (null as [nvarchar](255)) [ChangedBy], d.*
into [dbo].[DisbursementsAuditTrail] from [dbo].[Disbursements] d
  LEFT JOIN [dbo].[Disbursements] ON 1 = 0
GO

CREATE TRIGGER [dbo].[Disbursements_WriteToAuditTrailOnUpdate]
ON [dbo].[Disbursements]
AFTER Update
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[DisbursementsAuditTrail]
  select 'Update' ChangeType, GETUTCDATE() ChangedOn, i.UpdatedBy ChangedBy, d.*
from Deleted d
inner join inserted i
    on i.Id = d.Id
END
GO

CREATE TRIGGER [dbo].[Disbursements_WriteToAuditTrailOnDelete]
ON [dbo].[Disbursements]
AFTER Delete
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[DisbursementsAuditTrail]
  select 'Warning!!! Row was deleted!! Should not happen!' ChangeType,
         GETUTCDATE() ChangedOn,
         SUSER_SNAME() ChangedBy,
         d.*
from Deleted d;

END
GO

--
--[dbo].[RepaymentSchedules]
--
select top 0
  cast (null as [nvarchar](255)) [ChangeType],
  cast (null as [DateTime2]) [ChangedOn],
  cast (null as [nvarchar](255)) [ChangedBy], d.*
into [dbo].[RepaymentSchedulesAuditTrail] from [dbo].[RepaymentSchedules] d
  LEFT JOIN [dbo].[RepaymentSchedules] ON 1 = 0
GO

CREATE TRIGGER [dbo].[RepaymentSchedules_WriteToAuditTrailOnUpdate]
ON [dbo].[RepaymentSchedules]
AFTER Update
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[RepaymentSchedulesAuditTrail]
  select 'Update' ChangeType, GETUTCDATE() ChangedOn, i.UpdatedBy ChangedBy, d.*
from Deleted d
inner join inserted i
    on i.Id = d.Id
END
GO

CREATE TRIGGER [dbo].[RepaymentSchedules_WriteToAuditTrailOnDelete]
ON [dbo].[RepaymentSchedules]
AFTER Delete
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[RepaymentSchedulesAuditTrail]
  select 'Warning!!! Row was deleted!! Should not happen!' ChangeType,
         GETUTCDATE() ChangedOn,
         SUSER_SNAME() ChangedBy,
         d.*
from Deleted d;

END
GO

--
--[dbo].[Payments]
--
select top 0
  cast (null as [nvarchar](255)) [ChangeType],
  cast (null as [DateTime2]) [ChangedOn],
  cast (null as [nvarchar](255)) [ChangedBy], d.*
into [dbo].[PaymentsAuditTrail] from [dbo].[Payments] d
  LEFT JOIN [dbo].[Payments] ON 1 = 0
GO

CREATE TRIGGER [dbo].[Payments_WriteToAuditTrailOnUpdate]
ON [dbo].[Payments]
AFTER Update
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[PaymentsAuditTrail]
  select 'Update' ChangeType, GETUTCDATE() ChangedOn, i.UpdatedBy ChangedBy, d.*
from Deleted d
inner join inserted i
    on i.Id = d.Id
END
GO

CREATE TRIGGER [dbo].[Payments_WriteToAuditTrailOnDelete]
ON [dbo].[Payments]
AFTER Delete
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[PaymentsAuditTrail]
  select 'Warning!!! Row was deleted!! Should not happen!' ChangeType,
         GETUTCDATE() ChangedOn,
         SUSER_SNAME() ChangedBy,
         d.*
from Deleted d;

END
GO

--
--[dbo].[Penalties]
--
select top 0
  cast (null as [nvarchar](255)) [ChangeType],
  cast (null as [DateTime2]) [ChangedOn],
  cast (null as [nvarchar](255)) [ChangedBy], d.*
into [dbo].[PenaltiesAuditTrail] from [dbo].[Penalties] d
  LEFT JOIN [dbo].[Penalties] ON 1 = 0
GO

CREATE TRIGGER [dbo].[Penalties_WriteToAuditTrailOnUpdate]
ON [dbo].[Penalties]
AFTER Update
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[PenaltiesAuditTrail]
  select 'Update' ChangeType, GETUTCDATE() ChangedOn, i.UpdatedBy ChangedBy, d.*
from Deleted d
inner join inserted i
    on i.Id = d.Id
END
GO

CREATE TRIGGER [dbo].[Penalties_WriteToAuditTrailOnDelete]
ON [dbo].[Penalties]
AFTER Delete
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[PenaltiesAuditTrail]
  select 'Warning!!! Row was deleted!! Should not happen!' ChangeType,
         GETUTCDATE() ChangedOn,
         SUSER_SNAME() ChangedBy,
         d.*
from Deleted d;

END
GO

--
--[dbo].[InterestAccruals]
--
select top 0
  cast (null as [nvarchar](255)) [ChangeType],
  cast (null as [DateTime2]) [ChangedOn],
  cast (null as [nvarchar](255)) [ChangedBy], d.*
into [dbo].[InterestAccrualsAuditTrail] from [dbo].[InterestAccruals] d
  LEFT JOIN [dbo].[InterestAccruals] ON 1 = 0
GO

CREATE TRIGGER [dbo].[InterestAccruals_WriteToAuditTrailOnUpdate]
ON [dbo].[InterestAccruals]
AFTER Update
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[InterestAccrualsAuditTrail]
  select 'Update' ChangeType, GETUTCDATE() ChangedOn, i.UpdatedBy ChangedBy, d.*
from Deleted d
inner join inserted i
    on i.Id = d.Id
END
GO

CREATE TRIGGER [dbo].[InterestAccruals_WriteToAuditTrailOnDelete]
ON [dbo].[InterestAccruals]
AFTER Delete
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[InterestAccrualsAuditTrail]
  select 'Warning!!! Row was deleted!! Should not happen!' ChangeType,
         GETUTCDATE() ChangedOn,
         SUSER_SNAME() ChangedBy,
         d.*
from Deleted d;

END
GO

--
--[dbo].[Statements]
--
select top 0
  cast (null as [nvarchar](255)) [ChangeType],
  cast (null as [DateTime2]) [ChangedOn],
  cast (null as [nvarchar](255)) [ChangedBy], d.*
into [dbo].[StatementsAuditTrail] from [dbo].[Statements] d
  LEFT JOIN [dbo].[Statements] ON 1 = 0
GO

CREATE TRIGGER [dbo].[Statements_WriteToAuditTrailOnUpdate]
ON [dbo].[Statements]
AFTER Update
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[StatementsAuditTrail]
  select 'Update' ChangeType, GETUTCDATE() ChangedOn, i.UpdatedBy ChangedBy, d.*
from Deleted d
inner join inserted i
    on i.Id = d.Id
END
GO

CREATE TRIGGER [dbo].[Statements_WriteToAuditTrailOnDelete]
ON [dbo].[Statements]
AFTER Delete
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[StatementsAuditTrail]
  select 'Warning!!! Row was deleted!! Should not happen!' ChangeType,
         GETUTCDATE() ChangedOn,
         SUSER_SNAME() ChangedBy,
         d.*
from Deleted d;

END
GO

--
--[dbo].[Documents]
--
select top 0
  cast (null as [nvarchar](255)) [ChangeType],
  cast (null as [DateTime2]) [ChangedOn],
  cast (null as [nvarchar](255)) [ChangedBy], d.*
into [dbo].[DocumentsAuditTrail] from [dbo].[Documents] d
  LEFT JOIN [dbo].[Documents] ON 1 = 0
GO

CREATE TRIGGER [dbo].[Documents_WriteToAuditTrailOnUpdate]
ON [dbo].[Documents]
AFTER Update
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[DocumentsAuditTrail]
  select 'Update' ChangeType, GETUTCDATE() ChangedOn, i.UpdatedBy ChangedBy, d.*
from Deleted d
inner join inserted i
    on i.Id = d.Id
END
GO

CREATE TRIGGER [dbo].[Documents_WriteToAuditTrailOnDelete]
ON [dbo].[Documents]
AFTER Delete
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[DocumentsAuditTrail]
  select 'Warning!!! Row was deleted!! Should not happen!' ChangeType,
         GETUTCDATE() ChangedOn,
         SUSER_SNAME() ChangedBy,
         d.*
from Deleted d;

END
GO

--
--[dbo].[Notifications]
--
select top 0
  cast (null as [nvarchar](255)) [ChangeType],
  cast (null as [DateTime2]) [ChangedOn],
  cast (null as [nvarchar](255)) [ChangedBy], d.*
into [dbo].[NotificationsAuditTrail] from [dbo].[Notifications] d
  LEFT JOIN [dbo].[Notifications] ON 1 = 0
GO

CREATE TRIGGER [dbo].[Notifications_WriteToAuditTrailOnUpdate]
ON [dbo].[Notifications]
AFTER Update
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[NotificationsAuditTrail]
  select 'Update' ChangeType, GETUTCDATE() ChangedOn, i.UpdatedBy ChangedBy, d.*
from Deleted d
inner join inserted i
    on i.Id = d.Id
END
GO

CREATE TRIGGER [dbo].[Notifications_WriteToAuditTrailOnDelete]
ON [dbo].[Notifications]
AFTER Delete
AS
BEGIN
SET NOCOUNT ON;
  insert into [dbo].[NotificationsAuditTrail]
  select 'Warning!!! Row was deleted!! Should not happen!' ChangeType,
         GETUTCDATE() ChangedOn,
         SUSER_SNAME() ChangedBy,
         d.*
from Deleted d;

END
GO

USE LoanServicingSystem;
GO

--=========================================================
-- Drop View
--=========================================================

IF OBJECT_ID('dbo.vw_AuditTrail', 'V') IS NOT NULL
    DROP VIEW dbo.vw_AuditTrail;
GO

--=========================================================
-- Create View
--=========================================================

CREATE VIEW dbo.vw_AuditTrail
AS

--=========================================================
-- Roles
--=========================================================
SELECT
    ChangedOn,
    ChangedBy,
    'Roles' AS Module,
    ChangeType
FROM dbo.RolesAuditTrail

UNION ALL

--=========================================================
-- Users
--=========================================================
SELECT
    ChangedOn,
    ChangedBy,
    'Users',
    ChangeType
FROM dbo.UsersAuditTrail

UNION ALL

--=========================================================
-- Customers
--=========================================================
SELECT
    ChangedOn,
    ChangedBy,
    'Customers',
    ChangeType
FROM dbo.CustomersAuditTrail

UNION ALL

--=========================================================
-- Loan Products
--=========================================================
SELECT
    ChangedOn,
    ChangedBy,
    'Loan Products',
    ChangeType
FROM dbo.LoanProductsAuditTrail

UNION ALL

--=========================================================
-- Loans
--=========================================================
SELECT
    ChangedOn,
    ChangedBy,
    'Loans',
    ChangeType
FROM dbo.LoansAuditTrail

UNION ALL

--=========================================================
-- Disbursements
--=========================================================
SELECT
    ChangedOn,
    ChangedBy,
    'Disbursements',
    ChangeType
FROM dbo.DisbursementsAuditTrail

UNION ALL

--=========================================================
-- Repayment Schedules
--=========================================================
SELECT
    ChangedOn,
    ChangedBy,
    'Repayment Schedules',
    ChangeType
FROM dbo.RepaymentSchedulesAuditTrail

UNION ALL

--=========================================================
-- Payments
--=========================================================
SELECT
    ChangedOn,
    ChangedBy,
    'Payments',
    ChangeType
FROM dbo.PaymentsAuditTrail

UNION ALL

--=========================================================
-- Penalties
--=========================================================
SELECT
    ChangedOn,
    ChangedBy,
    'Penalties',
    ChangeType
FROM dbo.PenaltiesAuditTrail

UNION ALL

--=========================================================
-- Interest Accruals
--=========================================================
SELECT
    ChangedOn,
    ChangedBy,
    'Interest Accruals',
    ChangeType
FROM dbo.InterestAccrualsAuditTrail

UNION ALL

--=========================================================
-- Statements
--=========================================================
SELECT
    ChangedOn,
    ChangedBy,
    'Statements',
    ChangeType
FROM dbo.StatementsAuditTrail

UNION ALL

--=========================================================
-- Documents
--=========================================================
SELECT
    ChangedOn,
    ChangedBy,
    'Documents',
    ChangeType
FROM dbo.DocumentsAuditTrail

UNION ALL

--=========================================================
-- Notifications
--=========================================================
SELECT
    ChangedOn,
    ChangedBy,
    'Notifications',
    ChangeType
FROM dbo.NotificationsAuditTrail;
GO


