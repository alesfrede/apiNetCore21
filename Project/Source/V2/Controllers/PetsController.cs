﻿using System.Collections.Generic;
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

namespace Api213.V2.Controllers
{
    /// <inheritdoc cref="IPetsController" />
    /// <summary>
    ///     Pets Controller
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json", "application/problem+json")]
    [Consumes("application/json")]
    [ApiController]
    public class PetsController : ControllerBase, IPetsController
    {
        private readonly IPetsManager _manager;
        private readonly IInvalidResponseFactory _invalidResponseFactory;

        /// <inheritdoc />
        public PetsController(IPetsManager manager, IInvalidResponseFactory invalidResponseFactory)
        { 
            _manager = manager ?? throw new System.ArgumentNullException(nameof(manager));
            _invalidResponseFactory = invalidResponseFactory ?? throw new System.ArgumentNullException(nameof(invalidResponseFactory));
            _invalidResponseFactory.SetController(this);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Read All
        /// </summary>
        /// <param name="filteringSortingParams">sort, paging , embed , custom fields</param>
        /// <returns></returns>
        /// <response code="200">successfully retrieved.</response>
        /// <response code="400">BadRequest parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PetFullDto>), 200)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReadAllAsync([FromQuery] FilteringSortingParams filteringSortingParams)
        {
            try 
            {
                if (!ModelState.IsValid)
                    return _invalidResponseFactory.Response(new BadRequestObjectResult(ModelState));

                return Ok(await _manager.Get(filteringSortingParams));
            }
            catch (System.Exception ex) 
            {
                return _invalidResponseFactory.Response(new BadRequestObjectResult(ex), "Argumentos no válidos");
            }
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
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReadOneAsync([FromRoute] string petName)
        {
            try
            {
                var pet = await _manager.ReadOne(petName);
                return Ok(ToView(pet));
            }
            catch (NotFoundException ex)
            {
                return _invalidResponseFactory.Response(NotFound(ex));
            }
            catch (System.Exception ex)
            {
                return _invalidResponseFactory.Response(new BadRequestObjectResult(ex));
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Create new object
        /// </summary>
        /// <param name="aPet"></param>
        /// <returns></returns>
        /// <response code="201">Successfully Created and Location</response>
        /// <response code="400">Unable to create.</response>
        /// <response code="500">Unable to create.Exception.</response>
        [HttpPost]
        [ProducesResponseType(typeof(PetDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] PetDto aPet)
        {
            try
            {
                var createdPet = await _manager.Create(Map(aPet));
                return CreatedLocation201(ToView(createdPet));
            }
            catch (System.DuplicateWaitObjectException e)
            {
                return _invalidResponseFactory.Response(StatusCode(StatusCodes.Status400BadRequest, e.Message));
            }
            catch (System.Exception e)
            {
                return _invalidResponseFactory.Response(StatusCode(StatusCodes.Status500InternalServerError, e.Message));
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
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] PetDto aPet)
        {
            try
            {
                if (!ModelState.IsValid) return _invalidResponseFactory.Response(BadRequest(ModelState));

                var updatedPet = await _manager.Replace(Map(aPet));
                return Ok(ToView(updatedPet));
            }
            catch (NotFoundException ex)
            {
                return _invalidResponseFactory.Response(NotFound(ex.Message));
            }
            catch (System.Exception ex)
            {
                return _invalidResponseFactory.Response(StatusCode(StatusCodes.Status409Conflict, ex.Message));
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
        /// <response code="409">Exception</response>
        [HttpDelete("{petName}")]
        [ProducesResponseType(typeof(PetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteAsync([FromRoute] string petName)
        {
            try
            {
                var deletedPet = await _manager.Delete(petName);
                return Ok(ToView(deletedPet));
            }
            catch (NotFoundException ex)
            {
                return _invalidResponseFactory.Response(NotFound(ex.Message));
            }
            catch (System.Exception ex)
            {
                return _invalidResponseFactory.Response(StatusCode(StatusCodes.Status409Conflict, ex.Message));
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
        /// <response code="400">Status400 BadRequest</response>
        [HttpGet("Search")]
        [ProducesResponseType(typeof(IEnumerable<PetDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status400BadRequest)]
        public IActionResult Search(
            [FromQuery] string namelike,
            [FromQuery] FilteringSortingParams filteringSortingParams)
        {
            try
            {
                var pets = _manager.GetByNameSubstring(namelike, filteringSortingParams);
                if (!pets.Any())
                    return _invalidResponseFactory.Response(NotFound(namelike));

                return Ok(pets);
            }
            catch (System.Exception ex)
            {
                return _invalidResponseFactory.Response(new BadRequestObjectResult(ex));
            }
        }

        /// <inheritdoc></inheritdoc>
        /// <summary>
        ///     JsonPatch to Apply the changes for  properties based rfc7386
        ///     {"op" : "replace",     "path" : "property",     "value" : "newvalue"}
        /// </summary>
        /// <param name="petName">id</param>
        /// <param name="patch">JsonPatchDocument</param>
        /// <returns>updated</returns>
        /// <response code="200">operation successfully.</response>
        /// <response code="400">BadRequest </response>
        /// <response code="404">NotFound</response>
        /// <response code="412">Format Error to patch.</response>
        /// <response code="409">Unable to update.</response>
        [HttpPatch("{petName}", Name = "UpdateOne")]
        [Consumes("application/json-patch+json", "application/json")]
        [ProducesResponseType(typeof(PetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Patch([FromRoute] string petName,
            [FromBody] JsonPatchDocument<PetEntity> patch)
        {
            try
            {
                var pet = _manager.ReadOne(petName).Result;
                patch.ApplyTo(pet, ModelState);

                if (!ModelState.IsValid)
                    return _invalidResponseFactory.Response(StatusCodes.Status412PreconditionFailed, "JsonPatchDocument", "Argumentos no válidos");

                await _manager.Update(pet);

                return Ok(pet);
            }
            catch (NotFoundException ex)
            {
                return _invalidResponseFactory.Response(NotFound(ex.Message));
            }
            catch (System.Exception e)
            {
                return _invalidResponseFactory.Response(StatusCode(StatusCodes.Status409Conflict, e.Message));
            }
        }

        /// <summary>
        ///     JsonMerge
        ///      
        /// </summary>
        /// <param name="petName"></param>
        /// <param name="patch"></param>
        /// <returns>updated</returns>
        /// <response code="200">operation successfully.</response>
        /// <response code="400">BadRequest </response>
        /// <response code="404">NotFound</response>
        /// <response code="412">Format Error to patch.</response>
        /// <response code="409">Unable to update.</response>
        [HttpPatch("Merge/{petName}", Name = "Merge")]
        [Consumes("application/json-patch+json", "application/json")]
        [ProducesResponseType(typeof(PetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(IErrorDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> JsonMergePatch([FromRoute] string petName,
            [FromBody] JsonMergePatchDocument<PetEntity> patch)
        {
            try
            {
                var pet = _manager.ReadOne(petName).Result;
                patch.ApplyTo(pet, ModelState);
          
                if (!ModelState.IsValid)
                    return _invalidResponseFactory.Response(StatusCodes.Status412PreconditionFailed, "JsonPatchDocument", "Argumentos no válidos");

                await _manager.Update(pet);

                return Ok(pet);
            }
            catch (NotFoundException ex)
            {
                return _invalidResponseFactory.Response(NotFound(ex.Message));
            }
            catch (System.Exception e)
            {
                return _invalidResponseFactory.Response(StatusCode(StatusCodes.Status409Conflict, e.Message));
            }
        }

        /// <summary>
        ///     map
        /// </summary>
        /// <param name="pet"></param>
        /// <returns></returns>
        private static PetDto ToView(PetEntity pet)
        {
            return new PetDto {Id = pet.Id, Description = pet.Description, Name = pet.Name };
        }

        /// <summary>
        ///     map
        /// </summary>
        /// <param name="aPet"></param>
        /// <returns></returns>
        private static PetEntity Map(PetDto aPet)
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