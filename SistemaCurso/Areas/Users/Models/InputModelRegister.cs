using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaCurso.Areas.Users.Models
{
    public class InputModelRegister
    {
        //variables nombre
        [Required(ErrorMessage = "El campo nombre es obligatorio")]
        public string name { get; set; }

        //variables apellido
        [Required(ErrorMessage = "El campo apellidos es obligatorio")]
        public string lastNames { get; set; }

        //variables identificación
        [Required(ErrorMessage = "El campo identificación es obligatorio")]
        public string identification { get; set; }

        //variables telefono
        [Required(ErrorMessage = "El campo telefono es obligatorio.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{2})\)?[-. ]?([0-9]{2})[-. ]?([0-9]{4})$", ErrorMessage = "El formato del teléfono ingresado no es válido.")]
        public string phoneNumber { get; set; }

        //variables corrreo
        [Required(ErrorMessage = "El campo correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es una dirección de correo electrónico válida.")]
        public string email { get; set; }

        //variables contraseña
        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "El campo contraseña es obligatorio.")]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        public string password { get; set; }

        [Required(ErrorMessage = "Seleccione un tipo.")]
        public string role { get; set; }
        public byte[] Image { get;  set; }
        public string ID { get; set; }
        public int Id { get; set; }
        public IdentityUser IdentityUser { get;  set; }

    }
}
