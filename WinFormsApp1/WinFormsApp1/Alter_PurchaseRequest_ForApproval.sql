-- Add approval columns to PurchaseRequest table (if not already created)
-- Run this if your PurchaseRequest table was created earlier without approval fields

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PurchaseRequest') AND name = 'Status')
BEGIN
    ALTER TABLE PurchaseRequest ADD Status NVARCHAR(20) DEFAULT 'Pending';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PurchaseRequest') AND name = 'RequestedBy')
BEGIN
    ALTER TABLE PurchaseRequest ADD RequestedBy NVARCHAR(64);
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PurchaseRequest') AND name = 'RequestDate')
BEGIN
    ALTER TABLE PurchaseRequest ADD RequestDate DATETIME DEFAULT GETDATE();
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PurchaseRequest') AND name = 'ApprovedBy')
BEGIN
    ALTER TABLE PurchaseRequest ADD ApprovedBy NVARCHAR(64);
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PurchaseRequest') AND name = 'ApprovedDate')
BEGIN
    ALTER TABLE PurchaseRequest ADD ApprovedDate DATETIME;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PurchaseRequest') AND name = 'RejectionReason')
BEGIN
    ALTER TABLE PurchaseRequest ADD RejectionReason NVARCHAR(500);
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PurchaseRequest') AND name = 'HoldReason')
BEGIN
    ALTER TABLE PurchaseRequest ADD HoldReason NVARCHAR(500);
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PurchaseRequest') AND name = 'Remarks')
BEGIN
    ALTER TABLE PurchaseRequest ADD Remarks NVARCHAR(500);
END

-- Update existing records to have default status
UPDATE PurchaseRequest SET Status = 'Pending' WHERE Status IS NULL;

PRINT 'PurchaseRequest table updated with approval columns.';