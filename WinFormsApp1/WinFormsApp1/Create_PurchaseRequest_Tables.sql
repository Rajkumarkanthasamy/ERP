-- ============================================================
-- PURCHASE REQUEST TABLES - Complete Schema
-- For: ERP_Database
-- Created: 2026-07-17
-- ============================================================

-- Drop existing tables if they exist (for clean recreation)
IF OBJECT_ID('PurchaseRequestDetail', 'U') IS NOT NULL DROP TABLE PurchaseRequestDetail;
IF OBJECT_ID('PurchaseRequest', 'U') IS NOT NULL DROP TABLE PurchaseRequest;
IF OBJECT_ID('BOMtoPRConversion', 'U') IS NOT NULL DROP TABLE BOMtoPRConversion;
GO

-- ============================================================
-- 1. PURCHASE REQUEST HEADER TABLE
-- ============================================================
CREATE TABLE PurchaseRequest
(
    PRID                    INT IDENTITY(1,1) PRIMARY KEY,
    PRNumber                NVARCHAR(32) NOT NULL UNIQUE,
    ProjectCode             NVARCHAR(32) NOT NULL,
    ProductNo               NVARCHAR(32) NULL,
    VendorCode              NVARCHAR(32) NOT NULL,
    TotalAmount             DECIMAL(18,2) DEFAULT 0,
    Status                  NVARCHAR(20) DEFAULT 'Pending',     -- Pending, Approved, Rejected, On Hold
    RequestedBy             NVARCHAR(64) NULL,
    RequestDate             DATETIME DEFAULT GETDATE(),
    ApprovedBy              NVARCHAR(64) NULL,
    ApprovedDate            DATETIME NULL,
    RejectionReason         NVARCHAR(500) NULL,
    HoldReason              NVARCHAR(500) NULL,
    Remarks                 NVARCHAR(500) NULL,
    IsClubbed               BIT DEFAULT 0,                    -- 1 if this PR was created by clubbing multiple PRs
    ClubbedFromPRIDs        NVARCHAR(255) NULL,                 -- Comma-separated PRIDs if clubbed
    CreatedDate             DATETIME DEFAULT GETDATE(),
    ModifiedDate            DATETIME DEFAULT GETDATE(),
    CreatedBy               NVARCHAR(64) NULL,
    ModifiedBy              NVARCHAR(64) NULL
);
GO

-- Index for faster filtering
CREATE INDEX IX_PurchaseRequest_Status ON PurchaseRequest(Status);
CREATE INDEX IX_PurchaseRequest_ProjectCode ON PurchaseRequest(ProjectCode);
CREATE INDEX IX_PurchaseRequest_VendorCode ON PurchaseRequest(VendorCode);
CREATE INDEX IX_PurchaseRequest_RequestDate ON PurchaseRequest(RequestDate DESC);
GO

-- ============================================================
-- 2. PURCHASE REQUEST DETAIL (LINE ITEMS) TABLE
-- ============================================================
CREATE TABLE PurchaseRequestDetail
(
    DetailID                INT IDENTITY(1,1) PRIMARY KEY,
    PRID                    INT NOT NULL,
    ItemCode                NVARCHAR(32) NOT NULL,
    ItemDescription         NVARCHAR(255) NULL,
    Quantity                DECIMAL(18,3) DEFAULT 0,
    UnitCost                DECIMAL(18,2) DEFAULT 0,
    TotalCost               DECIMAL(18,2) DEFAULT 0,            -- Computed: Quantity * UnitCost
    UOM                     NVARCHAR(15) NULL,                  -- Units of Measure
    BOMProjectCode          NVARCHAR(32) NULL,                  -- Reference to BOM project
    BOMCode                 NVARCHAR(32) NULL,                  -- Reference to BOM code
    DeliveryDate            DATETIME NULL,
    Remarks                 NVARCHAR(255) NULL,
    CreatedDate             DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_PRDetail_PurchaseRequest 
        FOREIGN KEY (PRID) REFERENCES PurchaseRequest(PRID) ON DELETE CASCADE
);
GO

