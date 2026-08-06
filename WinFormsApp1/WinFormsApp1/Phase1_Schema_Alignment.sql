-- ============================================================
-- PHASE 1: Schema alignment for PR → Clubbing → PO flow
-- Safe / idempotent script for ERP_Database
-- ============================================================

PRINT 'Phase 1 schema alignment starting...';
GO

-- ------------------------------------------------------------
-- 1. PurchaseRequest header columns used by the app
-- ------------------------------------------------------------
IF COL_LENGTH('PurchaseRequest', 'VendorName') IS NULL
    ALTER TABLE PurchaseRequest ADD VendorName NVARCHAR(128) NULL;
GO

IF COL_LENGTH('PurchaseRequest', 'Status') IS NULL
    ALTER TABLE PurchaseRequest ADD Status NVARCHAR(30) NULL CONSTRAINT DF_PR_Status DEFAULT 'Pending';
GO

IF COL_LENGTH('PurchaseRequest', 'IsClubbed') IS NULL
    ALTER TABLE PurchaseRequest ADD IsClubbed BIT NULL CONSTRAINT DF_PR_IsClubbed DEFAULT 0;
GO

IF COL_LENGTH('PurchaseRequest', 'ClubbedFromPRIDs') IS NULL
    ALTER TABLE PurchaseRequest ADD ClubbedFromPRIDs NVARCHAR(500) NULL;
GO

IF COL_LENGTH('PurchaseRequest', 'RequestedBy') IS NULL
    ALTER TABLE PurchaseRequest ADD RequestedBy NVARCHAR(64) NULL;
GO

IF COL_LENGTH('PurchaseRequest', 'RequestDate') IS NULL
    ALTER TABLE PurchaseRequest ADD RequestDate DATETIME NULL CONSTRAINT DF_PR_RequestDate DEFAULT GETDATE();
GO

IF COL_LENGTH('PurchaseRequest', 'ApprovedBy') IS NULL
    ALTER TABLE PurchaseRequest ADD ApprovedBy NVARCHAR(64) NULL;
GO

IF COL_LENGTH('PurchaseRequest', 'ApprovedDate') IS NULL
    ALTER TABLE PurchaseRequest ADD ApprovedDate DATETIME NULL;
GO

IF COL_LENGTH('PurchaseRequest', 'RejectionReason') IS NULL
    ALTER TABLE PurchaseRequest ADD RejectionReason NVARCHAR(500) NULL;
GO

IF COL_LENGTH('PurchaseRequest', 'HoldReason') IS NULL
    ALTER TABLE PurchaseRequest ADD HoldReason NVARCHAR(500) NULL;
GO

IF COL_LENGTH('PurchaseRequest', 'Remarks') IS NULL
    ALTER TABLE PurchaseRequest ADD Remarks NVARCHAR(500) NULL;
GO

IF COL_LENGTH('PurchaseRequest', 'ModifiedBy') IS NULL
    ALTER TABLE PurchaseRequest ADD ModifiedBy NVARCHAR(64) NULL;
GO

IF COL_LENGTH('PurchaseRequest', 'ModifiedDate') IS NULL
    ALTER TABLE PurchaseRequest ADD ModifiedDate DATETIME NULL;
GO

IF COL_LENGTH('PurchaseRequest', 'CreatedBy') IS NULL
    ALTER TABLE PurchaseRequest ADD CreatedBy NVARCHAR(64) NULL;
GO

UPDATE PurchaseRequest SET Status = 'Pending' WHERE Status IS NULL;
UPDATE PurchaseRequest SET IsClubbed = 0 WHERE IsClubbed IS NULL;
GO

-- ------------------------------------------------------------
-- 2. Detail table used by current C# code: PurchaseRequestDetailNew
-- ------------------------------------------------------------
IF OBJECT_ID('PurchaseRequestDetailNew', 'U') IS NULL
BEGIN
    CREATE TABLE PurchaseRequestDetailNew
    (
        DetailID              INT IDENTITY(1,1) PRIMARY KEY,
        PRID                  NVARCHAR(32) NULL,
        PRNumber              NVARCHAR(32) NOT NULL,
        ProjectBOMCode        NVARCHAR(64) NULL,
        ProjectCode           NVARCHAR(32) NULL,
        ProductCode           NVARCHAR(64) NULL,
        ProductNo             NVARCHAR(64) NULL,
        ItemCode              NVARCHAR(64) NOT NULL,
        ItemDescription       NVARCHAR(255) NULL,
        Specification         NVARCHAR(255) NULL,
        Make                  NVARCHAR(128) NULL,
        MfgPartNo             NVARCHAR(128) NULL,
        Quantity              DECIMAL(18,3) DEFAULT 0,
        UOM                   NVARCHAR(32) NULL,
        UnitCost              DECIMAL(18,2) DEFAULT 0,
        TotalCost             DECIMAL(18,2) DEFAULT 0,
        VendorCode            NVARCHAR(64) NULL,
        VendorName            NVARCHAR(128) NULL,
        DrawingNo             NVARCHAR(128) NULL,
        Location              NVARCHAR(128) NULL,
        HSNCode               NVARCHAR(64) NULL,
        BOMCode               NVARCHAR(64) NULL,
        BOMQuantity           DECIMAL(18,3) NULL,
        AlreadyPurchasedQty   DECIMAL(18,3) NULL,
        BalanceQty            DECIMAL(18,3) NULL,
        LineStatus            NVARCHAR(40) NULL CONSTRAINT DF_PRDN_LineStatus DEFAULT 'Pending',
        Remarks               NVARCHAR(255) NULL,
        CreatedBy             NVARCHAR(64) NULL,
        CreatedDate           DATETIME NULL CONSTRAINT DF_PRDN_CreatedDate DEFAULT GETDATE(),
        ModifiedBy            NVARCHAR(64) NULL,
        ModifiedDate          DATETIME NULL
    );

    CREATE INDEX IX_PRDN_PRNumber ON PurchaseRequestDetailNew(PRNumber);
    CREATE INDEX IX_PRDN_ItemCode ON PurchaseRequestDetailNew(ItemCode);
    CREATE INDEX IX_PRDN_LineStatus ON PurchaseRequestDetailNew(LineStatus);
END
GO

IF COL_LENGTH('PurchaseRequestDetailNew', 'LineStatus') IS NULL
    ALTER TABLE PurchaseRequestDetailNew ADD LineStatus NVARCHAR(40) NULL CONSTRAINT DF_PRDN_LineStatus2 DEFAULT 'Pending';
GO

IF COL_LENGTH('PurchaseRequestDetailNew', 'BOMCode') IS NULL
    ALTER TABLE PurchaseRequestDetailNew ADD BOMCode NVARCHAR(64) NULL;
GO

IF COL_LENGTH('PurchaseRequestDetailNew', 'ModifiedDate') IS NULL
    ALTER TABLE PurchaseRequestDetailNew ADD ModifiedDate DATETIME NULL;
GO

UPDATE PurchaseRequestDetailNew SET LineStatus = 'Pending' WHERE LineStatus IS NULL OR LTRIM(RTRIM(LineStatus)) = '';
GO

-- ------------------------------------------------------------
-- 3. Helper view of allowed PR statuses (documentation)
-- ------------------------------------------------------------
/*
  Pending
  Approved
  Rejected
  On Hold
  Clubbed
  Partially Converted
  Fully Converted
*/
PRINT 'Phase 1 schema alignment completed.';
GO
