using System;
using System.Collections.Generic;

#nullable disable

namespace nitecare.Model
{
    public partial class Feedback
    {
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public string FeedbackContent { get; set; }
        public string FeedbackAvatar { get; set; }
        public string FeedbackImage { get; set; }

        public virtual User User { get; set; }
    }
}
