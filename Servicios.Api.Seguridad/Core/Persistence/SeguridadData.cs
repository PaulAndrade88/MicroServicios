using Microsoft.AspNetCore.Identity;
using Servicios.Api.Seguridad.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.Api.Seguridad.Core.Persistence
{
    public class SeguridadData
    {
        public static async Task InsertarUsuario(SeguridadContexto context, UserManager<Usuario> usuarioManager)
        {
            if (!usuarioManager.Users.Any())
            {
                var usuario = new Usuario
                {
                    Nombre = "Paul",
                    Apellido = "Andrade",
                    Direccion = "Maya 123",
                    UserName = "PAAG",
                    Email = "paag_23@hotmail.com"                    
                };

                await usuarioManager.CreateAsync(usuario, "Password123$");
            }
        }
    }
}
