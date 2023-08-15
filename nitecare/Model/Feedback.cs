using System;
using System.Collections.Generic;

#nullable disable

namespace nitecare.Model
{
    public partial class Feedback
    {
        public Feedback()
        {
            Orders = new HashSet<Order>();
        }

        public int FeedbackId { get; set; }
        public string FeedbackContent { get; set; }
        public string FeedbackAvatar { get; set; }
        public string FeedbackImage { get; set; }
        public string StoreFeedback { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
