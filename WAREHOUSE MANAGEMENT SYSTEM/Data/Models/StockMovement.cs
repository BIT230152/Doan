namespace WAREHOUSE_MANAGEMENT_SYSTEM.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public class StockMovement : BaseModel
    {
        [Required]
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        [Display(Name = "Loại giao dịch")]
        public MovementType MovementType { get; set; }

        [Required]
        [Display(Name = "Số lượng")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Ngày giao dịch")]
        public DateTime MovementDate { get; set; } = DateTime.Now;

       
        [Required]
        [Display(Name = "Quốc gia")]
        [MaxLength(100)]
        public string Country { get; set; } // Thay thế cho Note
    }

    public enum MovementType
    {
        [Display(Name = "Nhập kho")]
        Import,
        [Display(Name = "Xuất kho")]
        Export
    }
}