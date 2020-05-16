using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItsyBitsy.Data
{
    [Table("Website")]
    public class Website
    {
        public Website()
        {
            Pages = new List<Page>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Seed { get; set; }
        public virtual ICollection<Page> Pages { get; set; }
    }
}
