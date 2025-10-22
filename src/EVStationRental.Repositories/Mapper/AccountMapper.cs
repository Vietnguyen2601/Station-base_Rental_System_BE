using EVStationRental.Common.DTOs;
using EVStationRental.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVStationRental.Repositories.Mapper
{
    public static class AccountMapper
    {
        public static ViewAccountDTO ToViewAccountDTO(this Account account)
        {
            if (account == null) return null!;
            return new ViewAccountDTO
            {
                AccountId = account.AccountId,
                Username = account.Username,
                Email = account.Email,
                ContactNumber = account.ContactNumber,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt,
                IsActive = account.Isactive,
                // Since the current database uses one-to-many relationship, get role from Role property
                RoleName = account.Role != null ? new List<string> { account.Role.RoleName } : new List<string>()
            };
        }

        //Chưa có làm hash + token nên chưa làm dc
        //public static Account ToAccount(this CreateAccountDTO dto)
        //{
        //    if (dto == null) throw new ArgumentNullException(nameof(dto));

        //    return new Account
        //    {
        //        AccountId = Guid.NewGuid(),
        //        Username = dto.Username,
        //        Password = dto.Password, // Will be hashed in service
        //        Email = dto.Email,
        //        ContactNumber = dto.ContactNumber,
        //        CreatedAt = DateTime.UtcNow,
        //        UpdatedAt = null,
        //        IsActive = true
        //        // AccountRoles: Created separately in service after account save
        //    };
        //}

        //public static void MapToAccount(this UpdateAccountDTO dto, Account account)
        //{
        //    if (dto == null) throw new ArgumentNullException(nameof(dto));
        //    if (account == null) throw new ArgumentNullException(nameof(account));

        //    account.Username = dto.Username ?? account.Username;
        //    if (!string.IsNullOrEmpty(dto.Password))
        //    {
        //        account.Password = dto.Password; // Will be hashed in service
        //    }
        //    account.Email = dto.Email ?? account.Email;
        //    account.ContactNumber = dto.ContactNumber ?? account.ContactNumber;
        //    account.IsActive = dto.IsActive;
        //    account.UpdatedAt = DateTime.UtcNow;
        //}

    }
}
