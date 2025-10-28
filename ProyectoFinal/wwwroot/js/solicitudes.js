// Aislar funciones en módulo autoejecutable
(() => {
    const Solicitudes = {
        init() {
            this.registrarEventos();
        },

        registrarEventos() {
            // Click en Crear -> AJAX
            $('#btnGuardarSolicitud').on('click', () => {
                this.CrearSolicitud();
            });

            // Evitar submit accidental por Enter (opcional)
            $('#formCrearSolicitud').on('submit', (e) => e.preventDefault());
        },

        CrearSolicitud() {
            const form = $('#formCrearSolicitud');

            // Si tienes jQuery Validate en la vista:
            if (typeof form.valid === 'function' && !form.valid()) return;

            const fd = new FormData(form[0]);
            const token = $('input[name="__RequestVerificationToken"]').val();

            $.ajax({
                url: form.attr('action'),
                type: 'POST',
                dataType: 'json',
                headers: { 'RequestVerificationToken': token },
                processData: false,
                contentType: false,
                data: fd,
                success: function (response) {
                    if (response && response.success) {
                        // Datos del form
                        const identificacion = $('#IdCliente').val();
                        const montoNumber = Number($('#Monto').val() || 0);

                        const mensaje = `
                            Se ha realizado una solicitud para el usuario con identificación:
                            <strong>${identificacion}</strong>
                            por un monto de
                            <strong>₡${montoNumber.toLocaleString('es-CR')}</strong>.
                        `;

                        $('#mensajeSolicitudExitosa').html(mensaje);

                        // Mostrar modal Bootstrap
                        new bootstrap.Modal(document.getElementById('modalSolicitudExitosa')).show();

                        // Limpiar formulario
                        form[0].reset();

                        // ====== Ir al tracking ======
                        const id = response.idSolicitud || response.IdSolicitud || response.data?.idSolicitud;
                        if (id) {
                            Swal.fire({
                                title: 'Solicitud creada',
                                html: mensaje + '<br><br>¿Deseas ver el tracking?',
                                icon: 'success',
                                showCancelButton: true,
                                confirmButtonText: 'Ver tracking',
                                cancelButtonText: 'Cerrar'
                            }).then(r => {
                                if (r.isConfirmed) {
                                    window.location.href = `/FlujoSolicitudes/Tracking?id=${encodeURIComponent(id)}`;
                                }
                            });
                        }

                    } else {
                        Swal.fire({
                            title: 'Error',
                            text: (response && response.message) || 'Hubo un error, por favor intente de nuevo.',
                            icon: 'error'
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        title: 'Error',
                        text: 'No se pudo procesar la solicitud.',
                        icon: 'error'
                    });
                }
            });
        }
    };

    $(document).ready(() => Solicitudes.init());
})();
