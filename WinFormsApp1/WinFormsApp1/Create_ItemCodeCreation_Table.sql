-- =====================================================
-- SQL Script: Create ItemCodeCreation Table
-- Database: ERP_Database
-- Purpose: Store item code creation requests before approval
-- Once approved, data moves to ItemMaster
-- =====================================================

USE [ERP_Database];
GO

-- Drop table if exists (for recreation)
IF OBJECT_ID('dbo.ItemCodeCreation', 'U') IS NOT NULL
    DROP TABLE dbo.ItemCodeCreation;
GO

-- =====================================================
-- Create ItemCodeCreation Table
-- =====================================================
CREATE TABLE dbo.ItemCodeCreation (
    -- Primary Key
    ID INT IDENTITY(1,1) NOT NULL,

    -- Request Identification
    RequestID NVARCHAR(32) NOT NULL,
    RequestDate DATETIME NOT NULL DEFAULT GETDATE(),

    -- Requestor Information
    Requestor NVARCHAR(128) NOT NULL,
    Department NVARCHAR(64) NOT NULL,

    -- Item Details (from ICCRF Step 1)
    ItemCategory NVARCHAR(64) NOT NULL,
    ItemDescription NVARCHAR(255) NOT NULL,
    TechnicalSpecification NVARCHAR(500) NULL,
    UnitOfMeasure NVARCHAR(15) NOT NULL,
    DrawingReference NVARCHAR(255) NULL,
    CriticalityLevel NVARCHAR(32) NULL,        -- ABC Classification
    HSNCode NVARCHAR(32) NULL,                  -- HSN/SAC Code

    -- Item Code Structure (from Step 2 & 5)
    ItemCode NVARCHAR(32) NOT NULL,             -- Generated Item Code (e.g., FA-XX-1001)
    Prefix NVARCHAR(8) NOT NULL,                -- Category Prefix (FA, MNT, CON, etc.)
    CategoryCode NVARCHAR(8) NOT NULL,          -- Sub-category code
    SequentialNumber NVARCHAR(8) NOT NULL,      -- Sequential number

    -- Approval Information (from Step 3)
    AuthorizedCreator NVARCHAR(128) NOT NULL,   -- Who created the item master
    ApprovalAuthority NVARCHAR(128) NOT NULL,   -- Who must approve (Material Manager / Operations Head)
    ApprovalStatus NVARCHAR(16) NOT NULL DEFAULT 'Pending',  -- Pending, Approved, Rejected, On Hold
    ApprovalRemarks NVARCHAR(500) NULL,         -- Remarks from approver
    ApprovedBy NVARCHAR(128) NULL,              -- Name of approver
    ApprovalDate DATETIME NULL,                 -- Date of approval/rejection

    -- Store Location (from Step 4 - filled upon GIN)
    StoreLocation NVARCHAR(255) NULL,
    BinNumber NVARCHAR(32) NULL,

    -- Emergency/Exception Handling
    IsEmergency BIT NOT NULL DEFAULT 0,         -- 1 = Emergency creation
    EmergencyApprovedBy NVARCHAR(128) NULL,     -- Emergency approver name

    -- Audit & Tracking
    CreatedBy NVARCHAR(128) NOT NULL DEFAULT SUSER_SNAME(),
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ModifiedBy NVARCHAR(128) NULL,
    ModifiedDate DATETIME NULL,

    -- Flags for workflow
    IsActive BIT NOT NULL DEFAULT 0,            -- 1 = Activated for transactions (after approval)
    IsPushedToItemMaster BIT NOT NULL DEFAULT 0, -- 1 = Data pushed to ItemMaster table
    PushDate DATETIME NULL,                      -- When data was pushed to ItemMaster

    -- Additional fields for future use
    Others NVARCHAR(255) NULL,

    -- Constraints
    CONSTRAINT PK_ItemCodeCreation PRIMARY KEY CLUSTERED (ID ASC),
    CONSTRAINT UQ_ItemCodeCreation_RequestID UNIQUE NONCLUSTERED (RequestID),
    CONSTRAINT UQ_ItemCodeCreation_ItemCode UNIQUE NONCLUSTERED (ItemCode),
    CONSTRAINT CHK_ItemCodeCreation_Status CHECK (ApprovalStatus IN ('Pending', 'Approved', 'Rejected', 'On Hold'))
);
GO

