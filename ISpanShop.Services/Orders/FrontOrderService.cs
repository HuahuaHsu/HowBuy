using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISpanShop.Common.Enums;
using ISpanShop.Models.DTOs.Orders;
using ISpanShop.Repositories.Orders;

namespace ISpanShop.Services.Orders
{
    public class FrontOrderService : IFrontOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public FrontOrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<FrontOrderListDto>> GetMemberOrdersAsync(int memberId)
        {
            var orders = await _orderRepository.GetOrdersByMemberIdAsync(memberId);
            
            return orders.Select(o => new FrontOrderListDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CreatedAt = o.CreatedAt,
                FinalAmount = o.FinalAmount,
                Status = (OrderStatus)(o.Status ?? 0),
                StatusName = GetStatusName(o.Status),
                StoreName = o.Store?.StoreName ?? "未知商店",
                FirstProductName = o.OrderDetails.FirstOrDefault()?.ProductName,
                FirstProductImage = o.OrderDetails.FirstOrDefault()?.CoverImage,
                TotalItemCount = o.OrderDetails.Sum(od => od.Quantity)
            }).ToList();
        }

        public async Task<FrontOrderDetailDto> GetOrderDetailAsync(long orderId, int memberId)
        {
            var o = await _orderRepository.GetOrderByIdAsync(orderId);
            
            // 安全性檢查：確保訂單屬於該會員
            if (o == null || o.UserId != memberId)
            {
                return null;
            }

            return new FrontOrderDetailDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CreatedAt = o.CreatedAt,
                PaymentDate = o.PaymentDate,
                CompletedAt = o.CompletedAt,
                TotalAmount = o.TotalAmount,
                ShippingFee = o.ShippingFee,
                FinalAmount = o.FinalAmount,
                Status = (OrderStatus)(o.Status ?? 0),
                StatusName = GetStatusName(o.Status),
                StoreName = o.Store?.StoreName ?? "未知商店",
                RecipientName = o.RecipientName,
                RecipientPhone = o.RecipientPhone,
                RecipientAddress = o.RecipientAddress,
                Note = o.Note,
                Items = o.OrderDetails.Select(od => new FrontOrderItemDto
                {
                    Id = od.Id,
                    ProductId = od.ProductId,
                    VariantId = od.VariantId,
                    ProductName = od.ProductName,
                    VariantName = od.VariantName,
                    CoverImage = od.CoverImage,
                    Price = od.Price ?? 0,
                    Quantity = od.Quantity
                }).ToList()
            };
        }

        private string GetStatusName(byte? status)
        {
            return status switch
            {
                0 => "待付款",
                1 => "待出貨",
                2 => "運送中",
                3 => "已完成",
                4 => "已取消",
                5 => "退貨/款中",
                6 => "已退款",
                _ => "未知"
            };
        }
    }
}
