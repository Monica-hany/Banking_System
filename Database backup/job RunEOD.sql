SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE RUNEOD
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TransactionId INT,
            @SenderId INT,
            @ReceiverId INT,
            @Amount DECIMAL(18,2),
            @SenderCurrencyId INT,
            @ReceiverCurrencyId INT,
            @SenderBalance DECIMAL(18,2),
            @SenderRate DECIMAL(18,6),
            @ReceiverRate DECIMAL(18,6),
            @ConvertedAmount DECIMAL(18,2)

    DECLARE transaction_cursor CURSOR FOR
        SELECT Transaction_ID, Sender_Account_id, Receiver_Account_id, Amount
        FROM Transactions
        WHERE 
             NeedsJob = 1;
             AND CAST(ValueDate AS DATE) = CAST(GETDATE() AS DATE) -- comment To be able to test it 

    OPEN transaction_cursor;
    FETCH NEXT FROM transaction_cursor INTO @TransactionId, @SenderId, @ReceiverId, @Amount;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            -- Get sender balance and currency
            SELECT @SenderBalance = Balance, @SenderCurrencyId = Currency_id
            FROM Accounts
            WHERE Account_ID = @SenderId;

            -- Get receiver currency
            SELECT @ReceiverCurrencyId = Currency_id
            FROM Accounts
            WHERE Account_ID = @ReceiverId;

            -- Get currency rates from Look_Currency
            SELECT @SenderRate = Currency_value
            FROM Look_Currency
            WHERE Currency_id = @SenderCurrencyId;

            SELECT @ReceiverRate = Currency_value
            FROM Look_Currency
            WHERE Currency_id = @ReceiverCurrencyId;

            -- Currency conversion
            SET @ConvertedAmount = (@Amount * @SenderRate) / @ReceiverRate;

            -- Check if sender has enough balance
           -- IF @SenderBalance < @Amount
           -- BEGIN
             --   THROW 51000, 'Insufficient balance for scheduled transfer.', 1;
          --  END

            -- Deduct from sender
            UPDATE Accounts
            SET Balance = Balance - @Amount
            WHERE Account_ID = @SenderId;

            -- Add to receiver
            UPDATE Accounts
            SET Balance = Balance + @ConvertedAmount
            WHERE Account_ID = @ReceiverId;

            -- Mark transaction as processed
            UPDATE Transactions
            SET NeedsJob = 0
            WHERE Transaction_ID = @TransactionId;

            COMMIT TRANSACTION;
        END TRY

        BEGIN CATCH
            ROLLBACK TRANSACTION;
            DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
            PRINT 'Error: ' + @ErrorMessage;
        END CATCH;

        FETCH NEXT FROM transaction_cursor INTO @TransactionId, @SenderId, @ReceiverId, @Amount;
    END

    CLOSE transaction_cursor;
    DEALLOCATE transaction_cursor;
END
GO
