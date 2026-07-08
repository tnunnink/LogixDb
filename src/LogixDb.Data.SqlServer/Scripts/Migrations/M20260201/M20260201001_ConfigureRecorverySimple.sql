-- This script ensures the database doesn't grow the transaction log out of control 
-- during large Logix imports.
ALTER DATABASE CURRENT SET RECOVERY SIMPLE;