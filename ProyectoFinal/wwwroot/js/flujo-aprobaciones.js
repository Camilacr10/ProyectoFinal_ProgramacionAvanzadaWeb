(() => {
    const Aprob = {
        tabla: null,
        clientes: [],
        clientesMap: {},

        async init() {
            await this.cargarClientes(); // primero clientes
            this.inicializarTabla();
            this.registrarEventos();
        },

        async cargarClientes() {
            try {
                const r = await $.get('/Clientes/ObtenerClientes?_ts=' + Date.now());
                if (r.esError) {
                    this.clientes = [];
                    this.clientesMap = {};
                    return;
                }
                this.clientes = r.data || [];
                this.clientesMap = {};
                this.clientes.forEach(c => {
                    const id = c.idCliente ?? c.IdCliente ?? c.id ?? c.Id;
                    const nombre = c.nombreCompleto
                        || [c.nombre, c.apellido].filter(Boolean).join(' ')
                        || c.nombre || c.identificacion
                        || ('Cliente #' + id);
                    this.clientesMap[id] = nombre;
                });
            } catch {
                this.clientes = [];
                this.clientesMap = {};
            }
        },

        inicializarTabla() {
            this.tabla = $('#tablaAprob').DataTable({
                ajax: { url: '/FlujoSolicitudes/ObtenerAprobaciones', type: 'GET', dataSrc: 'data' },
                columns: [
                    { data: 'idSolicitud', title: 'ID' },
                    {
                        data: 'idCliente', title: 'Cliente',
                        render: (id) => this.clientesMap[id] || id || ''
                    },
                    { data: 'monto', title: 'Monto' },
                    { data: 'estado', title: 'Estado' },
                    {
                        data: null, orderable: false, title: 'Acciones', render: (row) => `
              <button class="btn btn-sm btn-success aprobar" data-id="${row.idSolicitud}">Aprobar</button>
              <button class="btn btn-sm btn-warning devolver" data-id="${row.idSolicitud}">Devolución</button>
              <a class="btn btn-sm btn-secondary" href="/FlujoSolicitudes/Tracking?id=${row.idSolicitud}">Tracking</a>`
                    }
                ],
                responsive: true, processing: true, pageLength: 10
            });
        },

        registrarEventos() {
            $('#tablaAprob').on('click', '.aprobar', (e) => this.cambiar($(e.currentTarget).data('id'), 'Aprobado'));
            $('#tablaAprob').on('click', '.devolver', (e) => this.cambiar($(e.currentTarget).data('id'), 'Devolucion'));
        },

        async cambiar(id, nuevoEstado) {
            const { value: comentario } = await Swal.fire({ title: 'Comentario (opcional)', input: 'text', showCancelButton: true });
            if (comentario === undefined) return;
            App.ajaxPost('/FlujoSolicitudes/CambiarEstado', { IdSolicitud: id, NuevoEstado: nuevoEstado, Comentario: comentario }, (r) => {
                Swal.fire('Éxito', r.mensaje || 'Estado actualizado', 'success');
                this.tabla.ajax.reload();
            });
        }
    };
    $(document).ready(() => Aprob.init());
})();