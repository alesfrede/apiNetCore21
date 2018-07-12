using Api213.V2.Models;

namespace Api213.V2.Exception
{
    /// <inheritdoc />
    /// <summary>
    /// PetNotFoundException
    /// </summary>
    public class PetNotFoundException 
        : ApiException
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="PetNotFoundException"/> class.
        /// 
        /// </summary>
        /// <param name="aPet"></param>
        public PetNotFoundException(PetEntity aPet)
            : this(aPet.Name)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="PetNotFoundException"/> class.
        /// 
        /// </summary>
        /// <param name="petName"></param>
        public PetNotFoundException(string petName)
            : base($"  {petName} was not found.")
        {
        }
    }
}
