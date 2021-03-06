﻿using gpro_web.Helpers;
using gpro_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace gpro_web.Services
{    
    public interface IClienteService
{
    Cliente BuscarPorCuit(Int64 cuit);
    List<Cliente> BuscarClientes(String Dato);
    List<Cliente> GetAll();
    void NuevoCliente(Cliente cliente);
    Task UpdateClienteAsync(Cliente cliente);
}

public class ClienteService : IClienteService
{
    private gpro_dbContext _context;

    public ClienteService(gpro_dbContext context)
        {
            _context = context;
        }

    public List<Cliente> GetAll()
    {
        var clientes = from b in _context.Cliente
                        select b;

        return clientes.ToList();
    }

    public List<Cliente> BuscarClientes(string Dato)
    {
        var clientes = from b in _context.Cliente
                       where b.ApellidoCliente.Contains(Dato) || b.NombreCliente.Contains(Dato) || b.RazonSocialCliente.Contains(Dato)
                       select b;
        if (clientes.ToList().Count() == 0)
        {
            return null;
        }

        return clientes.ToList();
    }

    public Cliente BuscarPorCuit(Int64 cuit)
    {
        var cliente = from b in _context.Cliente
                      where b.IdCliente.Equals(cuit)
                      select b;

        if (cliente.ToList().Count() == 0)
        {
            return null;
        }
        return cliente.ToList().ElementAt(0);
    }

    public void NuevoCliente(Cliente cliente)
    {
            if (_context.Cliente.Any(x => x.IdCliente == cliente.IdCliente))
                throw new AppException("El cliente con CUIT " + cliente.IdCliente + " ya existe."); 
            _context.Cliente.Add(cliente);  
            _context.SaveChanges();
    }

    public async Task UpdateClienteAsync(Cliente cliente)
    {

            if (_context.Cliente.Any(x => (x.IdCliente == cliente.IdCliente) && (x.Id != cliente.Id)))

            {
                if (_context.Cliente.Any(x => x.IdCliente == cliente.IdCliente))
                   throw new AppException("El cliente con CUIT " + cliente.IdCliente + " ya existe.");

            }

            _context.Cliente.Update(cliente);
            await _context.SaveChangesAsync();

    }
}
}