-- =====================================================
-- Create Indexes for Performance
-- =====================================================

-- Index on ApprovalStatus for filtering pending/approved items
CREATE NONCLUSTERED INDEX IX_ItemCodeCreation_ApprovalStatus 
ON dbo.ItemCodeCreation (ApprovalStatus);
GO

-- Index on ItemCategory for filtering by category
CREATE NONCLUSTERED INDEX IX_ItemCodeCreation_ItemCategory 
ON dbo.ItemCodeCreation (ItemCategory);
GO

-- Index on Department for filtering by department
CREATE NONCLUSTERED INDEX IX_ItemCodeCreation_Department 
ON dbo.ItemCodeCreation (Department);
GO

-- Index on RequestDate for date range queries
CREATE NONCLUSTERED INDEX IX_ItemCodeCreation_RequestDate 
ON dbo.ItemCodeCreation (RequestDate DESC);
GO

-- Index on IsPushedToItemMaster to track sync status
CREATE NONCLUSTERED INDEX IX_ItemCodeCreation_IsPushed 
ON dbo.ItemCodeCreation (IsPushedToItemMaster);
GO

-- =====================================================
-- Create Stored Procedures
-- =====================================================

-- 1. Insert new item code creation request
CREATE PROCEDURE [dbo].[sp_ItemCodeCreation_Insert]
    @RequestID NVARCHAR(32),
    @Requestor NVARCHAR(128),
    @Department NVARCHAR(64),
    @ItemCategory NVARCHAR(64),
    @ItemDescription NVARCHAR(255),
    @TechnicalSpecification NVARCHAR(500) = NULL,
    @UnitOfMeasure NVARCHAR(15),
    @DrawingReference NVARCHAR(255) = NULL,
    @CriticalityLevel NVARCHAR(32) = NULL,
    @HSNCode NVARCHAR(32) = NULL,
    @ItemCode NVARCHAR(32),
    @Prefix NVARCHAR(8),
    @CategoryCode NVARCHAR(8),
    @SequentialNumber NVARCHAR(8),
    @AuthorizedCreator NVARCHAR(128),
    @ApprovalAuthority NVARCHAR(128),
    @IsEmergency BIT = 0,
    @EmergencyApprovedBy NVARCHAR(128) = NULL,
    @CreatedBy NVARCHAR(128) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @CreatedBy IS NULL
        SET @CreatedBy = SUSER_SNAME();

    INSERT INTO dbo.ItemCodeCreation (
        RequestID, RequestDate, Requestor, Department,
        ItemCategory, ItemDescription, TechnicalSpecification, UnitOfMeasure,
        DrawingReference, CriticalityLevel, HSNCode,
        ItemCode, Prefix, CategoryCode, SequentialNumber,
        AuthorizedCreator, ApprovalAuthority,
        IsEmergency, EmergencyApprovedBy,
        CreatedBy, CreatedDate
    )
    VALUES (
        @RequestID, GETDATE(), @Requestor, @Department,
        @ItemCategory, @ItemDescription, @TechnicalSpecification, @UnitOfMeasure,
        @DrawingReference, @CriticalityLevel, @HSNCode,
        @ItemCode, @Prefix, @CategoryCode, @SequentialNumber,
        @AuthorizedCreator, @ApprovalAuthority,
        @IsEmergency, @EmergencyApprovedBy,
        @CreatedBy, GETDATE()
    );

    SELECT SCOPE_IDENTITY() AS NewID;
END
GO

-- 2. Update approval status
CREATE PROCEDURE [dbo].[sp_ItemCodeCreation_UpdateApproval]
    @RequestID NVARCHAR(32),
    @ApprovalStatus NVARCHAR(16),
    @ApprovalRemarks NVARCHAR(500) = NULL,
    @ApprovedBy NVARCHAR(128),
    @StoreLocation NVARCHAR(255) = NULL,
    @BinNumber NVARCHAR(32) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ItemCodeCreation
    SET 
        ApprovalStatus = @ApprovalStatus,
        ApprovalRemarks = @ApprovalRemarks,
        ApprovedBy = @ApprovedBy,
        ApprovalDate = GETDATE(),
        StoreLocation = ISNULL(@StoreLocation, StoreLocation),
        BinNumber = ISNULL(@BinNumber, BinNumber),
        IsActive = CASE WHEN @ApprovalStatus = 'Approved' THEN 1 ELSE 0 END,
        ModifiedBy = @ApprovedBy,
        ModifiedDate = GETDATE()
    WHERE RequestID = @RequestID;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- 3. Get pending requests for approval dashboard
