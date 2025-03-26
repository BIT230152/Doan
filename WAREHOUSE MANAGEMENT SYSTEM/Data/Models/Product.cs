namespace WAREHOUSE_MANAGEMENT_SYSTEM.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.InteropServices;

    public class Product : BaseModel
    {

        [Required]
        [Display(Name = "Tên sản phẩm")]
        [MaxLength(50, ErrorMessage = "Độ dài tối đa của trường 'Tên sản phẩm' là 50 ký tự")]
        public string Name { get; set; }

        [Display(Name = "Miêu tả")]
        [MaxLength(2000, ErrorMessage = "Độ dài tối đa của trường 'Miêu tả' là 200 0 ký tự")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Số lượng")]
        [Range(0, double.MaxValue, ErrorMessage = "Số lượng sản phẩm không thể dưới 0")]
        public int Count { get; set; }

        

        [Required]
        [Display(Name = "Giá mua")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ", ApplyFormatInEditMode = false)]
        [Range(0, double.MaxValue, ErrorMessage = "Giá mua không thể dưới 0")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Cost { get; set; }

        [Required]
        [Display(Name = "Giá bán")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ", ApplyFormatInEditMode = false)]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán không thể dưới 0")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }



        [Display(Name = "Hình ảnh")]
        [Url(ErrorMessage = "URL không hợp lệ")]
        public string ImageUrl { get; set; }



        [Required]
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }




    }
}
