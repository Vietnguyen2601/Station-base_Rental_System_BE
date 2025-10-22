using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVStationRental.Common.Helpers
{
    public class ErrorDetail
    {
        public string? Field { get; set; }     // Tên field lỗi
        public string Message { get; set; } = string.Empty;  // Thông báo lỗi

        public ErrorDetail(string message, string? field = null)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Field = field;
        }
    }
}
