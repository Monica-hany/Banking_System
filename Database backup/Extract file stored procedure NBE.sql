ALTER TABLE Transactions ADD IsExtracted BIT DEFAULT 0;

CREATE TABLE temp_table (
    DataLine NVARCHAR(MAX)
);

SELECT * FROM Transactions WHERE IsExtracted = 0;

WITH First22 AS (
    SELECT TOP 22 *
    FROM Transactions
    WHERE IsExtracted IS NULL
    ORDER BY Transaction_ID  -- replace with actual PK or order field if different
)

UPDATE First22
SET IsExtracted = 0;


SELECT * FROM sys.procedures WHERE name = 'ExportTransactionsToTextFile';


CREATE OR ALTER PROCEDURE ExportTransactionsToTextFile
AS
BEGIN
    SET NOCOUNT ON;

    -- Step 1: Truncate the temp table
    IF OBJECT_ID('dbo.temp_table') IS NOT NULL
        TRUNCATE TABLE dbo.temp_table;

    -- Step 2: Insert new transaction data as delimited strings
    INSERT INTO temp_table (DataLine)
    SELECT 
        CONCAT_WS(';',
            -- Sender Full Name
            ISNULL(SU.Fname, '') + ' ' + ISNULL(SU.Lname, ''),
            -- Receiver Full Name
            ISNULL(RU.Fname, '') + ' ' + ISNULL(RU.Lname, ''),
            -- Transaction Amount
            CAST(T.Amount AS NVARCHAR),
            -- Currency Name
            ISNULL(C.Currency_name, ''),
            -- Timestamp
            CONVERT(NVARCHAR, T.TimeStamp, 120),
            -- Sender Email
            ISNULL(SU.email, ''),
            -- Receiver Email
            ISNULL(RU.email, ''),
            -- Sender Account Type
            ISNULL(S_AT.AccountType_name, ''),
            -- Receiver Account Type
            ISNULL(R_AT.AccountType_name, '') --,


            -- Sender Balance in EGP
           -- CAST(SA.Balance * C.Currency_value AS NVARCHAR),
            -- Receiver Balance in EGP
          --  CAST(RA.Balance * C.Currency_value AS NVARCHAR)


        ) AS DataLine
    FROM Transactions T
    -- Sender side
    INNER JOIN Accounts SA ON T.Sender_Account_id = SA.Account_ID
    INNER JOIN Users SU ON SA.User_ID = SU.User_id
    INNER JOIN Look_AccountType S_AT ON SA.AccountType_id = S_AT.AccountType_id

    -- Receiver side
    INNER JOIN Accounts RA ON T.Receiver_Account_id = RA.Account_ID
    INNER JOIN Users RU ON RA.User_ID = RU.User_id
    INNER JOIN Look_AccountType R_AT ON RA.AccountType_id = R_AT.AccountType_id

    -- Currency (same for both sender and receiver assumed)
    INNER JOIN Look_Currency C ON SA.Currency_id = C.Currency_id

    WHERE T.IsExtracted = 0;

    -- Step 3: Mark transactions as extracted
   -- UPDATE Transactions
  --  SET IsExtracted = 1
   -- WHERE IsExtracted = 0;

   -- Return the contents of temp_table
    SELECT DataLine FROM temp_table;

END


CREATE TABLE ImportedTransactions (
    SenderName NVARCHAR(100),
    ReceiverName NVARCHAR(100),
    Amount NVARCHAR(50),
    Currency NVARCHAR(50),
    Timestamp NVARCHAR(50),
    SenderEmail NVARCHAR(100),
    ReceiverEmail NVARCHAR(100),
    SenderAccountType NVARCHAR(50),
    ReceiverAccountType NVARCHAR(50)
);


EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'xp_cmdshell', 1;
RECONFIGURE;




CREATE OR ALTER PROCEDURE ImportTransactionsFromFolder
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FilePath NVARCHAR(255) = 'C:\Users\DELL\Desktop\MiniBank\Extracted Transaction Files\';
    DECLARE @Command NVARCHAR(1000);
    DECLARE @FileName NVARCHAR(255);

    -- Temp table to hold filenames
    CREATE TABLE #FileList (Line NVARCHAR(4000));

    -- Get list of CSV files in the folder
    SET @Command = 'dir "' + @FilePath + '*.csv" /b';
    INSERT INTO #FileList
    EXEC xp_cmdshell @Command;

    DECLARE file_cursor CURSOR FOR
        SELECT Line FROM #FileList WHERE Line LIKE '%.csv';

    OPEN file_cursor;
    FETCH NEXT FROM file_cursor INTO @FileName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        DECLARE @FullFilePath NVARCHAR(500) = @FilePath + @FileName;

        -- Load the file into a temp table
        CREATE TABLE #TempImport (
            SenderName NVARCHAR(100),
            ReceiverName NVARCHAR(100),
            Amount NVARCHAR(50),
            Currency NVARCHAR(50),
            [Timestamp] NVARCHAR(50),
            SenderEmail NVARCHAR(100),
            ReceiverEmail NVARCHAR(100),
            SenderAccountType NVARCHAR(50),
            ReceiverAccountType NVARCHAR(50)
        );

        DECLARE @BulkInsertCommand NVARCHAR(1000) = '
        BULK INSERT #TempImport
        FROM ''' + @FullFilePath + '''
        WITH (
            FIELDTERMINATOR = ' + ''';''' + ',
            ROWTERMINATOR = ' + '''\n''' + ',
            FIRSTROW = 2,
            CODEPAGE = ''65001'',
            TABLOCK
        )';

        EXEC(@BulkInsertCommand);

        -- Insert only new (distinct) rows
        INSERT INTO ImportedTransactions
        SELECT * FROM #TempImport
        EXCEPT
        SELECT * FROM ImportedTransactions;

        DROP TABLE #TempImport;

        FETCH NEXT FROM file_cursor INTO @FileName;
    END

    CLOSE file_cursor;
    DEALLOCATE file_cursor;
    DROP TABLE #FileList;

    -- Return result
    SELECT * FROM ImportedTransactions;
END;


EXEC ImportTransactionsFromFolder;


ALTER TABLE Transactions
ADD 
    ValueDate DATETIME NOT NULL DEFAULT GETDATE(),
    NeedsJob BIT NOT NULL DEFAULT 0;
