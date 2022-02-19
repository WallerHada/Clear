using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectWebAPI.Models.Tables
{
    [Table("blogs")]
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 插入时生成
        [Column("blog_id")] // 数据库列重命名
        public int BlogId { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$")] // 只能使用字母。第一个字母必须为大写。 允许使用空格，但不允许使用数字和特殊字符。
        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Title { get; set; }

        // [Comment("The URL of the blog")] // 列注释；也可表注释
        [Required] // 必填
        [Column(TypeName = "varchar(200)")] // 配置为非 unicode 字符串，其最大长度为 200
        public string Url { get; set; }

        [Range(1, 100)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(5, 2)")] // 将 Rating 配置为十进制，其精度为 5，小数位数为 2
        public decimal Rating { get; set; }

        [NotMapped] // 建表时排除该属性；置于class类时，排除表
        public DateTime LoadedFromDatabase { get; set; }

        [MaxLength(500)] // 最大长度500
        public string Url2 { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 插入时生成
        public DateTime Inserted { get; set; }
    }
}
