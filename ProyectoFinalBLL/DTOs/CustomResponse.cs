namespace ProyectoFinalBLL.DTOs
{
    public class CustomResponse<T>
    {
        public bool EsError { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public T? Data { get; set; }

        public CustomResponse()
        {
            EsError = false;
            Mensaje = "Acción realizada correctamente.";
        }

        // ✅ Métodos estáticos 
        public static CustomResponse<T> Ok(T? data = default, string msg = "Acción realizada correctamente.")
            => new CustomResponse<T> { EsError = false, Mensaje = msg, Data = data };

        public static CustomResponse<T> Fail(string msg)
            => new CustomResponse<T> { EsError = true, Mensaje = msg, Data = default };
    }
}
