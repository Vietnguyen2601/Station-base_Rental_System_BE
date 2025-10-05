using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVStationRental.Common.DTOs
{
    public class UpdateAccountDTO
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự.")]
        public string Username { get; set; } = null!;

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự.")]
        public string? Password { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
        public string? ContactNumber { get; set; }

        public bool IsActive { get; set; } = true;

    }
}
