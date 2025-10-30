(() => {
    const Solicitudes = {
        init() {
            this.bindCedulaSelect(); // nuevo
            this.bindCreate();
            this.bindDelete();
        },

        // ====== CÉDULA / CLIENTE ======
        bindCedulaSelect() {
            const cedulaSelect = document.getElementById('CedulaSelect');
            const idClienteInput = document.getElementById('IdCliente');
            const nombreInput = document.getElementById('NombreCliente');
            const apellidoInput = document.getElementById('ApellidoCliente');

            if (!cedulaSelect) return; // Solo aplica en Create

            cedulaSelect.addEventListener('change', () => {
                const opt = cedulaSelect.selectedOptions[0];
                if (!opt || !opt.value) {
                    idClienteInput.value = '';
                    nombreInput.value = '';
                    apellidoInput.value = '';
                    return;
                }

                // Cargar datos desde los atributos del <option>
                idClienteInput.value = opt.getAttribute('data-idcliente') || '';
                nombreInput.value = opt.getAttribute('data-nombre') || '';
                apellidoInput.value = opt.getAttribute('data-apellido') || '';
            });
        },

        // ====== CREATE ======
        bindCreate() {
            const form = document.getElementById('formCrearSolicitud');
            const btn = document.getElementById('btnGuardarSolicitud');
            if (!form || !btn) return; // no estamos en página de creación

            form.addEventListener('submit', (e) => e.preventDefault());
            btn.addEventListener('click', () => this.crearSolicitud(form));
        },

        crearSolicitud(form) {
            if (typeof $(form).valid === 'function' && !$(form).valid()) return;

            const fd = new FormData(form);
            const token = form.querySelector('input[name="__RequestVerificationToken"]')?.value || '';

            fetch(form.getAttribute('action'), {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': token,
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: fd
            })
                .then(async r => {
                    const ct = r.headers.get('content-type') || '';
                    if (!ct.includes('application/json')) {
                        // Si la acción devuelve HTML por redirect, redirigimos a Index
                        window.location.href = '/Solicitudes/Index';
                        return null;
                    }
                    return r.json();
                })
                .then(response => {
                    if (!response) return;

                    if (response && response.success) {
                        const cedulaSel = document.getElementById('CedulaSelect');
                        const cedulaTxt = cedulaSel?.selectedOptions[0]?.value || '';
                        const montoNumber = Number(document.getElementById('Monto')?.value || 0);

                        const mensaje = `
                            Se ha realizado una solicitud para el usuario con cédula:
                            <strong>${cedulaTxt}</strong>
                            por un monto de
                            <strong>₡${montoNumber.toLocaleString('es-CR')}</strong>.
                        `;

                        const msgEl = document.getElementById('mensajeSolicitudExitosa');
                        if (msgEl) msgEl.innerHTML = mensaje;
                        const modalEl = document.getElementById('modalSolicitudExitosa');
                        if (modalEl && window.bootstrap?.Modal) {
                            new bootstrap.Modal(modalEl).show();
                        }

                        form.reset();

                        const id = response.idSolicitud || response.IdSolicitud || response.data?.idSolicitud;
                        if (id && window.Swal) {
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
                        } else {
                            window.location.href = '/Solicitudes/Index';
                        }

                    } else {
                        const msg = (response && (response.message || response.error)) || 'Hubo un error, por favor intente de nuevo.';
                        if (window.Swal) {
                            Swal.fire({ title: 'Error', text: msg, icon: 'error' });
                        } else {
                            alert(msg);
                        }
                    }
                })
                .catch(() => {
                    if (window.Swal) {
                        Swal.fire({ title: 'Error', text: 'No se pudo procesar la solicitud.', icon: 'error' });
                    } else {
                        alert('No se pudo procesar la solicitud.');
                    }
                });
        },

        // ====== DELETE ======
        bindDelete() {
            const table = document.getElementById('tablaSolicitudes');
            if (!table) return;

            document.addEventListener('click', (ev) => {
                const btn = ev.target.closest('.btn-delete');
                if (!btn) return;

                const id = btn.getAttribute('data-id');
                const nombre = btn.getAttribute('data-name') || '-';
                const cedula = btn.getAttribute('data-cedula') || '-';
                const monto = Number(btn.getAttribute('data-monto') || 0);

                const form = document.querySelector('.delete-form-' + id);
                if (!form) return;

                const html = `
      <div class="text-start" style="line-height:1.6">
        <div>Se eliminará la <strong>solicitud #${id}</strong>.</div>
        <div>Cliente: <strong>${nombre}</strong> <span class="text-muted">(${cedula})</span></div>
        <div>Monto: <strong>₡${monto.toLocaleString('es-CR')}</strong></div>
      </div>
    `;

                const doSubmit = () => form.submit();

                if (window.Swal) {
                    Swal.fire({
                        icon: 'warning',
                        title: '¿Eliminar solicitud de crédito?',
                        html,
                        showCancelButton: true,
                        confirmButtonText: 'Sí, eliminar',
                        cancelButtonText: 'Cancelar',
                        reverseButtons: true,
                        focusCancel: true,
                        customClass: {
                            confirmButton: 'btn btn-primary',
                            cancelButton: 'btn btn-secondary'
                        },
                        buttonsStyling: false
                    }).then(res => { if (res.isConfirmed) doSubmit(); });
                } else {
                    if (confirm(`¿Eliminar la solicitud #${id} de ${nombre}?`)) doSubmit();
                }
            }, false);
        }

    };

    document.addEventListener('DOMContentLoaded', () => Solicitudes.init());
})();
