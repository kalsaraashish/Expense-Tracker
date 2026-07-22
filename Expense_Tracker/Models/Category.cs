using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Column(TypeName ="nvarchar(50)")]
        [Required]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(5)")]
        public string Tcon { get; set; } = "";
        
        [Column(TypeName ="nvarchar(10)")]
        public string Type { get; set; }= "Expense";

        [NotMapped]
        public String? TitleWithIcon
        {
            get
            {
                return this.Tcon + " " + this.Title;
            }
        }
    }
}