CREATE PROCEDURE [dbo].[sp_ItemCodeCreation_GetPending]
    @ApprovalAuthority NVARCHAR(128) = NULL,
    @ItemCategory NVARCHAR(64) = NULL,
    @Department NVARCHAR(64) = NULL,
    @ApprovalStatus NVARCHAR(16) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ID,
        RequestID,
        RequestDate,
        Requestor,
        Department,
        ItemCategory,
        ItemDescription,
        TechnicalSpecification,
        UnitOfMeasure,
        DrawingReference,
        CriticalityLevel,
        HSNCode,
        ItemCode,
        Prefix,
        CategoryCode,
        SequentialNumber,
        AuthorizedCreator,
        ApprovalAuthority,
        ApprovalStatus,
        ApprovalRemarks,
        ApprovedBy,
        ApprovalDate,
        StoreLocation,
        BinNumber,
        IsEmergency,
        EmergencyApprovedBy,
        CreatedBy,
        CreatedDate,
        IsActive,
        IsPushedToItemMaster
    FROM dbo.ItemCodeCreation
    WHERE (@ApprovalAuthority IS NULL OR ApprovalAuthority = @ApprovalAuthority)
      AND (@ItemCategory IS NULL OR ItemCategory = @ItemCategory)
      AND (@Department IS NULL OR Department = @Department)
      AND (@ApprovalStatus IS NULL OR ApprovalStatus = @ApprovalStatus)
    ORDER BY RequestDate DESC;
END
GO

-- 4. Get single request by RequestID
CREATE PROCEDURE [dbo].[sp_ItemCodeCreation_GetByID]
    @RequestID NVARCHAR(32)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM dbo.ItemCodeCreation
    WHERE RequestID = @RequestID;
END
GO

