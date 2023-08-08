using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.BaseModel
{
    public class PageVm
    {
        public int PageId { get; set; }
        public string PageName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PageContent { get; set; }
        public string ImageName { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
