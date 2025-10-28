(() => {
    const Rep = {
        tabla: null,
        usuariosMap: {},   // id -> etiqueta visible (userName/email)

        init() {
            // 1) Inicializa la tabla y eventos de inmediato (no bloquea)
            this.inicializarTabla();
            this.registrarEventos();

            // 2) Cargar usuarios en background (si falla, no rompe nada)
            this.cargarUsuarios()
                .then(() => {
                    // Si ya hay datos en la tabla, re-dibujamos para que
                    // la columna Usuario muestre el label en vez del id.
                    if (this.tabla) this.tabla.rows().invalidate().draw(false);
                })
                .catch(() => { /* ignorar, mostramos el id */ });
        },

        async cargarUsuarios() {
            try {
                const r = await $.get('/Usuarios/ObtenerUsuarios?_ts=' + Date.now());
                if (r && !r.esError && Array.isArray(r.data)) {
                    const map = {};
                    r.data.forEach(u => {
                        const id = u.id ?? u.Id;
                        const label = u.userName || u.email || ('Usuario #' + id);
                        if (id) map[id] = label;
                    });
                    this.usuariosMap = map;
                } else {
                    this.usuariosMap = {};
                }
            } catch {
                this.usuariosMap = {};
            }
        },

        inicializarTabla() {
            this.tabla = $('#tablaTracking').DataTable({
                data: [],
                columns: [
                    { data: 'idSolicitud', title: 'Solicitud' },
                    { data: 'accion', title: 'Acción' },
                    { data: 'comentario', title: 'Comentario' },
                    { data: 'estado', title: 'Estado' },
                    {
                        data: 'usuarioId', title: 'Usuario',
                        // Si tenemos el mapa, mostramos userName/email; si no, dejamos el id
                        render: (id) => this.usuariosMap[id] || id || ''
                    },
                    { data: 'fecha', title: 'Fecha', render: v => v ? new Date(v).toLocaleString() : '' }
                ],
                responsive: true, pageLength: 10
            });
        },

        registrarEventos() {
            $('#btnConsultar').on('click', () => this.cargar());
        },

        cargar() {
            const id = $('#gestionId').val();
            if (!id) return;
            $.get(`/FlujoSolicitudes/ObtenerTracking?id=${id}`, (r) => {
                if (r.esError) {
                    return Swal.fire('Error', r.mensaje || 'No fue posible obtener el tracking', 'error');
                }
                this.tabla.clear().rows.add(r.data || []).draw();
            }).fail(() => {
                Swal.fire('Error', 'Error de comunicación', 'error');
            });
        }
    };

    $(document).ready(() => Rep.init());
})();