using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ItsyBitsy.Data
{
    public enum ContentType : byte
    {
        Html = 0,
        Css = 1,
        Javascript = 2,
        Image = 3,
        Other = 4,
        Json = 5
    }

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
        [DataType(DataType.Url)]
        public string Uri { get; set; }

        [Required]
        [MaxLength(3)]
        public string StatusCode { get; set; }

        public ContentType ContentType { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime TimeStamp { get; set; }

        public virtual Website Website { get; set; }

        [ForeignKey("Website")]
        public int WebsiteId { get; set; }

        public virtual Session Session { get; set; }

        [ForeignKey("Session")]
        public int SessionId { get; set; }

        public virtual Page ParentPage { get; set; }

        [ForeignKey("ParentPage")]
        public int? ParentPageId { get; set; }

        [DataType(DataType.Duration)]
        public long DownloadTime { get; set; }
    }
}
