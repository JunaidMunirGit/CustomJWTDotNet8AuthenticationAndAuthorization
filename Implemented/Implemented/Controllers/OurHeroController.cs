﻿using Implemented.Helpers;
using Implemented.Models;
using Implemented.Services;
using Microsoft.AspNetCore.Mvc;

namespace Implemented.Controllers
{

    [Route("api/[controller]")]
    [ApiController] // Controller level
    [Authorize]
    public class OurHeroController : ControllerBase
    {
        private readonly IOurHeroService _heroService;
        public OurHeroController(IOurHeroService heroService)
        {
            _heroService = heroService;
        }

        //[ApiController]  // Action method level
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool? isActive = null)
        {
            var heros = await _heroService.GetAllHeros(isActive);
            return Ok(heros);
        }

        [HttpGet("{id}")]
        //[Route("{id}")] // /api/OurHero/:id
        public async Task<IActionResult> Get(int id)
        {
            var hero = await _heroService.GetHerosByID(id);
            if (hero == null)
            {
                return NotFound();
            }
            return Ok(hero);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OurHero heroObject)
        {
            var hero = await _heroService.AddOurHero(heroObject);

            if (hero == null)
            {
                return BadRequest();
            }

            return Ok(new
            {
                message = "Super Hero Created Successfully!!!",
                id = hero!.Id
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] OurHero heroObject)
        {
            var hero = await _heroService.UpdateOurHero(id, heroObject);
            if (hero == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                message = "Super Hero Updated Successfully!!!",
                id = hero!.Id
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!await _heroService.DeleteHerosByID(id))
            {
                return NotFound();
            }

            return Ok(new
            {
                message = "Super Hero Deleted Successfully!!!",
                id = id
            });
        }
    }
}