using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopListMvc.Data
{
    public class Profile
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        [MaxLength(20)]
        public string Nickname { get; set; }
        [MaxLength(50)]
        public string PhotoUrl { get; set; }
        [MaxLength(40)]
        public string UserName { get; set; }
        [MaxLength(255)]
        public string AboutUser { get; set; }
        public IdentityUser User { get; set; }
    }
}
