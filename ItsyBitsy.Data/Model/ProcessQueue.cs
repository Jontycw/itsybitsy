using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItsyBitsy.Data
{
    [Table("ProcessQueue")]
    public class ProcessQueue
    {
        public ProcessQueue()
        {
            if (TimeStamp == default)
                TimeStamp = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(4000), DataType(DataType.Url)]
        public string Link { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TimeStamp { get; set; }

        [ForeignKey("Session")]
        public int SessionId { get; set; }
        public virtual Session Session { get; set; }

        [ForeignKey("Website")]
        public int WebsiteId { get; set; }
        public virtual Website Website { get; set; }
        public int ParentId { get; set; }
    }
}
