﻿using System.ComponentModel.DataAnnotations;

namespace Api213.V2.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class PetEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get;   set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MaxLength(50)]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Owner Owner { get; set; }
    }
}