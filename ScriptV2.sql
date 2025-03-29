/********************************************
 * Drop Database if It Exists
 ********************************************/
USE [master];
GO
IF DB_ID(N'SWD_GrowthTrackingSystemDb') IS NOT NULL
BEGIN
    PRINT 'Dropping existing database SWD_GrowthTrackingSystemDb...';
    ALTER DATABASE [SWD_GrowthTrackingSystemDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [SWD_GrowthTrackingSystemDb];
END
GO

/********************************************
 * Create Database
 ********************************************/
CREATE DATABASE [SWD_GrowthTrackingSystemDb];
GO

USE [SWD_GrowthTrackingSystemDb];
GO

/********************************************
 * Create Tables with Defaults and Primary Keys
 ********************************************/

-- UserAccount Table
CREATE TABLE [dbo].[UserAccount](
    [UserAccountID] [uniqueidentifier] NOT NULL,
    [FullName] [nvarchar](100) NOT NULL,
    [Email] [nvarchar](100) NOT NULL,
    [PasswordHash] [nvarchar](255) NULL,
    [PhoneNumber] [nvarchar](15) NULL,
    [Role] [nvarchar](20) NOT NULL,
    [CreatedAt] [datetime] NULL,
    [UpdatedAt] [datetime] NULL,
    [IsActive] [bit] NULL,
    [ProfilePictureUrl] [varchar](255) NULL,
    [Address] [text] NULL,
    [Bio] [text] NULL,
    [EmailVerified] [bit] NULL,
    [PhoneVerified] [bit] NULL, -- Thêm cột PhoneVerified
    [VerificationToken] [varchar](255) NULL,
    [ResetToken] [varchar](255) NULL,
    [ResetTokenExpiry] [datetime] NULL,
    [OAuth2GoogleId] [varchar](255) NULL,
    [OAuth2FacebookId] [varchar](255) NULL,
    [LastLoginAt] [datetime] NULL,
    CONSTRAINT PK_UserAccount PRIMARY KEY CLUSTERED 
    (
        [UserAccountID] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    CONSTRAINT UQ_UserAccount_Email UNIQUE NONCLUSTERED 
    (
        [Email] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO

-- Thiết lập giá trị mặc định cho UserAccount
ALTER TABLE [dbo].[UserAccount] ADD CONSTRAINT DF_UserAccount_UserAccountID DEFAULT (NEWID()) FOR [UserAccountID];
GO
ALTER TABLE [dbo].[UserAccount] ADD CONSTRAINT DF_UserAccount_CreatedAt DEFAULT (GETDATE()) FOR [CreatedAt];
GO
ALTER TABLE [dbo].[UserAccount] ADD CONSTRAINT DF_UserAccount_UpdatedAt DEFAULT (GETDATE()) FOR [UpdatedAt];
GO
ALTER TABLE [dbo].[UserAccount] ADD CONSTRAINT DF_UserAccount_IsActive DEFAULT ((1)) FOR [IsActive];
GO
ALTER TABLE [dbo].[UserAccount] ADD CONSTRAINT DF_UserAccount_EmailVerified DEFAULT ((0)) FOR [EmailVerified];
GO
ALTER TABLE [dbo].[UserAccount] ADD CONSTRAINT DF_UserAccount_PhoneVerified DEFAULT ((0)) FOR [PhoneVerified];
GO

-- BugReport Table
CREATE TABLE dbo.BugReport (
    bug_report_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_BugReport_bug_report_id DEFAULT (NEWID()),
    user_id UNIQUEIDENTIFIER NOT NULL,
    report_type VARCHAR(50) NULL,
    description TEXT NOT NULL,
    screenshot_url VARCHAR(255) NULL,
    status VARCHAR(20) NOT NULL CONSTRAINT DF_BugReport_status DEFAULT ('open'),
    created_at DATETIME NULL CONSTRAINT DF_BugReport_created_at DEFAULT (GETDATE()),
    updated_at DATETIME NULL CONSTRAINT DF_BugReport_updated_at DEFAULT (GETDATE()),
    CONSTRAINT PK_BugReport PRIMARY KEY CLUSTERED (bug_report_id)
);
GO

-- Notification Table
CREATE TABLE dbo.Notification (
    notification_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Notification_notification_id DEFAULT (NEWID()),
    user_id UNIQUEIDENTIFIER NOT NULL, 
    notification_type VARCHAR(20) NOT NULL, -- Loại thông báo: 'email', 'sms', 'app'
    content TEXT NOT NULL, -- Nội dung thông báo
    status VARCHAR(20) NOT NULL CONSTRAINT DF_Notification_status DEFAULT ('pending'), -- Trạng thái: 'pending', 'sent', 'failed'
    sent_at DATETIME NULL, -- Thời điểm gửi thông báo (NULL nếu chưa gửi)
    created_at DATETIME NULL CONSTRAINT DF_Notification_created_at DEFAULT (GETDATE()), -- Thời điểm tạo thông báo
    updated_at DATETIME NULL CONSTRAINT DF_Notification_updated_at DEFAULT (GETDATE()), -- Thời điểm cập nhật cuối cùng
    CONSTRAINT PK_Notification PRIMARY KEY CLUSTERED (notification_id)
);
GO

-- Booking Table
CREATE TABLE dbo.Booking (
    booking_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Booking_booking_id DEFAULT (NEWID()),
    parent_id UNIQUEIDENTIFIER NOT NULL,
    child_id UNIQUEIDENTIFIER NOT NULL,
    schedule_id UNIQUEIDENTIFIER NOT NULL,
    status VARCHAR(20) NOT NULL CONSTRAINT DF_Booking_status DEFAULT ('pending'), -- pending, confirmed, booked, cancelled
    booking_date DATETIME NULL CONSTRAINT DF_Booking_booking_date DEFAULT (GETDATE()),
    doctor_confirmation_deadline DATETIME NULL,
    payment_deadline DATETIME NULL, -- Thêm cột để lưu thời hạn thanh toán (48 giờ sau khi bác sĩ xác nhận)
    notes TEXT NULL,
    created_at DATETIME NULL CONSTRAINT DF_Booking_created_at DEFAULT (GETDATE()),
    updated_at DATETIME NULL CONSTRAINT DF_Booking_updated_at DEFAULT (GETDATE()),
    StatusDelete BIT NOT NULL CONSTRAINT DF_Booking_StatusDelete DEFAULT (0),
    CancelledBy VARCHAR(20) NULL, -- Thêm cột để lưu ai hủy (Parent hoặc Doctor)
    CancellationTime DATETIME NULL, -- Thêm cột để lưu thời gian hủy
    CONSTRAINT PK_Booking PRIMARY KEY CLUSTERED (booking_id)
);
GO

-- Child Table
CREATE TABLE dbo.Child (
    child_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Child_child_id DEFAULT (NEWID()),
    parent_id UNIQUEIDENTIFIER NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    date_of_birth DATE NOT NULL,
    gender VARCHAR(10) NULL,
    birth_weight DECIMAL(5,2) NULL,
    birth_height DECIMAL(5,2) NULL,
    avatar_url VARCHAR(255) NULL,
    created_at DATETIME NULL CONSTRAINT DF_Child_created_at DEFAULT (GETDATE()),
    updated_at DATETIME NULL CONSTRAINT DF_Child_updated_at DEFAULT (GETDATE()),
    CONSTRAINT PK_Child PRIMARY KEY CLUSTERED (child_id)
);
GO

-- Consultation Table
CREATE TABLE dbo.Consultation (
    consultation_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Consultation_consultation_id DEFAULT (NEWID()),
    booking_id UNIQUEIDENTIFIER NOT NULL,
    doctor_id UNIQUEIDENTIFIER NOT NULL,
    consultation_date DATETIME NOT NULL,
    consultation_notes TEXT NULL,
    status VARCHAR(20) NOT NULL CONSTRAINT DF_Consultation_status DEFAULT ('scheduled'),
    created_at DATETIME NULL CONSTRAINT DF_Consultation_created_at DEFAULT (GETDATE()),
    updated_at DATETIME NULL CONSTRAINT DF_Consultation_updated_at DEFAULT (GETDATE()),
    StatusDelete BIT NOT NULL CONSTRAINT DF_Consultation_StatusDelete DEFAULT (0),
    CONSTRAINT PK_Consultation PRIMARY KEY CLUSTERED (consultation_id)
);
GO

-- Certificate Table
CREATE TABLE dbo.Certificate (
    certificate_id UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_Certificate_certificate_id DEFAULT (NEWID()),
    doctor_id UNIQUEIDENTIFIER NOT NULL,
    certificate_type VARCHAR(50) NOT NULL,
    certificate_number VARCHAR(100) NULL,
    issuing_authority VARCHAR(100) NULL,
    issue_date DATE NULL,
    expiry_date DATE NULL,
    document_url VARCHAR(255) NULL,
    created_at DATETIME NULL 
        CONSTRAINT DF_Certificate_created_at DEFAULT (GETDATE()),
    updated_at DATETIME NULL 
        CONSTRAINT DF_Certificate_updated_at DEFAULT (GETDATE()),
    CONSTRAINT PK_Certificate PRIMARY KEY CLUSTERED (certificate_id)
);
GO

-- Doctor Table
CREATE TABLE dbo.Doctor (
    doctor_id UNIQUEIDENTIFIER NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    date_of_birth DATE NULL,
    gender VARCHAR(10) NULL,
    address VARCHAR(255) NULL,
    phone_number VARCHAR(20) NULL,
    specialization VARCHAR(100) NULL,
    experience_years INT NULL,
    workplace VARCHAR(255) NULL,
    biography TEXT NULL,
    profile_photo VARCHAR(255) NULL,
    average_rating DECIMAL(3,2) NULL CONSTRAINT DF_Doctor_average_rating DEFAULT ((0.00)),
    created_at DATETIME NULL CONSTRAINT DF_Doctor_created_at DEFAULT (GETDATE()),
    updated_at DATETIME NULL CONSTRAINT DF_Doctor_updated_at DEFAULT (GETDATE()),
    CONSTRAINT PK_Doctor PRIMARY KEY CLUSTERED (doctor_id)
);
GO

-- Feedback Table
CREATE TABLE dbo.Feedback (
    feedback_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Feedback_feedback_id DEFAULT (NEWID()),
    consultation_id UNIQUEIDENTIFIER NOT NULL,
    parent_id UNIQUEIDENTIFIER NOT NULL,
    rating INT NOT NULL,
    comment TEXT NULL,
    created_at DATETIME NULL CONSTRAINT DF_Feedback_created_at DEFAULT (GETDATE()),
    updated_at DATETIME NULL CONSTRAINT DF_Feedback_updated_at DEFAULT (GETDATE()),
    CONSTRAINT PK_Feedback PRIMARY KEY CLUSTERED (feedback_id)
);
GO

-- GrowthRecord Table
CREATE TABLE dbo.GrowthRecord (
    growth_record_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_GrowthRecord_growth_record_id DEFAULT (NEWID()),
    child_id UNIQUEIDENTIFIER NOT NULL,
    recorded_at DATETIME NOT NULL,
    weight DECIMAL(5,2) NULL,
    height DECIMAL(5,2) NULL,
    head_circumference DECIMAL(5,2) NULL,
    bmi DECIMAL(5,2) NULL,
    notes TEXT NULL,
    created_at DATETIME NULL CONSTRAINT DF_GrowthRecord_created_at DEFAULT (GETDATE()),
    updated_at DATETIME NULL CONSTRAINT DF_GrowthRecord_updated_at DEFAULT (GETDATE()),
    CONSTRAINT PK_GrowthRecord PRIMARY KEY CLUSTERED (growth_record_id)
);
GO

-- Milestone Table
CREATE TABLE dbo.Milestone (
    milestone_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Milestone_milestone_id DEFAULT (NEWID()),
    child_id UNIQUEIDENTIFIER NOT NULL,
    milestone_type VARCHAR(50) NOT NULL,
    milestone_date DATE NOT NULL,
    description TEXT NULL,
    created_at DATETIME NULL CONSTRAINT DF_Milestone_created_at DEFAULT (GETDATE()),
    updated_at DATETIME NULL CONSTRAINT DF_Milestone_updated_at DEFAULT (GETDATE()),
    CONSTRAINT PK_Milestone PRIMARY KEY CLUSTERED (milestone_id)
);
GO

-- Parent Table
CREATE TABLE dbo.Parent (
    parent_id UNIQUEIDENTIFIER NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    date_of_birth DATE NULL,
    gender VARCHAR(10) NULL,
    address VARCHAR(255) NULL,
    avatar_url VARCHAR(255) NULL,
    created_at DATETIME NULL CONSTRAINT DF_Parent_created_at DEFAULT (GETDATE()),
    updated_at DATETIME NULL CONSTRAINT DF_Parent_updated_at DEFAULT (GETDATE()),
    CONSTRAINT PK_Parent PRIMARY KEY CLUSTERED (parent_id)
);
GO

-- Schedule Table
CREATE TABLE dbo.Schedule (
    schedule_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Schedule_schedule_id DEFAULT (NEWID()),
    doctor_id UNIQUEIDENTIFIER NOT NULL,
    start_time DATETIME NOT NULL,
    end_time DATETIME NOT NULL,
    location VARCHAR(255) NULL,
    is_available BIT NULL CONSTRAINT DF_Schedule_is_available DEFAULT ((1)),
    created_at DATETIME NULL CONSTRAINT DF_Schedule_created_at DEFAULT (GETDATE()),
    updated_at DATETIME NULL CONSTRAINT DF_Schedule_updated_at DEFAULT (GETDATE()),
    StatusDelete BIT NOT NULL CONSTRAINT DF_Schedule_StatusDelete DEFAULT (0),
    CONSTRAINT PK_Schedule PRIMARY KEY CLUSTERED (schedule_id)
);
GO

-- Transaction Table
CREATE TABLE dbo.[Transaction] (
    transaction_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Transaction_transaction_id DEFAULT (NEWID()),
    parent_id UNIQUEIDENTIFIER NOT NULL,
    booking_id UNIQUEIDENTIFIER NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    payment_method VARCHAR(20) NOT NULL,
    status VARCHAR(20) NOT NULL CONSTRAINT DF_Transaction_status DEFAULT ('pending'),
    transaction_date DATETIME NULL CONSTRAINT DF_Transaction_transaction_date DEFAULT (GETDATE()),
    reference_code VARCHAR(100) NULL,
    created_at DATETIME NULL CONSTRAINT DF_Transaction_created_at DEFAULT (GETDATE()),
    updated_at DATETIME NULL CONSTRAINT DF_Transaction_updated_at DEFAULT (GETDATE()),
    CONSTRAINT PK_Transaction PRIMARY KEY CLUSTERED (transaction_id)
);
GO

/********************************************
 * Create Foreign Key Constraints
 ********************************************/

ALTER TABLE dbo.BugReport 
    ADD CONSTRAINT FK_BugReport_User FOREIGN KEY (user_id)
    REFERENCES dbo.UserAccount(UserAccountID);
GO

ALTER TABLE dbo.Notification 
    ADD CONSTRAINT FK_Notification_User FOREIGN KEY (user_id)
    REFERENCES dbo.UserAccount(UserAccountID);
GO

ALTER TABLE dbo.Booking 
    ADD CONSTRAINT FK_Booking_Child FOREIGN KEY (child_id)
    REFERENCES dbo.Child(child_id);
GO

ALTER TABLE dbo.Booking 
    ADD CONSTRAINT FK_Booking_Parent FOREIGN KEY (parent_id)
    REFERENCES dbo.Parent(parent_id);
GO

ALTER TABLE dbo.Booking 
    ADD CONSTRAINT FK_Booking_Schedule FOREIGN KEY (schedule_id)
    REFERENCES dbo.Schedule(schedule_id);
GO

ALTER TABLE dbo.Child 
    ADD CONSTRAINT FK_Child_Parent FOREIGN KEY (parent_id)
    REFERENCES dbo.Parent(parent_id);
GO

ALTER TABLE dbo.Consultation 
    ADD CONSTRAINT FK_Consultation_Booking FOREIGN KEY (booking_id)
    REFERENCES dbo.Booking(booking_id);
GO

ALTER TABLE dbo.Consultation 
    ADD CONSTRAINT FK_Consultation_Doctor FOREIGN KEY (doctor_id)
    REFERENCES dbo.Doctor(doctor_id);
GO

ALTER TABLE dbo.Certificate 
    ADD CONSTRAINT FK_Certificate_Doctor FOREIGN KEY (doctor_id)
    REFERENCES dbo.Doctor(doctor_id);
GO

ALTER TABLE dbo.Doctor 
    ADD CONSTRAINT FK_Doctor_User FOREIGN KEY (doctor_id)
    REFERENCES dbo.UserAccount(UserAccountID);
GO

ALTER TABLE dbo.Feedback 
    ADD CONSTRAINT FK_Feedback_Consultation FOREIGN KEY (consultation_id)
    REFERENCES dbo.Consultation(consultation_id);
GO

ALTER TABLE dbo.Feedback 
    ADD CONSTRAINT FK_Feedback_Parent FOREIGN KEY (parent_id)
    REFERENCES dbo.Parent(parent_id);
GO

ALTER TABLE dbo.GrowthRecord 
    ADD CONSTRAINT FK_GrowthRecord_Child FOREIGN KEY (child_id)
    REFERENCES dbo.Child(child_id);
GO

ALTER TABLE dbo.Milestone 
    ADD CONSTRAINT FK_Milestone_Child FOREIGN KEY (child_id)
    REFERENCES dbo.Child(child_id);
GO

ALTER TABLE dbo.Parent 
    ADD CONSTRAINT FK_Parent_User FOREIGN KEY (parent_id)
    REFERENCES dbo.UserAccount(UserAccountID);
GO

ALTER TABLE dbo.Schedule 
    ADD CONSTRAINT FK_Schedule_Doctor FOREIGN KEY (doctor_id)
    REFERENCES dbo.Doctor(doctor_id);
GO

ALTER TABLE dbo.[Transaction] 
    ADD CONSTRAINT FK_Transaction_Booking FOREIGN KEY (booking_id)
    REFERENCES dbo.Booking(booking_id);
GO

ALTER TABLE dbo.[Transaction] 
    ADD CONSTRAINT FK_Transaction_Parent FOREIGN KEY (parent_id)
    REFERENCES dbo.Parent(parent_id);
GO