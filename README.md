# EV Rental Platform

## Giới thiệu
EV Rental Platform là một nền tảng cho thuê xe điện (EV) được thiết kế để hỗ trợ người thuê (EV Renter), nhân viên tại điểm thuê (Station Staff), và quản trị viên (Admin). Dự án nhằm cung cấp giải pháp thuê xe hiệu quả, bền vững, với các tính năng chính như đăng ký/xác thực, đặt xe qua bản đồ, nhận/trả xe, quản lý đội xe, và phân tích dữ liệu.

- **Mục tiêu**: Tạo ra một hệ thống dễ mở rộng, thân thiện với người dùng, và tích hợp các công nghệ hiện đại.
- **Bối cảnh**: Dự án khởi đầu với team nhỏ (4-5 người), hướng tới scale lên multi-station.

## Tính năng chính
- **EV Renter**: Đăng ký/xác thực, đặt xe, nhận/trả xe, xem lịch sử/thống kê.
- **Station Staff**: Quản lý giao/nhận xe, xác thực khách, thanh toán, cập nhật trạng thái xe.
- **Admin**: Quản lý đội xe/điểm thuê, khách hàng, nhân viên, báo cáo/AI phân tích.

## Kiến trúc và Công nghệ
- **Kiến trúc**: Modular Monolith (dễ mở rộng sang Microservices).
- **Tech Stack**:
  - **Backend**: ASP.NET 8 (Minimal APIs + MediatR) với CQRS.
  - **Frontend**: ReactJS (hooks) cho UI động.
  - **Database**: PostgreSQL (PostGIS), Redis, MongoDB.
  - **Messaging**: RabbitMQ.
  - **Cloud**: Azure (App Service + AKS) với Docker.
- **Xu hướng**: Cloud-based, AI-driven insights, mobile-first, sustainable EV features.

## Thành viên
| Name                | Role                | Task                                 |
|---------------------|---------------------|--------------------------------------|
| Nguyễn Minh Nhật    | Backend Developer   | Quản lý dự án, phát triển backend    |
| Nguyễn Quốc Việt    | FullStack Developer | Leader, Thiết kế và phát triển UI    |
| Nguyễn Minh Thiện   | Backend Developer   | Xây dựng API và logic nghiệp vụ      |
| Nguyễn Tấn Phát     | Frontend Developer  | Leader, quản lý dự án frontend       |

## Cài đặt
1. **Yêu cầu**:
   - .NET 8 SDK
   - PostgreSQL
2. **Bước cài đặt**:
   - Clone repository: `git clone <repository-url>`
   - Chuyển đến thư mục: `cd EVRentalPlatform`
   - Khôi phục dependencies:
     - Backend: `dotnet restore EVRentalPlatform.APIService.sln`
   - Cấu hình chuỗi kết nối trong `appsettings.json` (EVRentalPlatform.APIService).
   - Chạy ứng dụng:
     - Backend: `dotnet run --project EVRentalPlatform.APIService`

## Sử dụng
- **EV Renter**: Truy cập app/web, đăng ký, đặt xe qua bản đồ, nhận/trả xe tại điểm.
- **Station Staff**: Đăng nhập, quản lý giao/nhận xe, cập nhật trạng thái.
- **Admin**: Truy cập dashboard, phân tích báo cáo, điều phối tài nguyên.
- **API**: Sử dụng Swagger tại `/swagger` sau khi chạy backend.

## Main Flow
- **Quy Trình Thuê Xe Hoàn Chỉnh (EV Renter)**:
  1. Đăng ký và xác thực.
  2. Đặt xe qua bản đồ.
  3. Nhận xe tại điểm.
  4. Trả xe và thanh toán.
  - **Ngoại lệ**: Xe hết, giấy tờ không hợp lệ.
- **Quy Trình Quản Lý Giao/Nhận Xe (Station Staff)**:
  1. Kiểm tra và chuẩn bị xe.
  2. Giao xe cho khách.
  3. Nhận xe và xử lý thanh toán.
  - **Ngoại lệ**: Giấy tờ không khớp.
- **Quy Trình Phân Tích và Điều Phối (Admin)**:
  1. Thu thập dữ liệu báo cáo.
  2. Phân tích và dự báo nhu cầu.
  3. Điều phối tài nguyên.
  - **Ngoại lệ**: Dữ liệu thiếu.

## Ghi chú
- **Tính năng mở rộng**: Tích hợp telematics, multi-tenant khi scale.
- **Rủi ro**: Vendor lock-in (Azure), bảo mật dữ liệu → Mitigate bằng multi-cloud và mã hóa.
- **Hỗ trợ**: Liên hệ team hoặc mở issue trên repository để được hỗ trợ.