-- 5. Push approved item to ItemMaster
CREATE PROCEDURE [dbo].[sp_ItemCodeCreation_PushToItemMaster]
    @RequestID NVARCHAR(32),
    @PushedBy NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @ItemCode NVARCHAR(32);
        DECLARE @ItemDescription NVARCHAR(255);
        DECLARE @UnitOfMeasure NVARCHAR(15);
        DECLARE @HSNCode NVARCHAR(32);
        DECLARE @StoreLocation NVARCHAR(255);
        DECLARE @DrawingReference NVARCHAR(255);
        DECLARE @CreatedBy NVARCHAR(128);

        -- Get the approved request details
        SELECT 
            @ItemCode = ItemCode,
            @ItemDescription = ItemDescription,
            @UnitOfMeasure = UnitOfMeasure,
            @HSNCode = HSNCode,
            @StoreLocation = StoreLocation,
            @DrawingReference = DrawingReference,
            @CreatedBy = CreatedBy
        FROM dbo.ItemCodeCreation
        WHERE RequestID = @RequestID AND ApprovalStatus = 'Approved' AND IsPushedToItemMaster = 0;

        IF @ItemCode IS NULL
        BEGIN
            RAISERROR('Request not found, not approved, or already pushed.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Insert into ItemMaster
        INSERT INTO dbo.ItemMaster (
            ItemCode,
            ItemDescription,
            CreatedDate,
            UnitCost,
            [Type],
            [Status],
            Remarks,
            Others,
            MinStockQty,
            [Issue],
            Receipt,
            OpeningQuantity,
            ClosingQuantity,
            StockValue,
            Units,
            PurchaseType,
            AvailableQty,
            ItemTypeCode,
            CreatedBy,
            [Location],
            HSNSACCode,
            DrawingNo,
            PreviousUnitCost,
            TypeofStorage,
            TargetCost,
            WARUpdatedDate,
            OpeningStock,
            OpeningRate,
            FixedCost
        )
        VALUES (
            @ItemCode,
            @ItemDescription,
            CONVERT(NVARCHAR(255), GETDATE(), 120),
            0.0,                                    -- UnitCost (to be updated later)
            'New',                                  -- Type
            'Active',                               -- Status
            'Created via ItemCodeCreation SOP',     -- Remarks
            NULL,                                   -- Others
            '0',                                    -- MinStockQty
            0,                                      -- Issue
            0,                                      -- Receipt
            0,                                      -- OpeningQuantity
            0,                                      -- ClosingQuantity
            0,                                      -- StockValue
            @UnitOfMeasure,                         -- Units
            'Regular',                              -- PurchaseType
            0,                                      -- AvailableQty
            LEFT(@ItemCode, CHARINDEX('-', @ItemCode) - 1), -- ItemTypeCode (extract prefix)
            @CreatedBy,                             -- CreatedBy
            @StoreLocation,                         -- Location
            @HSNCode,                               -- HSNSACCode
            @DrawingReference,                      -- DrawingNo
            0,                                      -- PreviousUnitCost
            'General',                              -- TypeofStorage
            0,                                      -- TargetCost
            CONVERT(VARCHAR(255), GETDATE(), 120),  -- WARUpdatedDate
            0,                                      -- OpeningStock
            0,                                      -- OpeningRate
            0.00                                    -- FixedCost
        );

        -- Mark as pushed
        UPDATE dbo.ItemCodeCreation
        SET 
            IsPushedToItemMaster = 1,
            PushDate = GETDATE(),
            ModifiedBy = @PushedBy,
            ModifiedDate = GETDATE()
        WHERE RequestID = @RequestID;

        COMMIT TRANSACTION;

        SELECT 'SUCCESS' AS Result, @ItemCode + ' pushed to ItemMaster successfully.' AS Message;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 'ERROR' AS Result, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO

-- 6. Get next sequential number for a prefix
CREATE PROCEDURE [dbo].[sp_ItemCodeCreation_GetNextSequence]
    @Prefix NVARCHAR(8)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaxSeq INT;

    SELECT @MaxSeq = MAX(CAST(SequentialNumber AS INT))
    FROM dbo.ItemCodeCreation
    WHERE Prefix = @Prefix;

    IF @MaxSeq IS NULL
        SET @MaxSeq = 1000;
    ELSE
        SET @MaxSeq = @MaxSeq + 1;

    SELECT @MaxSeq AS NextSequence;
END
GO

-- 7. Get dashboard statistics
CREATE PROCEDURE [dbo].[sp_ItemCodeCreation_GetStatistics]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        COUNT(CASE WHEN ApprovalStatus = 'Pending' THEN 1 END) AS TotalPending,
        COUNT(CASE WHEN ApprovalStatus = 'Approved' THEN 1 END) AS TotalApproved,
        COUNT(CASE WHEN ApprovalStatus = 'Rejected' THEN 1 END) AS TotalRejected,
        COUNT(CASE WHEN ApprovalStatus = 'On Hold' THEN 1 END) AS TotalOnHold,
        COUNT(CASE WHEN IsPushedToItemMaster = 1 THEN 1 END) AS TotalPushedToMaster,
        COUNT(*) AS TotalRequests
    FROM dbo.ItemCodeCreation;
END
GO

-- =====================================================
-- Sample Data Insert (Optional - for testing)
-- =====================================================
/*
EXEC [dbo].[sp_ItemCodeCreation_Insert]
    @RequestID = 'ICCRF-2026-001',
    @Requestor = 'Theertha Rao',
    @Department = 'Purchase',
    @ItemCategory = 'Capex / Fixed Asset',
    @ItemDescription = 'CNC MACHINE VMC 850',
    @TechnicalSpecification = 'Vertical Machining Center, 850x500x500mm',
    @UnitOfMeasure = 'Nos',
    @DrawingReference = 'DRW-CNC-001',
    @CriticalityLevel = 'A - High Critical',
    @HSNCode = '8457.10.00',
    @ItemCode = 'FA-XX-1001',
    @Prefix = 'FA',
    @CategoryCode = 'XX',
    @SequentialNumber = '1001',
    @AuthorizedCreator = 'Purchase Department',
    @ApprovalAuthority = 'Material Manager';
*/

PRINT 'ItemCodeCreation table and stored procedures created successfully!';
GO