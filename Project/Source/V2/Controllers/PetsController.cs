using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api213.V2.Dto;
using Api213.V2.Exception;
using Api213.V2.Helper;
using Api213.V2.Interface;
using Api213.V2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Api213.V2.Controllers
{
    /// <inheritdoc cref="IPetsController" />
    /// <summary>
    ///     Pets Controller
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ApiController]
    public class PetsController : ControllerBase, IPetsController
    {
        private readonly IPetsManager _manager;

        /// <inheritdoc />
        public PetsController(IPetsManager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        /// <inheritdoc />
        /// <summary>
        ///     Read All
        /// </summary>
        /// <param name="filteringSortingParams">sort y custom fields</param>
        /// <returns></returns>
        /// <response code="200">successfully retrieved.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PetDto>), 200)]
        public async Task<IActionResult> ReadAllAsync([FromQuery] FilteringSortingParams filteringSortingParams)
        {
            return Ok(await _manager.Get(filteringSortingParams));
        }

        /// <inheritdoc />
        /// <summary>
        ///     Get by Name
        /// </summary>
        /// <param name="petName">name</param>
        /// <returns>Pet</returns>
        /// <response code="200">successfully retrieved.</response>
        /// <response code="400">is invalid.</response>
        /// <response code="404">NotFound.</response>
        [HttpGet("{petName}")]
        [ProducesResponseType(typeof(PetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReadOneAsync([FromRoute] string petName)
        {
            try
            {
                var pet = await _manager.ReadOne(petName);
                return Ok(ToView(pet));
            }
            catch (NotFoundException ex)
            {
                return InvalidResponseFactory(NotFound(ex.Message));
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Create new object
        /// </summary>
        /// <param name="aPet"></param>
        /// <returns></returns>
        /// <response code="201">successfully Created.</response>
        /// <response code="405">Unable to create.</response>
        /// <response code="400">BadRequest.</response>
        [HttpPost]
        [ProducesResponseType(typeof(PetDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public async Task<IActionResult> CreateAsync([FromBody] PetDto aPet)
        {
            try
            {
                var createdPet = await _manager.Create(Map(aPet));
                return CreatedLocation201(ToView(createdPet));
            }
            catch (System.Exception e)
            {
                return InvalidResponseFactory(StatusCode(StatusCodes.Status405MethodNotAllowed, e.Message));
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Replace Objet
        /// </summary>
        /// <param name="aPet"></param>
        /// <returns></returns>
        /// <response code="200">successfully retrieved.</response>
        /// <response code="400">BadRequest</response>
        /// <response code="404">NotFound</response>
        [HttpPut]
        [ProducesResponseType(typeof(PetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] PetDto aPet)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var updatedPet = await _manager.Replace(Map(aPet));
                return Ok(ToView(updatedPet));
            }
            catch (NotFoundException ex)
            {
                return InvalidResponseFactory(NotFound(ex.Message));
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Delete one
        /// </summary>
        /// <param name="petName">Name of pet</param>
        /// <returns></returns>
        /// <response code="200">successfully retrieved.</response>
        /// <response code="404">NotFound</response>
        [HttpDelete("{petName}")]
        [ProducesResponseType(typeof(PetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync([FromRoute] string petName)
        {
            try
            {
                var deletedPet = await _manager.Delete(petName);
                return Ok(ToView(deletedPet));
            }
            catch (NotFoundException ex)
            {
                return InvalidResponseFactory(NotFound(ex.Message));
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Search
        /// </summary>
        /// <param name="namelike">by name like</param>
        /// <param name="filteringSortingParams">sort</param>
        /// <returns>Pets</returns>
        /// <response code="200">successfully retrieved.</response>
        /// <response code="404">NotFound</response>
        [HttpGet("Search")]
        [ProducesResponseType(typeof(IEnumerable<PetDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Search(
            [FromQuery] string namelike,
            [FromQuery] FilteringSortingParams filteringSortingParams)
        {
            var pets = _manager.GetByNameSubstring(namelike, filteringSortingParams);
            if (!pets.Any()) return InvalidResponseFactory(NotFound(namelike));

            return Ok(pets);
        }

        /// <summary>
        ///     JsonPatch to Apply the changes for  properties .
        ///     {"op" : "replace",
        ///     "path" : "property",
        ///     "value" : "newvalue"}
        /// </summary>
        /// <param name="petName">id</param>
        /// <param name="patch">JsonPatchDocument</param>
        /// <returns>updated</returns>
        /// <response code="200">operation successfully.</response>
        /// <response code="400">BadRequest </response>
        /// <response code="404">NotFound</response>
        /// <response code="405">Unable to update.</response>
        [HttpPatch("{petName}", Name = "UpdateOne")]
        [Consumes("application/json-patch+json", "application/json")]
        public async Task<IActionResult> Patch([FromRoute] string petName,
            [FromBody] JsonPatchDocument<PetEntity> patch)
        {
            try
            {
                var pet = _manager.ReadOne(petName).Result;
                patch.ApplyTo(pet, ModelState);
                if (!ModelState.IsValid)
                    return InvalidResponseFactory(StatusCodes.Status400BadRequest, "JsonPatchDocument.ApplyTo");

                await _manager.Update(pet);

                return Ok(pet);
            }
            catch (NotFoundException ex)
            {
                return InvalidResponseFactory(NotFound(ex.Message));
            }
            catch (System.Exception e)
            {
                return InvalidResponseFactory(StatusCode(StatusCodes.Status405MethodNotAllowed, e.Message));
            }
        }

        /// <summary>
        /// InvalidResponseFactory for ObjectResult
        /// </summary>
        /// <param name="objectResult"></param>
        /// <returns></returns>
        private IActionResult InvalidResponseFactory(ObjectResult objectResult)
        {
            return InvalidResponseFactory(objectResult.StatusCode, objectResult.Value.ToString());
        }

        /// <summary>
        /// Errors : InvalidResponseFactory
        /// </summary>
        /// <param name="code"></param>
        /// <param name="extraMessage"></param>
        /// <returns></returns>
        private IActionResult InvalidResponseFactory(int? code, string extraMessage)
        {
            var codehttp = code ?? 500;

            ModelState.AddModelError("ExtraMessage", extraMessage);
            var problemDetails = new ValidationProblemDetails(ModelState)
            {
                Instance = HttpContext.Request.Path,
                Status = code,
                Type = "https://asp.net/core",
                Detail = "HttpContext:" + HttpContext.Request.Method + " " + HttpContext.Request.Path + ", " + (char)13 +
                         "ExtraMessage: " + extraMessage + (char)13 +
                         ", Please refer to the errors property for additional details."
            };
            if (code == null || (code > 511 || code < 100))
            {
                codehttp = 500;
            }

            var type = new MediaTypeCollection { "application/problem+json" };
            var objectResult = StatusCode(codehttp, problemDetails);
            objectResult.ContentTypes = type;
            return objectResult;
        }

        /// <summary>
        ///     map
        /// </summary>
        /// <param name="pet"></param>
        /// <returns></returns>
        private static PetDto ToView(PetEntity pet)
        {
            return new PetDto {Id = pet.Id, Description = pet.Description, Name = pet.Name};
        }

        /// <summary>
        ///     map
        /// </summary>
        /// <param name="aPet"></param>
        /// <returns></returns>
        private PetEntity Map(PetDto aPet)
        {
            return new PetEntity {Id = aPet.Id, Description = aPet.Description, Name = aPet.Name};
        }

        /// <summary>
        ///     Status201Created + URI read new object
        /// </summary>
        /// <param name="oPet"></param>
        /// <returns></returns>
        private CreatedAtActionResult CreatedLocation201(PetDto oPet)
        {
            return CreatedAtAction(
                nameof(ReadOneAsync),
                new {petName = oPet.Name},
                oPet);
        }
    }
}