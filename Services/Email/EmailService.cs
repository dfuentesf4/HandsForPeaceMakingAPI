using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace HandsForPeaceMakingAPI.Services.Email
{
    public class EmailService
    {
        private readonly SMPTConfig _smptConfig;
        private readonly IWebHostEnvironment _environment;

        public EmailService(SMPTConfig smptConfig, IWebHostEnvironment environment)
        {
            _smptConfig = smptConfig;
            _environment = environment;
        }

        public async Task EnviarCorreo(string token, string correo, string Nombres, string user)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("SIMPOSIO UMG", _smptConfig.Email));
            email.To.Add(new MailboxAddress(Nombres, correo));
            email.Subject = "Restablecimiento de contraseña HFPMApp";

            var htmlBody = $@"
                            <html>
                            <body>
                                <h1>HOLA {Nombres} </h1>
                                <p>Hemos recibido una solicitud para restablecer tu contraseña</p>
                                <p>El usuario {user} fue el que solicito el restablecimiento </p>
                                <ul>
                                    <li>Nombres: {Nombres}</li>
                                    <li>Usuario: {user}</li>
                                    <li>Token: {token}</li>
                                </ul>
                                <p>Para restablecer tu contraseña, por favor Ingrese a la aplicacion HFPMApp dirijase al apartado de olvide mi contraseña
                                    Luego dirijase a la opcionde restablecer contraseña, donde se le pedira el usuario, que ingrese el token y que ingrese una nueva contraseña
                                    despues de llenar estos campos puede enviar el formulario y si su solicitud es procesada con exito recibira un correo 
                                    de afirmacion donde se le informara que su conraseña fue restablecida correctamente.</p>
                            </body>
                            </html>";

            email.Body = new TextPart("html") { Text = htmlBody };

            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(_smptConfig.Email, _smptConfig.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
        }
    }
}
