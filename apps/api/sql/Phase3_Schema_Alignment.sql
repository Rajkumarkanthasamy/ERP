-- ============================================================
-- Optional schema for web ERP GRN / comments (run on ERP_Database)
-- Safe / idempotent — same intent as WinForms Phase3_Schema_Alignment.sql
-- ============================================================

IF OBJECT_ID('ProcurementComment', 'U') IS NULL
BEGIN
    CREATE TABLE ProcurementComment
    (
        CommentID     INT IDENTITY(1,1) PRIMARY KEY,
        EntityType    NVARCHAR(20) NOT NULL,
        EntityRef     NVARCHAR(64) NOT NULL,
        CommentText   NVARCHAR(2000) NOT NULL,
        CreatedBy     NVARCHAR(64) NULL,
        CreatedDate   DATETIME NOT NULL CONSTRAINT DF_PC_Created DEFAULT GETDATE()
    );
    CREATE INDEX IX_PC_Entity ON ProcurementComment(EntityType, EntityRef);
END
GO

IF OBJECT_ID('ProcurementAttachment', 'U') IS NULL
BEGIN
    CREATE TABLE ProcurementAttachment
    (
        AttachmentID  INT IDENTITY(1,1) PRIMARY KEY,
        EntityType    NVARCHAR(20) NOT NULL,
        EntityRef     NVARCHAR(64) NOT NULL,
        FileName      NVARCHAR(260) NOT NULL,
        FilePath      NVARCHAR(500) NOT NULL,
        FileSizeKB    DECIMAL(18,2) NULL,
        UploadedBy    NVARCHAR(64) NULL,
        UploadedDate  DATETIME NOT NULL CONSTRAINT DF_PA_Uploaded DEFAULT GETDATE(),
        Remarks       NVARCHAR(255) NULL
    );
    CREATE INDEX IX_PA_Entity ON ProcurementAttachment(EntityType, EntityRef);
END
GO

IF OBJECT_ID('ProcurementGRN', 'U') IS NULL
BEGIN
    CREATE TABLE ProcurementGRN
    (
        GRNID         INT IDENTITY(1,1) PRIMARY KEY,
        GRNNumber     NVARCHAR(40) NOT NULL UNIQUE,
        PORef         NVARCHAR(64) NULL,
        VendorCode    NVARCHAR(64) NULL,
        ProjectCode   NVARCHAR(64) NULL,
        InvoiceNo     NVARCHAR(100) NULL,
        ReceivedBy    NVARCHAR(64) NULL,
        ReceivedDate  DATETIME NOT NULL CONSTRAINT DF_GRN_Date DEFAULT GETDATE(),
        Status        NVARCHAR(30) NOT NULL CONSTRAINT DF_GRN_Status DEFAULT 'Received',
        Remarks       NVARCHAR(500) NULL,
        CreatedBy     NVARCHAR(64) NULL,
        CreatedDate   DATETIME NOT NULL CONSTRAINT DF_GRN_Created DEFAULT GETDATE()
    );
END
GO

IF COL_LENGTH('ProcurementGRN', 'InvoiceNo') IS NULL
BEGIN
    ALTER TABLE ProcurementGRN ADD InvoiceNo NVARCHAR(100) NULL;
END
GO

IF OBJECT_ID('ProcurementGRNDetail', 'U') IS NULL
BEGIN
    CREATE TABLE ProcurementGRNDetail
    (
        GRNDetailID   INT IDENTITY(1,1) PRIMARY KEY,
        GRNID         INT NOT NULL,
        GRNNumber     NVARCHAR(40) NOT NULL,
        POID          INT NULL,
        ItemCode      NVARCHAR(64) NOT NULL,
        OrderedQty    DECIMAL(18,3) DEFAULT 0,
        ReceivedQty   DECIMAL(18,3) DEFAULT 0,
        UnitPrice     DECIMAL(18,2) DEFAULT 0,
        Amount        DECIMAL(18,2) DEFAULT 0,
        UOM           NVARCHAR(32) NULL,
        Remarks       NVARCHAR(255) NULL,
        CONSTRAINT FK_GRNDetail_GRN FOREIGN KEY (GRNID) REFERENCES ProcurementGRN(GRNID)
    );
    CREATE INDEX IX_GRND_GRN ON ProcurementGRNDetail(GRNNumber);
END
GO

PRINT 'Phase 3 schema alignment completed.';
GO
