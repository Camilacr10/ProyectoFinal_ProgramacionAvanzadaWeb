(() => {
    const Rep = {
        // Tabla principal del reporte
        tabla: null,
        // Mapa de usuarios para mostrar usuario en vez del ID
        usuariosMap: {},

        // Inicializa la pantalla
        init() {
            this.inicializarTabla();
            this.registrarEventos();
            this.cargarUsuarios().then(() => {
                if (this.tabla) this.tabla.rows().invalidate().draw(false);
            }).catch(() => { });
        },

        // Carga los usuarios para mostrar el nombre en el reporte
        async cargarUsuarios() {
            try {
                // Puede responder /Usuarios/ObtenerUsuarios
                const r = await $.get('/Usuarios/ObtenerUsuarios?_ts=' + Date.now());
                if (r && !r.esError && Array.isArray(r.data)) {
                    const map = {};
                    r.data.forEach(u => {
                        const id = u.id ?? u.Id;
                        const label = (u.userName ?? u.UserName ?? '') || (u.email ?? u.Email ?? '') || ('Usuario #' + id);
                        if (id != null) map[String(id)] = label; // forzar llave string
                    });
                    this.usuariosMap = map;
                } else {
                    this.usuariosMap = {};
                }
            } catch {
                this.usuariosMap = {};
            }
        },

        // Crea la tabla del reporte
        inicializarTabla() {
            if (!$.fn.DataTable) {
                console.error('DataTables no está cargado');
                return;
            }

            this.tabla = $('#tablaTracking').DataTable({
                data: [],
                columns: [
                    { data: 'idSolicitud', title: 'Solicitud' },
                    { data: 'accion', title: 'Acción' },
                    { data: 'comentario', title: 'Comentario' },
                    { data: 'estado', title: 'Estado' },
                    {
                        data: 'usuarioId',
                        title: 'Usuario',
                        render: (id) => {
                            const key = id != null ? String(id) : '';
                            return this.usuariosMap[key] || id || '';
                        }
                    },
                    {
                        data: 'fecha',
                        title: 'Fecha',
                        render: v => v ? new Date(v).toLocaleString() : ''
                    }
                ],
                responsive: true,
                pageLength: 10
            });
        },

        // Eventos del formulario de consulta
        registrarEventos() {
            $('#btnConsultar').on('click', () => this.cargar());
        },

        // Carga los movimientos del tracking
        cargar() {
            const id = $('#gestionId').val();
            if (!id) return;

            $.get(`/FlujoSolicitudes/ObtenerTracking?id=${id}`, (r) => {
                if (r.esError) {
                    return Swal.fire('Error', r.mensaje || 'No fue posible obtener el tracking', 'error');
                }
                this.tabla.clear().rows.add(r.data || []).draw();
            }).fail(() => Swal.fire('Error', 'Error de comunicación', 'error'));
        }
    };

    // Inicia el módulo al cargar la página
    $(document).ready(() => Rep.init());
})();