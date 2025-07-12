-- Create LOGS table
CREATE TABLE IF NOT EXISTS LOGS (
    log_id SERIAL PRIMARY KEY,
    date_logged DATE NOT NULL,
    level VARCHAR(15) NOT NULL,
    message VARCHAR(256) NOT NULL
);

-- Create Users table
CREATE TABLE IF NOT EXISTS Users (
    ID SERIAL PRIMARY KEY,
    ums_userid VARCHAR(450) NOT NULL UNIQUE,  -- Add UNIQUE constraint to avoid duplicate ums_userid
    EMAIL VARCHAR(256) NOT NULL,
    FirstName VARCHAR(256) NOT NULL,
    LastName VARCHAR(256) NOT NULL,
    DateLastLogin TIMESTAMP,
    DateLastLogout TIMESTAMP,
    DateCreated TIMESTAMP DEFAULT CURRENT_TIMESTAMP,  -- Default to current timestamp for DateCreated
    DateRetired TIMESTAMP
);

-- Create Transactions table
CREATE TABLE IF NOT EXISTS Transactions (
    TransactionsId SERIAL PRIMARY KEY,
    UserId INT REFERENCES Users(ID) ON DELETE CASCADE,  -- Ensure UserId is INT and use ON DELETE CASCADE for cleanup
    Details VARCHAR(15),
    PostingDate DATE,
    Description VARCHAR(256),
    Amount DECIMAL(10, 2),  -- Specify precision for Amount
    TYPE VARCHAR(64),
    Balance DECIMAL(10, 2),  -- Specify precision for Balance
    CheckOrSlip INT
);


-- Create PlaidItems table
CREATE TABLE IF NOT EXISTS PlaidItems (
    PlaidItemId SERIAL PRIMARY KEY,
    UserId INT REFERENCES Users(ID) ON DELETE CASCADE,
    AccessToken TEXT NOT NULL,
    InstitutionName TEXT,
    DateLinked TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS Category
(
    CategoryId SERIAL PRIMARY KEY,
    Name TEXT
);

CREATE TABLE IF NOT EXISTS BankInfo
(
    BankInfoId SERIAL PRIMARY KEY,
    UserId INT REFERENCES Users(ID) ON DELETE CASCADE,
    BankName TEXT NOT NULL,
    TotalBankBalance DECIMAL(10, 2) NOT NULL,
);


CREATE TABLE IF NOT EXISTS StreamlinedTransactions (
    StreamLinedTransactionsId SERIAL PRIMARY KEY,
    UserId INT REFERENCES Users(ID) ON DELETE CASCADE,
    TransactionId TEXT,
    Name TEXT,
    CategoryId INT,
    Note TEXT,
    Amount DECIMAL,
    Date TIMESTAMPTZ,
    EnvironmentType TEXT
);


CREATE TABLE IF NOT EXISTS CategoryTransaction_Junc
(
    CategoryId INT,
    StreamLinedTransactionsId INT,
    PRIMARY KEY (CategoryId, StreamLinedTransactionsId),
    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId),
    FOREIGN KEY (StreamLinedTransactionsId) REFERENCES StreamlinedTransactions(StreamLinedTransactionsId)
);


CREATE TABLE IF NOT EXISTS BudgetCaps
(
    CategoryBudget INT,
    CategoryId INT,
    BankInfoId INT,
    UserId INT,
    PRIMARY KEY (CategoryId, CategoryBudget, BankInfoId, UserId),
    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId),
    FOREIGN KEY (BankInfoId) REFERENCES BankInfo(BankInfo),
    FOREIGN KEY (UserId) REFERENCES Users(ID)
);

CREATE TABLE IF NOT EXISTS BudgetCaps
(
    CategoryBudget INT,
    CategoryId INT,
    BankInfoId INT,
    UserId INT,
    PRIMARY KEY (CategoryId, CategoryBudget, BankInfoId, UserId),
    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId),
    FOREIGN KEY (BankInfoId) REFERENCES BankInfo(BankInfoId),
    FOREIGN KEY (UserId) REFERENCES Users(ID)
);

-- UPDATE category
-- SET category_budget = CASE
--     WHEN name ILIKE '%rent/mortgage%' THEN 1500
--     WHEN name ILIKE '%Utilities%' THEN 500
--     WHEN name ILIKE '%Groceries%' THEN 500
--     WHEN name ILIKE '%Transportation%' THEN 100
--     WHEN name ILIKE '%Insurance%' THEN 50
--     WHEN name ILIKE '%Healthcare%' THEN 50
--     WHEN name ILIKE '%phone/internet%' THEN 100
--     WHEN name ILIKE '%entertainment%' THEN 300
--     WHEN name ILIKE '%income%' THEN 3000
--     ELSE 1000
-- END;
