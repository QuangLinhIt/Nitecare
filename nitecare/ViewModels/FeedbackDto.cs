using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.ViewModels
{
    public class FeedbackDto
    {
        public int FeedbackId { get; set; }
        public string UserName { get; set; }
        public string FeedbackContent { get; set; }
        public string FeedbackAvatar { get; set; }
        public string FeedbackImage { get; set; }
    }
}
