using System.Threading.Tasks;
using Api213.V2.Dto;
using Api213.V2.Helper;
using Api213.V2.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Api213.V2.Controllers
{
    /// <summary>
    ///     IPetsController
    /// </summary>
    public interface IPetsController
    {
        /// <summary>
        ///     Read All
        /// </summary>
        /// <param name="filteringSortingParams">sort y custom fields</param>
        /// <returns></returns>
        /// <response code="200">successfully retrieved.</response>
        /// <response code="400">BadRequest parameters</response>
        Task<IActionResult> ReadAllAsync(FilteringSortingParams filteringSortingParams);

        /// <summary>
        ///     Get by Name
        /// </summary>
        /// <param name="petName">name</param>
        /// <returns>Pet</returns>
        /// <response code="200">successfully retrieved.</response>
        /// <response code="400">is invalid.</response>
        /// <response code="404">NotFound.</response>
        Task<IActionResult> ReadOneAsync(string petName);

        /// <summary>
        ///     Create new object
        /// </summary>
        /// <param name="aPet"></param>
        /// <returns></returns>
        /// <response code="201">Successfully Created and Location</response>
        /// <response code="400">Unable to create.</response>
        /// <response code="500">Unable to create.Exception.</response>
        Task<IActionResult> CreateAsync([FromBody] PetDto aPet);

        /// <summary>
        ///     Replace Objet
        /// </summary>
        /// <param name="aPet"></param>
        /// <returns></returns>
        /// <response code="200">successfully retrieved.</response>
        /// <response code="400">BadRequest</response>
        /// <response code="404">NotFound</response>
        Task<IActionResult> UpdateAsync([FromBody] PetDto aPet);

        /// <summary>
        ///     Delete one
        /// </summary>
        /// <param name="petName">Name of pet</param>
        /// <returns></returns>
        /// <response code="200">successfully retrieved.</response>
        /// <response code="404">NotFound</response>
        /// <response code="409">Exception</response>
        Task<IActionResult> DeleteAsync(string petName);

        /// <summary>
        ///     Search
        /// </summary>
        /// <param name="namelike">by name like</param>
        /// <param name="filteringSortingParams">sort</param>
        /// <returns>Pets</returns>
        /// <response code="200">successfully retrieved.</response>
        /// <response code="404">NotFound</response>
        /// <response code="400">Status400 BadRequest</response>
        IActionResult Search([FromQuery] string namelike, FilteringSortingParams filteringSortingParams);

        /// <summary>
        ///     JsonPatch to Apply the changes for  properties .
        ///     rfc7386 : This specification defines the JSON merge patch format and processing
        ///     rules.The merge patch format is primarily intended for use with the
        ///     HTTP PATCH method as a means of describing a set of modifications to
        ///     a target resource's content.
        ///     based in https://tools.ietf.org/html/rfc7386
        /// </summary>
        /// <param name="petName">id</param>
        /// <param name="patch">JsonPatchDocument</param>
        /// <returns>updated</returns>
        /// <response code="200">operation successfully.</response>
        /// <response code="400">BadRequest </response>
        /// <response code="404">NotFound</response>
        /// <response code="412">Format Error to patch.</response>
        /// <response code="409">Unable to update.</response>
        Task<IActionResult> Patch([FromRoute] string petName, [FromBody] JsonPatchDocument<PetEntity> patch);
    }
}




