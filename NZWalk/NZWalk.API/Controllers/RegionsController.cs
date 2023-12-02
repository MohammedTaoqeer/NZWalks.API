using Microsoft.AspNetCore.Http;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalk.API.Models.Domain;
using NZWalk.API.Models.DTO;
using NZWalks.API.Data;
using System.Data.Common;
using System.Reflection.Metadata.Ecma335;
using NZWalk.API.Repositories;
using NZWalk.API.CustomActionFiler;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository,
            IMapper mapper,
            ILogger<RegionsController> logger)

        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }
        [HttpGet]
      // [Authorize(Roles ="Reader")]
        public async Task<IActionResult> GetAll()
        {
                
                // Get Data From DataBase - Domain Models
                var regions = await regionRepository.GetAllAsync();

                logger.LogInformation($"Finished GetAllRegions request with data: {JsonSerializer.Serialize(regions)}");

              
                // Return DTOs
                return Ok(mapper.Map<List<RegionDTO>>(regions));
           
        }
        [HttpGet]
        [Route("{id:Guid}")]
      // [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
           

            // Get Data From DataBase - Domain Models

            var regions = await regionRepository.GetByIdAsync(id);



            if (regions == null)
            {
                return NotFound();

            }

            //Map/Convert Region Domain Model to Region DTO

            //var regionDTO = new RegionDTO
            //{
            //     Id=regions.Id,
            //     Code=regions.Code,
            //     Name=regions.Name,
            //     RegionImageUrl=regions.RegionImageUrl 

            //};

            //Region DTO back to Client
            return Ok(mapper.Map<RegionDTO>(regions));
        }
        //Post to create new Region
        [HttpPost]
        [ValidateModel]
     //   [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDTO addRegionRequestDTO)
        {
           
                // Map or Convert DTO to Domain Model
                var regions = mapper.Map<Region>(addRegionRequestDTO);
                // Use Domain Model to Create Region
                regions = await regionRepository.CreateAsync(regions);
                //Map Domain model to DTO
                var regionDTO = mapper.Map<RegionDTO>(regions);
                return CreatedAtAction(nameof(GetById), new { id = regionDTO.Id }, regionDTO);
           
        }
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
      //  [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDTO updateRegionRequestDTO)
        {
            
                //Map Dto to Domain Model
                //var regions = new Region
                //{
                //    Code = updateRegionRequestDTO.Code,
                //    Name = updateRegionRequestDTO.Name,
                //    RegionImageUrl = updateRegionRequestDTO.RegionImageUrl
                //};
                //Checks if region exists

                var regions = mapper.Map<Region>(updateRegionRequestDTO);

                regions = await regionRepository.UpdateAsync(id, regions);
                if (regions == null)
                {
                    return NotFound();
                }



                //Convert domian model to dto

                //var regionDTO = new RegionDTO
                //{
                //    Id = regions.Id,
                //    Code = regions.Code,
                //    Name = regions.Name,
                //    RegionImageUrl = regions.RegionImageUrl
                //};

                var regionDTO = mapper.Map<RegionDTO>(regions);
                return Ok(regionDTO);
          
        }

        [HttpDelete]
        [Route("{id:Guid}")]
     //   [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            //var regions=  await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
            var regions = await regionRepository.DeleteAsync(id);
            if (regions == null)
            {
                return NotFound();
            }

            //return deleted region back
            //app domain model to dto

            var regionDTO = mapper.Map<RegionDTO>(regions);
            return Ok(regionDTO);
        }
    }
}



