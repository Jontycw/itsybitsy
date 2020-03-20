using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ItsyBitsy.Data
{
    public class PageLinks
    {
        [Key, ForeignKey("PageId")]
        public Page Page { get; set; }
        public int PageId { get; set; }

        [Key, ForeignKey("ParentPageId")]
        public Page ParentPage { get; set; }
        public int ParentPageId { get; set; }

        [Key, ForeignKey("SessionId")]
        public Session Session { get; set; }
        public int SessionId { get; set; }
    }
}
