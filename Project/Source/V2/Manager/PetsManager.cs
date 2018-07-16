﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api213.V2.Dal;
using Api213.V2.Exception;
using Api213.V2.Helper;
using Api213.V2.Interface;
using Api213.V2.Models;

namespace Api213.V2.Manager
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class PetsManager : IPetsManager
    {
        private readonly IGenericRepository<PetEntity> _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PetsManager"/> class.
        /// 
        /// </summary>
        /// <param name="context"></param>
        public PetsManager(IGenericRepository<PetEntity> context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<PetEntity>> ReadAll()
        {
            var item = _context.Get();
            return Task.FromResult(item.AsEnumerable());
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Task<PetEntity> ReadOne(string name)
        {
            var petInput = _context.Get(c => c.Name == name).FirstOrDefault();
            if (petInput == null)
                throw new NotFoundException(name + " NotFound");
            return Task.FromResult(petInput);
        }

        /// <summary>
        /// </summary>
        /// <param name="pet"></param>
        /// <returns></returns>
        public Task<PetEntity> Create(PetEntity pet)
        {
            var newpet = new PetEntity { Id = pet.Id, Description = pet.Description, Name = pet.Name };
            if (IsExists(pet.Name).Result)
                throw new DuplicateWaitObjectException(pet.Name);
            _context.Insert(newpet);
               _context.Save();

            return Task.FromResult(newpet);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="pet"></param>
        /// <returns></returns>
        public Task<PetEntity> Replace(PetEntity pet)
        {
            if (IsExists(pet.Name).Result)
            {
                Delete(pet.Name);
                var newpet = Create(pet);
                _context.Save();
                return Task.FromResult(newpet.Result);
            }

            throw new NotFoundException(pet.Name + " NotFound");
        }

        /// <inheritdoc />
        public Task<PetEntity> Update(PetEntity pet)
        {
            if (IsExists(pet.Name).Result)
            {
                _context.Update(pet);
                _context.Save();
                return Task.FromResult(pet);
            }

            throw new NotFoundException(pet.Name + " NotFound");
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<PetEntity> Delete(string name)
        {
            if (IsExists(name).Result)
            {
                var pet = _context.Get(x => x.Name == name).First();
                _context.Delete(pet);
                _context.Save();
                return Task.FromResult(pet);
            }

            throw new NotFoundException(name + " NotFound");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filteringSortingParams"></param>
        /// <returns></returns>
        public Task<IEnumerable<dynamic>> Get(FilteringSortingParams filteringSortingParams)
        {
            var item = _context.Get(filteringSortingParams);
            return Task.FromResult(item.AsQueryable().AsEnumerable());
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="namelike"></param>
        /// <param name="filteringSortingParams"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetByNameSubstring(string namelike, FilteringSortingParams filteringSortingParams)
        {
            var queryable = _context.Includes(filteringSortingParams.Embed);

            if (namelike != null)
                queryable = queryable.Where(x => x.Name.Contains(namelike));

            return _context.SortAndFieldsAndFilterList(queryable, filteringSortingParams);
        }
        
        /// <summary>
        /// </summary>
        /// <param name="namelike"></param>
        /// <returns></returns>
        public IQueryable<PetEntity> GetByNameSubstring(string namelike)
        {
            if (namelike == null)
                namelike = string.Empty;

            return _context.Get(x => x.Name.Contains(namelike));
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<bool> IsExists(string name)
        {
            var petexist = _context.Get(x => x.Name == name).Any();
            return Task.FromResult(petexist);
        }
    }
}