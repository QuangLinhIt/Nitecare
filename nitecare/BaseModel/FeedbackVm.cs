using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.BaseModel
{
    public class FeedbackVm
    {
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public string FeedbackContent { get; set; }
        public string AvatarName{ get; set; }
        public IFormFile AvatarFile { get; set; }
        public string ImageName { get; set; }
        public IFormFile ImageFile { get; set; }

    }
}
