using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopListMvc.Data
{
    /// <summary>
    /// Список продуктов
    /// </summary>
    public class ProductsGroup
    {
        public int Id { get; set; }
        /// <summary>
        /// Название списка
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Пользователь создавший список
        /// </summary>
        [Required]
        public string UserId { get; set; }
        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Удалён
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Продукты в списке
        /// </summary>
        public ICollection<Product> Products { get; set; }

        /// <summary>
        /// Пользователь создавший список
        /// </summary>
        public IdentityUser User { get; set; }
    }
}
