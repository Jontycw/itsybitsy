using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItsyBitsy.Data
{
    [Table("Page")]
    public class Page
    {
        public Page()
        {
            if (TimeStamp == default)
                TimeStamp = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(4000), DataType(DataType.Url)]
        public string Uri { get; set; }

        [Required]
        [MaxLength(3)]
        public string StatusCode { get; set; }

        public byte ContentType { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime TimeStamp { get; set; }

        public virtual Website Website { get; set; }

        [ForeignKey("Website")]
        public int WebsiteId { get; set; }

        public virtual Session Session { get; set; }

        [ForeignKey("Session")]
        public int SessionId { get; set; }

        [DataType(DataType.Duration)]
        public long DownloadTime { get; set; }
        public int ContentLength { get; set; }
    }
}