CREATE INDEX IX_PRDetail_PRID ON PurchaseRequestDetail(PRID);
CREATE INDEX IX_PRDetail_ItemCode ON PurchaseRequestDetail(ItemCode);
GO

-- ============================================================
-- 3. BOM TO PR CONVERSION TRACKING TABLE
-- Prevents duplicate PR creation from same BOM items
-- ============================================================
CREATE TABLE BOMtoPRConversion
(
    ConversionID            INT IDENTITY(1,1) PRIMARY KEY,
    BOMProjectCode          NVARCHAR(32) NOT NULL,
    ProductNo               NVARCHAR(32) NOT NULL,
    BOMCode                 NVARCHAR(32) NOT NULL,
    ItemCode                NVARCHAR(32) NOT NULL,
    PRID                    INT NOT NULL,
    PRNumber                NVARCHAR(32) NOT NULL,
    ConvertedQty            DECIMAL(18,3) DEFAULT 0,
    ConvertedDate           DATETIME DEFAULT GETDATE(),
    ConvertedBy             NVARCHAR(64) NULL,

    CONSTRAINT FK_BOMtoPR_PR 
        FOREIGN KEY (PRID) REFERENCES PurchaseRequest(PRID)
);
GO

CREATE INDEX IX_BOMtoPR_BOMProject ON BOMtoPRConversion(BOMProjectCode, ProductNo);
CREATE INDEX IX_BOMtoPR_ItemCode ON BOMtoPRConversion(ItemCode);
GO

-- ============================================================
-- 4. TRIGGER: Auto-calculate TotalCost in Detail table
-- ============================================================
CREATE TRIGGER trg_PRDetail_CalculateTotal
ON PurchaseRequestDetail
FOR INSERT, UPDATE
AS
BEGIN
    UPDATE PurchaseRequestDetail
    SET TotalCost = i.Quantity * i.UnitCost
    FROM PurchaseRequestDetail d
    INNER JOIN inserted i ON d.DetailID = i.DetailID;
END
GO

-- ============================================================
-- 5. TRIGGER: Auto-update TotalAmount in Header when detail changes
-- ============================================================
CREATE TRIGGER trg_PRDetail_UpdateHeaderTotal
ON PurchaseRequestDetail
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    UPDATE PurchaseRequest
    SET TotalAmount = (
        SELECT ISNULL(SUM(TotalCost), 0) 
        FROM PurchaseRequestDetail 
        WHERE PRID = p.PRID
    ),
    ModifiedDate = GETDATE()
    FROM PurchaseRequest p
    INNER JOIN (
        SELECT PRID FROM inserted
        UNION
        SELECT PRID FROM deleted
    ) t ON p.PRID = t.PRID;
END
GO

-- ============================================================
-- 6. STORED PROCEDURE: Get Next PR Number
-- Format: PR-YYYY-XXXXX (e.g., PR-2026-00001)
-- ============================================================
CREATE PROCEDURE sp_GetNextPRNumber
AS
BEGIN
    DECLARE @Year NVARCHAR(4) = CAST(YEAR(GETDATE()) AS NVARCHAR(4));
    DECLARE @Prefix NVARCHAR(8) = 'PR-' + @Year + '-';
    DECLARE @NextNum INT;

    SELECT @NextNum = ISNULL(MAX(CAST(SUBSTRING(PRNumber, LEN(@Prefix)+1, 10) AS INT)), 0) + 1
    FROM PurchaseRequest
    WHERE PRNumber LIKE @Prefix + '%';

    SELECT @Prefix + RIGHT('00000' + CAST(@NextNum AS NVARCHAR(5)), 5) AS NextPRNumber;
END
GO

-- ============================================================
-- 7. STORED PROCEDURE: Insert Purchase Request with Details
-- ============================================================
CREATE PROCEDURE sp_InsertPurchaseRequest
    @PRNumber NVARCHAR(32),
    @ProjectCode NVARCHAR(32),
    @ProductNo NVARCHAR(32) = NULL,
    @VendorCode NVARCHAR(32),
    @RequestedBy NVARCHAR(64) = NULL,
    @Remarks NVARCHAR(500) = NULL,
    @CreatedBy NVARCHAR(64) = NULL
