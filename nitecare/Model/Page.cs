using System;
using System.Collections.Generic;

#nullable disable

namespace nitecare.Model
{
    public partial class Page
    {
        public int PageId { get; set; }
        public string PageName { get; set; }
        public string PageImagePath { get; set; }
        public string PageContent { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
    }
}
