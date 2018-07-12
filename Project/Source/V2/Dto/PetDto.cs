namespace Api213.V2.Dto
{
    /// <summary>
    /// PetOutPut 
    /// </summary>
    public class PetDto
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