AS
BEGIN
    INSERT INTO PurchaseRequest (PRNumber, ProjectCode, ProductNo, VendorCode, RequestedBy, Remarks, CreatedBy)
    VALUES (@PRNumber, @ProjectCode, @ProductNo, @VendorCode, @RequestedBy, @Remarks, @CreatedBy);

    SELECT SCOPE_IDENTITY() AS NewPRID;
END
GO

-- ============================================================
-- 8. STORED PROCEDURE: Insert PR Detail Line Item
-- ============================================================
CREATE PROCEDURE sp_InsertPRDetail
    @PRID INT,
    @ItemCode NVARCHAR(32),
    @ItemDescription NVARCHAR(255) = NULL,
    @Quantity DECIMAL(18,3),
    @UnitCost DECIMAL(18,2),
    @UOM NVARCHAR(15) = NULL,
    @BOMProjectCode NVARCHAR(32) = NULL,
    @BOMCode NVARCHAR(32) = NULL
AS
BEGIN
    INSERT INTO PurchaseRequestDetail (PRID, ItemCode, ItemDescription, Quantity, UnitCost, UOM, BOMProjectCode, BOMCode)
    VALUES (@PRID, @ItemCode, @ItemDescription, @Quantity, @UnitCost, @UOM, @BOMProjectCode, @BOMCode);
END
GO

-- ============================================================
-- 9. STORED PROCEDURE: Update PR Approval Status
-- ============================================================
CREATE PROCEDURE sp_UpdatePRApproval
    @PRID INT,
    @Status NVARCHAR(20),           -- Approved, Rejected, On Hold
    @ApprovedBy NVARCHAR(64),
    @Remarks NVARCHAR(500) = NULL,
    @RejectionReason NVARCHAR(500) = NULL,
    @HoldReason NVARCHAR(500) = NULL
AS
BEGIN
    UPDATE PurchaseRequest
    SET Status = @Status,
        ApprovedBy = @ApprovedBy,
        ApprovedDate = GETDATE(),
        Remarks = @Remarks,
        RejectionReason = @RejectionReason,
        HoldReason = @HoldReason,
        ModifiedDate = GETDATE()
    WHERE PRID = @PRID AND Status = 'Pending';

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- ============================================================
-- 10. STORED PROCEDURE: Get PR List with Filters
-- ============================================================
CREATE PROCEDURE sp_GetPRList
    @StatusFilter NVARCHAR(20) = 'All',
    @ProjectFilter NVARCHAR(32) = 'All',
    @VendorFilter NVARCHAR(32) = 'All',
    @SearchPR NVARCHAR(32) = NULL
AS
BEGIN
    SELECT 
        PR.PRID,
        PR.PRNumber,
        PR.ProjectCode,
        PR.VendorCode,
        ISNULL(V.VendorName, PR.VendorCode) AS VendorName,
        (SELECT COUNT(*) FROM PurchaseRequestDetail WHERE PRID = PR.PRID) AS ItemCount,
        PR.TotalAmount,
        PR.Status,
        PR.RequestedBy,
        PR.RequestDate,
        PR.ApprovedBy,
        PR.ApprovedDate,
        PR.RejectionReason,
        PR.HoldReason,
        PR.Remarks
    FROM PurchaseRequest PR
    LEFT JOIN VendorMaster V ON PR.VendorCode = V.VendorCode
    WHERE (@StatusFilter = 'All' OR PR.Status = @StatusFilter)
      AND (@ProjectFilter = 'All' OR PR.ProjectCode = @ProjectFilter)
      AND (@VendorFilter = 'All' OR V.VendorName = @VendorFilter OR PR.VendorCode = @VendorFilter)
      AND (@SearchPR IS NULL OR PR.PRNumber LIKE '%' + @SearchPR + '%')
    ORDER BY PR.RequestDate DESC;
