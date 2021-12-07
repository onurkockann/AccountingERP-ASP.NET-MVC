namespace AccountingERP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Article")]
    public partial class Article
    {
        public int ArticleID { get; set; }

        [StringLength(4000)]
        public string Content { get; set; }

        public DateTime? EventDate { get; set; }

        public int? FirmUserID { get; set; }

        public int? FirmID { get; set; }

        public virtual Firm Firm { get; set; }

        public virtual FirmUser FirmUser { get; set; }
    }
}
