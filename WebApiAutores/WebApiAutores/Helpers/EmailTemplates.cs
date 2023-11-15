namespace WebApiAutores.Helpers
{
    public static class EmailTemplates
    {
        public static string LoginTemplate(string email)
        {
            return $@"
            <h3>Inicio de sesion realizado correctamente.</h3>
            <p>Le informamos que se ha iniciado sesion en su cuenta</p>         
            <p>Fecha y hora de inicio de sesion: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}</p>
            ";
        }
    }
}