END
GO

-- ============================================================
-- 11. STORED PROCEDURE: Get PR Statistics
-- ============================================================
CREATE PROCEDURE sp_GetPRStatistics
AS
BEGIN
    SELECT 
        SUM(CASE WHEN Status = 'Pending' THEN 1 ELSE 0 END) AS PendingCount,
        SUM(CASE WHEN Status = 'Approved' THEN 1 ELSE 0 END) AS ApprovedCount,
        SUM(CASE WHEN Status = 'Rejected' THEN 1 ELSE 0 END) AS RejectedCount,
        SUM(CASE WHEN Status = 'On Hold' THEN 1 ELSE 0 END) AS OnHoldCount,
        SUM(CASE WHEN Status = 'Pending' THEN TotalAmount ELSE 0 END) AS PendingAmount,
        SUM(TotalAmount) AS TotalAmount
    FROM PurchaseRequest;
END
GO

-- ============================================================
-- 12. STORED PROCEDURE: Check if BOM item already converted to PR
-- ============================================================
CREATE PROCEDURE sp_CheckBOMItemConverted
    @BOMProjectCode NVARCHAR(32),
    @ProductNo NVARCHAR(32),
    @ItemCode NVARCHAR(32)
AS
BEGIN
    SELECT 
        ISNULL(SUM(ConvertedQty), 0) AS AlreadyConvertedQty,
        COUNT(*) AS ConversionCount
    FROM BOMtoPRConversion
    WHERE BOMProjectCode = @BOMProjectCode
      AND ProductNo = @ProductNo
      AND ItemCode = @ItemCode;
END
GO

-- ============================================================
-- 13. STORED PROCEDURE: Insert BOM to PR Conversion Record
-- ============================================================
CREATE PROCEDURE sp_InsertBOMtoPRConversion
    @BOMProjectCode NVARCHAR(32),
    @ProductNo NVARCHAR(32),
    @BOMCode NVARCHAR(32),
    @ItemCode NVARCHAR(32),
    @PRID INT,
    @PRNumber NVARCHAR(32),
    @ConvertedQty DECIMAL(18,3),
    @ConvertedBy NVARCHAR(64) = NULL
AS
BEGIN
    INSERT INTO BOMtoPRConversion (BOMProjectCode, ProductNo, BOMCode, ItemCode, PRID, PRNumber, ConvertedQty, ConvertedBy)
    VALUES (@BOMProjectCode, @ProductNo, @BOMCode, @ItemCode, @PRID, @PRNumber, @ConvertedQty, @ConvertedBy);
END
GO

PRINT 'Purchase Request tables and stored procedures created successfully!';
PRINT '';
PRINT 'Tables created:';
PRINT '  1. PurchaseRequest (Header)';
PRINT '  2. PurchaseRequestDetail (Line Items)';
PRINT '  3. BOMtoPRConversion (Tracking)';
PRINT '';
PRINT 'Stored Procedures created:';
PRINT '  sp_GetNextPRNumber';
PRINT '  sp_InsertPurchaseRequest';
PRINT '  sp_InsertPRDetail';
PRINT '  sp_UpdatePRApproval';
PRINT '  sp_GetPRList';
PRINT '  sp_GetPRStatistics';
PRINT '  sp_CheckBOMItemConverted';
PRINT '  sp_InsertBOMtoPRConversion';
GO



--Purchase Request tables and stored procedures created successfully!
 
----Tables created:
 -- 1. PurchaseRequest (Header)
 -- 2. PurchaseRequestDetail (Line Items)
 -- 3. BOMtoPRConversion (Tracking)
 
--Stored Procedures created:
 -- sp_GetNextPRNumber
 -- sp_InsertPurchaseRequest
 -- sp_InsertPRDetail
 -- sp_UpdatePRApproval
 -- sp_GetPRList
 -- sp_GetPRStatistics
 -- sp_CheckBOMItemConverted
 -- sp_InsertBOMtoPRConversion

-- Completion time: 2026-07-20T10:23:11.2996629+05:30


