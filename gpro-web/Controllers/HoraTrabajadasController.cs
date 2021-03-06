﻿using System;
using gpro_web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gpro_web.Models;
using gpro_web.Dtos;
using AutoMapper;
using gpro_web.Helpers;
using Microsoft.Extensions.Options;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace gpro_web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HoraTrabajadasController : ControllerBase
    {
        private IHoraTrabajadaService _horaTrabajadaService;
        private readonly gpro_dbContext _context;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public HoraTrabajadasController(gpro_dbContext context, IMapper mapper,
            IOptions<AppSettings> appSettings, IHoraTrabajadaService horaTrabajadaService)
        {
            _horaTrabajadaService = horaTrabajadaService;
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;

        }

        // GET: HoraTrabajadas
        [Authorize(Roles = "Admin, PM, Member")]
        [HttpGet]
        public IActionResult GetHoraTrabajadaProy()
        {
            var datos = _horaTrabajadaService.HorasPorProyecto();
            
            return Ok(datos);
        }
        [Authorize(Roles = "Admin, PM, Member")]
        [HttpGet("porFecha/{id}/{inicio}/{fin}")]
        public IActionResult GetHoraTrabajadaRec([FromRoute] int id, String inicio, String fin)
        {
            var datos = _horaTrabajadaService.HorasPorRecurso(
                id,
                DateTime.Parse(inicio, null, DateTimeStyles.RoundtripKind),
                DateTime.Parse(fin, null, DateTimeStyles.RoundtripKind)
                );

            return Ok(datos);
        }

        [Authorize(Roles = "Admin, PM, Member")]
        [HttpGet("overbudget/{idProyecto}/{inicio}/{fin}")]
        public IActionResult GetHorasOverBudget([FromRoute] int idProyecto, String inicio, String fin)
        {
            var datos = _horaTrabajadaService.HorasOverBudget(
                idProyecto,
                DateTime.Parse(inicio, null, DateTimeStyles.RoundtripKind),
                DateTime.Parse(fin, null, DateTimeStyles.RoundtripKind)
                );

            return Ok(datos);
        }

        // GET: HoraTrabajadas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHoraTrabajada([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var horaTrabajada = _mapper.Map<HoraTrabajadasDto>(await _context.HoraTrabajada.FindAsync(id));

            if (horaTrabajada == null)
            {
                return NotFound();
            }

            return Ok(horaTrabajada);
        }

        // PUT: HoraTrabajadas/5
        [Authorize(Roles = "Admin, PM")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHoraTrabajada([FromRoute] int id, [FromBody] HoraTrabajada horaTrabajada)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != horaTrabajada.ProyectoIdProyecto)
            {
                return BadRequest();
            }

            _context.Entry(horaTrabajada).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HoraTrabajadaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: HoraTrabajadas/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHoraTrabajada([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var horaTrabajada = await _context.HoraTrabajada.FindAsync(id);
            if (horaTrabajada == null)
            {
                return NotFound();
            }

            _context.HoraTrabajada.Remove(horaTrabajada);
            await _context.SaveChangesAsync();

            return Ok(horaTrabajada);
        }

        private bool HoraTrabajadaExists(int id)
        {
            return _context.HoraTrabajada.Any(e => e.ProyectoIdProyecto == id);
        }

        [Authorize(Roles = "Admin, PM, Member")]
        [HttpGet("porProy/{id}")]
        public IActionResult HorasAdeudPorProy([FromRoute] int id)
        {
            return Ok(_horaTrabajadaService.HorasByProyecto(id));
        }

        [Authorize(Roles = "Admin, PM, Member")]
        [HttpGet("empleado/{idPerfil}/{idProyecto}")]
        public IActionResult HorasPorEmpleado([FromRoute] int idPerfil, int idProyecto)
        {
            return Ok(_horaTrabajadaService.HorasByEmpleado(idPerfil, idProyecto));
        }

        [Authorize(Roles = "Admin, PM, Member")]
        [HttpPost]
        public async Task<IActionResult> PostHoraTrabajada([FromBody] HoraTrabajadasDto horaTrabajada)
        {
            try
            {
                var horas = _mapper.Map<HoraTrabajada>(horaTrabajada);
                await _horaTrabajadaService.CargaHorasEmpl(horas);
                return Ok(horas);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            
        }
    }
}