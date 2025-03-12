using System;

namespace ChildApi.Application.Messaging
{
    // Lớp cache đơn giản lưu trữ ParentId nhận được từ RabbitMQ.
    public class ParentIdCache
    {
        public Guid ParentId { get; set; } = Guid.Empty;
    }
}
