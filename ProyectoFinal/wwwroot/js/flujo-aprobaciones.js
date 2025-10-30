(() => {
    const Aprob = {
        // Tabla principal de solicitudes en aprobación
        tabla: null,
        // Listado de clientes (si aplica)
        clientes: [],
        // Mapa rápido para mostrar nombre de cliente por ID
        clientesMap: {},

        // Inicializa la pantalla
        async init() {
            this.inicializarTabla();
            this.registrarEventos();
        },

        // Crea la tabla y carga los datos
        inicializarTabla() {
            if (!$.fn.DataTable) {
                console.error('DataTables no está cargado');
                return;
            }

            // Configuración de DataTable
            this.tabla = $('#tablaAprob').DataTable({
                ajax: {
                    url: '/FlujoSolicitudes/ObtenerAprobaciones',
                    type: 'GET',
                    dataSrc: 'data'
                },
                columns: [
                    { data: 'idSolicitud', title: 'ID' },
                    {
                        data: 'idCliente',
                        title: 'Cliente',
                        render: (id) => this.clientesMap[id] || id || ''
                    },
                    { data: 'monto', title: 'Monto' },
                    { data: 'estado', title: 'Estado' },
                    {
                        data: null,
                        orderable: false,
                        title: 'Acciones',
                        // Botones de acción para el gestor
                        render: (row) => `
              <button class="btn btn-sm btn-success aprobar" data-id="${row.idSolicitud}">Aprobar</button>
              <button class="btn btn-sm btn-warning devolver" data-id="${row.idSolicitud}">Devolución</button>
              <a class="btn btn-sm btn-secondary" href="/FlujoSolicitudes/Tracking?id=${row.idSolicitud}">Tracking</a>`
                    }
                ],
                responsive: true,
                processing: true,
                pageLength: 10
            });
        },

        // Eventos de los botones dentro de la tabla
        registrarEventos() {
            // Aprobar solicitud
            $('#tablaAprob').on('click', '.aprobar', (e) =>
                this.cambiar($(e.currentTarget).data('id'), 'Aprobado'));

            // Devolver solicitud
            $('#tablaAprob').on('click', '.devolver', (e) =>
                this.cambiar($(e.currentTarget).data('id'), 'Devolucion'));
        },

        // Cambia el estado de la solicitud
        async cambiar(id, nuevoEstado) {
            // Pide comentario antes de actualizar
            const { value: comentario } = await Swal.fire({
                title: 'Comentario (opcional)',
                input: 'text',
                showCancelButton: true
            });
            if (comentario === undefined) return;

            // Token antifalsificación
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';

            // Llamado AJAX al controlador
            $.ajax({
                url: '/FlujoSolicitudes/CambiarEstado',
                method: 'POST',
                headers: { 'RequestVerificationToken': token },
                data: {
                    IdSolicitud: id,
                    NuevoEstado: nuevoEstado,
                    Comentario: comentario
                },
                success: (r) => {
                    // Si hay error muestra alerta
                    if (r?.esError) {
                        Swal.fire('Error', r.mensaje || 'No se pudo actualizar', 'error');
                    } else {
                        // Muestra mensaje y recarga la tabla
                        Swal.fire('Éxito', r.mensaje || 'Estado actualizado', 'success');
                        this.tabla.ajax.reload();
                    }
                },
                error: () => Swal.fire('Error', 'Error de comunicación', 'error')
            });
        }
    };

    // Inicia el módulo al cargar la página
    $(document).ready(() => Aprob.init());
})();