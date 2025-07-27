using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Dtos.In
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }
}
