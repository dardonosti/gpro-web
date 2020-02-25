﻿using AutoMapper;
using gpro_web.Dtos;
using gpro_web.Helpers;
using gpro_web.Models;
using gpro_web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace gpro_web.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class EmpleadoController : ControllerBase
    {
        private IEmpleadoService _empleadoService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly gpro_dbContext _context;

        public EmpleadoController(
            IEmpleadoService empleadoService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            gpro_dbContext context)
        {
            _empleadoService = empleadoService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _context = context;
        }

        // [Authorize(Roles = "Admin, PM")]
        [HttpGet("")]
        public IActionResult AllEmpleados()
        {
            var empleados = _empleadoService.AllEmpleados();
            var empleadosDtos = _mapper.Map<IList<EmpleadoDto>>(empleados);

            if (empleados == null)
                return NotFound();
            {
            }

            return Ok(empleadosDtos);
        }

        // [Authorize(Roles = "Admin, PM")]
        [HttpGet("empleados/{dato}")]
        public IActionResult BuscarEmpleado(string dato)
        {
            var empleado = _empleadoService.BuscarEmpleados(dato);
            var empleadoDtos = _mapper.Map<IList<EmpleadoDto>>(empleado);

            if (empleado == null)
            {
                return NotFound();
            }

            return Ok(empleadoDtos);
        }

        // [Authorize(Roles = "Admin, PM")]
        [HttpGet("documento/{dni}")]
        public IActionResult BuscarPorDni(int dni)
        {
            var empleado = _empleadoService.BuscarPorDni(dni);
            var empleadoDtos = _mapper.Map<EmpleadoDto>(empleado);

            if (empleado == null)
            {
                return NotFound();
            }

            return Ok(empleadoDtos);
        }

        [Authorize(Roles = "Admin, PM")]
        [HttpPost("new")]
        public IActionResult NuevoEmpleado([FromBody]EmpleadoDto empleadoDto)
        {
            var empleado = _mapper.Map<Empleado>(empleadoDto);

            try
            {
                _empleadoService.NuevoEmpleado(empleado);
                return Ok(empleado);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [Authorize(Roles = "Admin,PM")]
        [HttpPut("update")]
        public ActionResult UpdateEmpleado([FromBody]EmpleadoDto empleadoDtos)
        {

            var empleado = _mapper.Map<Empleado>(empleadoDtos);

            try
            {
                _empleadoService.UpdateEmpleado(empleado);
                return Ok(empleado);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("proyectos/{idEmpleado}")]
        public ActionResult GetProyectos(int idEmpleado)
        {
            var proyectos = from e in _context.EmpleadoProyecto
                            where e.IdEmpleado.Equals(idEmpleado)
                            select e;

            var proyectosDto = _mapper.Map<IList<EmpleadoProyectoDto>>(proyectos.ToList());
            return Ok(proyectosDto);

        }
    }
}