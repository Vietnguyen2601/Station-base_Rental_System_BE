using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVStationRental.Common.Enums
{
    public enum ResultStatus
    {
        Error = -1,          // Lỗi chung
        Success = 200,       // Thành công
        Created = 201,       // Tạo mới thành công
        ValidationError = 400, // Lỗi validation
        NotFound = 404,      // Không tìm thấy
        Conflict = 409,      // Xung đột
        InternalError = 500  // Lỗi server
        // Thêm các mã trạng thái khác nếu cần
    }
}
