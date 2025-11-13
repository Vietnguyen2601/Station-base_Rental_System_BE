using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVStationRental.Common.Enums.EnumModel;
using EVStationRental.Repositories.Base;
using EVStationRental.Repositories.DBContext;
using EVStationRental.Repositories.IRepositories;
using EVStationRental.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace EVStationRental.Repositories.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly ElectricVehicleDBContext _context;

        public OrderRepository(ElectricVehicleDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Vehicle)
                    .ThenInclude(v => v.Model)
                .Include(o => o.Promotion)
                .Include(o => o.Staff)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<List<Order>> GetOrdersByCustomerIdAsync(Guid customerId)
        {
            return await _context.Orders
                .Include(o => o.Vehicle)
                    .ThenInclude(v => v.Model)
                .Include(o => o.Promotion)
                .Where(o => o.CustomerId == customerId && o.Isactive)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            order.CreatedAt = DateTime.Now;
            order.Isactive = true;
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> UpdateOrderAsync(Order order)
        {
            // Use AsTracking to enable change tracking for this query
            var existingOrder = await _context.Orders
                .AsTracking()
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
                
            if (existingOrder == null)
            {
                return null;
            }

            existingOrder.EndTime = order.EndTime;
            existingOrder.ReturnTime = order.ReturnTime;
            existingOrder.TotalPrice = order.TotalPrice;
            existingOrder.PromotionId = order.PromotionId;
            existingOrder.StaffId = order.StaffId;
            existingOrder.Status = order.Status;
            existingOrder.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return existingOrder;
        }

        public async Task<bool> IsVehicleAvailableAsync(Guid vehicleId, DateTime startTime, DateTime endTime)
        {
            // Check if vehicle exists and is available
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);

            if (vehicle == null || vehicle.Status != VehicleStatus.AVAILABLE)
            {
                return false;
            }

            // Check for overlapping orders with statuses that would block availability
            var hasConflict = await _context.Orders
                .AnyAsync(o =>
                    o.VehicleId == vehicleId &&
                    o.Isactive &&
                    (o.Status == OrderStatus.PENDING || 
                     o.Status == OrderStatus.CONFIRMED || 
                     o.Status == OrderStatus.ONGOING) &&
                    o.StartTime < endTime &&
                    (o.EndTime == null || o.EndTime > startTime));

            return !hasConflict;
        }

        public async Task<List<Order>> GetActiveOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Vehicle)
                    .ThenInclude(v => v.Model)
                .Where(o => o.Isactive)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByVehicleIdAsync(Guid vehicleId)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Promotion)
                .Where(o => o.VehicleId == vehicleId && o.Isactive)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Vehicle)
                    .ThenInclude(v => v.Station)
                .Include(o => o.Vehicle)
                    .ThenInclude(v => v.Model)
                .Include(o => o.Customer)
                .Include(o => o.Promotion)
                .Where(o => o.Isactive)
                .ToListAsync();
        }

        /// <summary>
        /// Get order by order code (6-character code)
        /// </summary>
        public async Task<Order?> GetOrderByCodeAsync(string orderCode)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Vehicle)
                    .ThenInclude(v => v.Model)
                .Include(o => o.Promotion)
                .Include(o => o.Staff)
                .Include(o => o.Contracts)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode && o.Isactive);
        }

        /// <summary>
        /// Creates order with deposit using wallet - WITHOUT stored procedure
        /// Uses transaction to ensure atomicity
        /// </summary>
        public async Task<Guid> CreateOrderWithDepositUsingWalletAsync(
            Guid customerId,
            Guid vehicleId,
            DateTime orderDate,
            DateTime startTime,
            DateTime endTime,
            decimal basePrice,
            decimal totalPrice,
            decimal depositAmount,
            string paymentMethod,
            Guid? promotionId = null,
            Guid? staffId = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Check vehicle availability
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
                
                if (vehicle == null)
                    throw new Exception($"Vehicle {vehicleId} does not exist");
                
                if (vehicle.Status != VehicleStatus.AVAILABLE)
                    throw new Exception($"Vehicle {vehicleId} not available (status={vehicle.Status})");

                // 2. Generate unique 6-char order code
                string orderCode;
                do
                {
                    orderCode = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
                } while (await _context.Orders.AnyAsync(o => o.OrderCode == orderCode));

                // 3. Create order
                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = customerId,
                    VehicleId = vehicleId,
                    OrderCode = orderCode,
                    OrderDate = orderDate,
                    StartTime = startTime,
                    EndTime = endTime,
                    BasePrice = basePrice,
                    TotalPrice = totalPrice,
                    Status = OrderStatus.CONFIRMED,
                    PromotionId = promotionId,
                    StaffId = staffId,
                    CreatedAt = DateTime.Now,
                    Isactive = true
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                // 4. Create contract
                var contract = new Contract
                {
                    ContractId = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    CustomerId = customerId,
                    VehicleId = vehicleId,
                    ContractDate = DateTime.Now,
                    FileUrl = "", // Staff will update later
                    CreatedAt = DateTime.Now,
                    Isactive = true
                };

                await _context.Contracts.AddAsync(contract);
                await _context.SaveChangesAsync();

                // 5. Handle deposit payment
                if (paymentMethod.ToUpper() == "WALLET" && depositAmount > 0)
                {
                    // Get wallet
                    var wallet = await _context.Wallets
                        .FirstOrDefaultAsync(w => w.AccountId == customerId);
                    
                    if (wallet == null)
                        throw new Exception($"Wallet for account {customerId} not found");
                    
                    if (wallet.Balance < depositAmount)
                        throw new Exception($"Insufficient wallet balance for account {customerId}");

                    // Debit wallet
                    wallet.Balance -= depositAmount;
                    wallet.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();

                    // Log wallet transaction (negative amount for deduction)
                    var walletTransaction = new WalletTransaction
                    {
                        TransactionId = Guid.NewGuid(),
                        WalletId = wallet.WalletId,
                        OrderId = order.OrderId,
                        Amount = -depositAmount, // Negative amount to indicate deduction
                        TransactionType = TransactionType.DEPOSIT, // Use DEPOSIT for deposit-related transactions
                        Description = $"Deposit deduction for order {order.OrderCode}",
                        CreatedAt = DateTime.Now,
                        Isactive = true
                    };

                    await _context.WalletTransactions.AddAsync(walletTransaction);
                    await _context.SaveChangesAsync();

                    // Create payment record as COMPLETED
                    var payment = new Payment
                    {
                        PaymentId = Guid.NewGuid(),
                        OrderId = order.OrderId,
                        GatewayTxId = $"WALLET-{Guid.NewGuid()}",
                        Amount = depositAmount,
                        PaymentDate = DateTime.Now,
                        PaymentMethod = "WALLET",
                        PaymentType = PaymentType.DEPOSIT,
                        Status = "COMPLETED",
                        CreatedAt = DateTime.Now,
                        Isactive = true
                    };

                    await _context.Payments.AddAsync(payment);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Create payment as PENDING for external gateway
                    var payment = new Payment
                    {
                        PaymentId = Guid.NewGuid(),
                        OrderId = order.OrderId,
                        Amount = depositAmount,
                        PaymentDate = DateTime.Now,
                        PaymentMethod = paymentMethod,
                        PaymentType = PaymentType.DEPOSIT,
                        Status = "PENDING",
                        CreatedAt = DateTime.Now,
                        Isactive = true
                    };

                    await _context.Payments.AddAsync(payment);
                    await _context.SaveChangesAsync();
                }

                // 6. Update vehicle status to RENTED
                vehicle.Status = VehicleStatus.RENTED;
                vehicle.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return order.OrderId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Finalize return payment using wallet - WITHOUT stored procedure
        /// Uses transaction to ensure atomicity
        /// </summary>
        public async Task<decimal> FinalizeReturnPaymentUsingWalletAsync(
            Guid orderId,
            string finalPaymentMethod = "WALLET")
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Get order
                var order = await _context.Orders
                    .Include(o => o.Payments)
                    .Include(o => o.Vehicle)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);
                
                if (order == null)
                    throw new Exception($"Order {orderId} not found");

                // 2. Calculate deposit sum
                var depositSum = order.Payments
                    .Where(p => p.PaymentType == PaymentType.DEPOSIT && p.Status == "COMPLETED" && p.Isactive)
                    .Sum(p => p.Amount);

                // 3. Calculate amount due
                var amountDue = order.TotalPrice - depositSum;

                // 4. Handle payment based on amount due
                if (amountDue > 0)
                {
                    // Customer needs to pay more
                    if (finalPaymentMethod.ToUpper() == "WALLET")
                    {
                        var wallet = await _context.Wallets
                            .FirstOrDefaultAsync(w => w.AccountId == order.CustomerId);
                        
                        if (wallet == null)
                            throw new Exception($"Wallet not found for account {order.CustomerId}");
                        
                        if (wallet.Balance < amountDue)
                            throw new Exception($"Insufficient wallet balance to settle final amount {amountDue} for order {orderId}");

                        // Debit wallet
                        wallet.Balance -= amountDue;
                        wallet.UpdatedAt = DateTime.Now;
                        await _context.SaveChangesAsync();

                        // Log wallet transaction
                        var walletTransaction = new WalletTransaction
                        {
                            TransactionId = Guid.NewGuid(),
                            WalletId = wallet.WalletId,
                            OrderId = orderId,
                            Amount = amountDue,
                            TransactionType = TransactionType.PAYMENT,
                            Description = $"Final payment for order {order.OrderCode}",
                            CreatedAt = DateTime.Now,
                            Isactive = true
                        };

                        await _context.WalletTransactions.AddAsync(walletTransaction);
                        await _context.SaveChangesAsync();

                        // Create payment record as COMPLETED
                        var payment = new Payment
                        {
                            PaymentId = Guid.NewGuid(),
                            OrderId = orderId,
                            GatewayTxId = $"WALLET-{Guid.NewGuid()}",
                            Amount = amountDue,
                            PaymentDate = DateTime.Now,
                            PaymentMethod = "WALLET",
                            PaymentType = PaymentType.FINAL,
                            Status = "COMPLETED",
                            CreatedAt = DateTime.Now,
                            Isactive = true
                        };

                        await _context.Payments.AddAsync(payment);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Create FINAL payment PENDING for external gateway
                        var payment = new Payment
                        {
                            PaymentId = Guid.NewGuid(),
                            OrderId = orderId,
                            Amount = amountDue,
                            PaymentDate = DateTime.Now,
                            PaymentMethod = finalPaymentMethod,
                            PaymentType = PaymentType.FINAL,
                            Status = "PENDING",
                            CreatedAt = DateTime.Now,
                            Isactive = true
                        };

                        await _context.Payments.AddAsync(payment);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (amountDue < 0)
                {
                    // Need to refund customer (overpaid)
                    var payment = new Payment
                    {
                        PaymentId = Guid.NewGuid(),
                        OrderId = orderId,
                        Amount = Math.Abs(amountDue),
                        PaymentDate = DateTime.Now,
                        PaymentMethod = finalPaymentMethod,
                        PaymentType = PaymentType.REFUND,
                        Status = "PENDING",
                        CreatedAt = DateTime.Now,
                        Isactive = true
                    };

                    await _context.Payments.AddAsync(payment);
                    await _context.SaveChangesAsync();
                }

                // 5. Update order status to COMPLETED
                order.Status = OrderStatus.COMPLETED;
                order.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                // 6. Update vehicle status to AVAILABLE
                if (order.Vehicle != null)
                {
                    order.Vehicle.Status = VehicleStatus.AVAILABLE;
                    order.Vehicle.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return amountDue;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
