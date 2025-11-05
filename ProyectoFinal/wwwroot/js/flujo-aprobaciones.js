(() => {
    const Aprob = {
        // Tabla principal de solicitudes en aprobación
        tabla: null,
        // Listado de clientes (si aplica)
        clientes: [],
        // Mapa rápido para mostrar cedula de cliente por ID
        clientesMap: {},

        // Inicializa la pantalla
        async init() {
            // Precargar mapa de clientes para que no aparezca el ID
            await this.cargarClientes();
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
                        render: (id, _t, row) => {
                            // forzar llave string para evitar desajustes número/cadena
                            const raw = (id ?? row?.idCliente ?? row?.IdCliente);
                            const key = raw != null ? String(raw) : '';
                            // si el backend ya manda un alias, usar como respaldo
                            const posibleCedula = row?.clienteIdentificacion || row?.identificacionCliente;
                            return this.clientesMap[key] || posibleCedula || (raw ?? '');
                        }
                    },
                    { data: 'monto', title: 'Monto' },
                    { data: 'estado', title: 'Estado' },
                    {
                        data: null,
                        orderable: false,
                        title: 'Acciones',
                        // Botones de acción para el gestor
                        render: (row) => `
              <button class="btn btn-sm btn-outline-primary me-1 mb-1 aprobar" data-id="${row.idSolicitud}">Aprobar</button>
              <button class="btn btn-sm btn-outline-danger me-1 mb-1 devolver" data-id="${row.idSolicitud}">Devolución</button>
              <a class="btn btn-sm btn-outline-secondary mb-1" href="/FlujoSolicitudes/Tracking?id=${row.idSolicitud}">Tracking</a>`
                    }
                ],
                responsive: true,
                processing: true,
                pageLength: 10
            });
        },

        // Eventos de los botones dentro de la tabla
        registrarEventos() {
            // Aprobar solicitud (azul - primary)
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
        },

        // Llena clientesMap con Id -> cédula
        async cargarClientes() {
            try {
                // Puede responder /Clientes/ObtenerClientes
                const r = await $.get('/Clientes/ObtenerClientes?_ts=' + Date.now());
                if (r && !r.esError && Array.isArray(r.data)) {
                    const map = {};
                    r.data.forEach(c => {
                        const id = c.idCliente ?? c.IdCliente ?? c.id ?? c.Id;
                        const ced = c.identificacion ?? c.Identificacion ?? '';
                        if (id != null) map[String(id)] = ced || ('Cliente #' + id);
                    });
                    this.clientesMap = map;
                } else {
                    this.clientesMap = {};
                }
            } catch {
                this.clientesMap = {};
            }
        }
    };

    // Inicia el módulo al cargar la página
    $(document).ready(() => Aprob.init());
})();