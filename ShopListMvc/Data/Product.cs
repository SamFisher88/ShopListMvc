using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopListMvc.Data
{
    // Продукт для покупки
    public class Product
    {
        public int Id { get; set; }
        /// <summary>
        /// Название продукта
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Идентификатор списка продуктов
        /// </summary>
        public int ProductsGroupId { get; set; }
        /// <summary>
        /// Индекс сортировки
        /// </summary>
        public int OrderIndex { get; set; }
        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Кем создан
        /// </summary>
        [Required]
        public string CreatedBy { get; set; }
        /// <summary>
        /// Завершён (куплен)
        /// </summary>
        public bool IsFinish { get; set; }
        /// <summary>
        /// Когда куплен
        /// </summary>
        public DateTime FinishedAt { get; set; }
        /// <summary>
        /// Кем куплен
        /// </summary>
        [Required]
        public string FinishedBy { get; set; }
        /// <summary>
        /// Удалён
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Список продуктов
        /// </summary>
        public ProductsGroup ProductsGroup { get; set; }

        /// <summary>
        /// Пользователь создавший продукт
        /// </summary>
        [ForeignKey("CreatedBy")]
        public IdentityUser CreatedUser { get; set; }

        /// <summary>
        /// Пользователь завершивший продукт
        /// </summary>
        [ForeignKey("FinishedBy")]
        public IdentityUser FinishedUser { get; set; }
    }
}
