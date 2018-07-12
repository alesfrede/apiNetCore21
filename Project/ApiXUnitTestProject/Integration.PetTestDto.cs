namespace ApiXUnitTestProject
{
    public partial class Integration
    {
        /// <summary>
        ///
        /// </summary>
        public class PetTestDto
        {
            public PetTestDto(int id, string name, string description)
            {
                Id = id;
                Name = name;
                Description = description;
            }

            public PetTestDto()
            {

            }

            /// <summary>
            /// 
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Description { get; set; }
        }

    }
} 
