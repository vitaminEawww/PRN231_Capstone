## **CÁC CORE FLOWS CỦA HỆ THỐNG**

### **1. FLOW ĐĂNG KÝ VÀ QUẢN LÝ THÀNH VIÊN**

- **Đăng ký tài khoản**: User → Customer → Profile Setup
- **Lựa chọn gói thành viên**: Customer → MembershipPackage → Payment
- **Quản lý membership**: MemberShipUsage → Renewal/Upgrade
- **Xác thực và phân quyền**: User roles & permissions

### **2. FLOW GHI NHẬN TÌNH TRẠNG HÚT THUỐC**

- **Khởi tạo hồ sơ hút thuốc**: Customer → SmokingRecord
  - Số lượng điếu/ngày (CigarettesPerDay)
  - Tần suất hút (SmokingFrequency)
  - Chi phí (CostPerPack)
  - Thương hiệu, triggers
- **Lịch sử hút thuốc**: SmokingStartDate, SmokingYears
- **Cập nhật định kỳ**: RecordDate tracking

### **3. FLOW LẬP KẾ HOẠCH CAI THUỐC**

- **Tạo kế hoạch**: Customer → QuitPlan
  - Lý do cai thuốc (Reasons)
  - Ngày bắt đầu (StartDate)
  - Mục tiêu (TargetDate)
- **Phân chia giai đoạn**: QuitPlan → QuitPlanPhase[]
- **Hỗ trợ tự động**: IsSystemGenerated plans
- **Tùy chỉnh cá nhân**: Manual adjustment

### **4. FLOW THEO DÕI TIẾN TRÌNH CAI THUỐC**

- **Ghi nhận hàng ngày**: Customer → DailyProgress
  - Số điếu hút (CigarettesSmoked)
  - Chi phí (MoneySpent)
  - Mức độ thèm muốn (CravingLevel)
  - Tâm trạng & năng lượng (MoodLevel, EnergyLevel)
- **Tính toán thống kê**:
  - Số ngày không hút (IsSmokeFree)
  - Tiền tiết kiệm được
  - Cải thiện sức khỏe
- **Thống kê tổng quan**: CustomerStatistics

### **5. FLOW HỆ THỐNG THÔNG BÁO & ĐỘNG VIÊN**

- **Thông báo định kỳ**: Notification system
  - Daily reminders (IsDailyReminderEnabled)
  - Weekly reports (IsWeeklyReportEnabled)
  - Motivational messages
- **Nhắc nhở lý do cai thuốc**: Based on QuitPlan.Reasons
- **Cài đặt tùy chỉnh**: Customer notification preferences

### **6. FLOW HUY HIỆU & THÀNH TÍCH**

- **Hệ thống huy hiệu**: Badge → UserBadge
  - Milestone achievements (1-day smoke free, money saved)
  - Criteria-based rewards
  - Points system
- **Bảng xếp hạng**: Leaderboard
- **Chia sẻ thành tích**: Post integration cho achievements

### **7. FLOW CỘNG ĐỒNG & CHIA SẺ**

- **Đăng bài chia sẻ**: Customer → Post
  - Kinh nghiệm cai thuốc
  - Thành tích đạt được
  - Động viên nhau
- **Tương tác**: PostComment, PostLike
- **Hỗ trợ lẫn nhau**: Community support

### **8. FLOW TƯ VẤN VỚI HUẤN LUYỆN VIÊN**

- **Đặt lịch tư vấn**: Customer → Coach → Consultation
- **Quản lý buổi tư vấn**:
  - Scheduling (ScheduledAt)
  - Duration & pricing
  - Status tracking
- **Trao đổi trực tuyến**: Message, Conversation
- **Đánh giá feedback**: Rating, Feedback

### **9. FLOW THANH TOÁN & MEMBERSHIP**

- **Xử lý thanh toán**: Payment processing
- **Quản lý gói dịch vụ**: MembershipPackage management
- **Theo dõi sử dụng**: MemberShipUsage tracking
- **Gia hạn/nâng cấp**: Package upgrade flows

### **10. FLOW QUẢN LÝ & BÁO CÁO**

- **Dashboard admin**: SystemReport
- **Thống kê hệ thống**: Usage analytics
- **Quản lý rating**: Rating, RatingSummary
- **Customer management**: Profile & activity tracking

### **11. FLOW COMMUNICATION**

- **Messaging system**: Message, Conversation
- **Notification delivery**: Push/email notifications
- **Multi-channel support**: Various communication channels

---
