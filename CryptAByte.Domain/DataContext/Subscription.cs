using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CryptAByte.Domain.DataContext
{
    public class Subscription
    {
        public Subscription()
        {
            Created = DateTime.Now;
        }

        [Key]
        public int SubscriptionId { get; set; }

        public string Email { get; set; }

        public DateTime Created { get; set; }

    }
}
