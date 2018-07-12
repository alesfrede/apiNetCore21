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
        [System.ComponentModel.DataAnnotations.Required]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
    }
}