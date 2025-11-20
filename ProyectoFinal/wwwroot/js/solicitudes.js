(() => {
    const Solicitudes = {
        ultimoIdCreado: null,

        init() {
            this.bindCedulaSelect();
            this.bindCreate();
            this.bindDelete();
            this.bindModalActions();
            const modalCrear = document.getElementById('modalCrearSolicitud');
            if (modalCrear) {
                modalCrear.addEventListener('hidden.bs.modal', () => {
                    window.location.reload();
                });
            }
        },

        // Validar Cedula
        bindCedulaSelect() {
            const cedulaSelect = document.getElementById('CedulaSelect');
            const idClienteInput = document.getElementById('IdCliente');
            const nombreInput = document.getElementById('NombreCliente');
            const apellidoInput = document.getElementById('ApellidoCliente');

            if (!cedulaSelect) return;

            cedulaSelect.addEventListener('change', () => {
                const opt = cedulaSelect.selectedOptions[0];
                if (!opt || !opt.value) {
                    idClienteInput.value = '';
                    nombreInput.value = '';
                    apellidoInput.value = '';
                    return;
                }

                idClienteInput.value = opt.getAttribute('data-idcliente') || '';
                nombreInput.value = opt.getAttribute('data-nombre') || '';
                apellidoInput.value = opt.getAttribute('data-apellido') || '';
            });
        },

        // Create
        bindCreate() {
            const form = document.getElementById('formCrearSolicitud');
            const btn = document.getElementById('btnGuardarSolicitud');
            if (!form || !btn) return;

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
                        window.location.href = '/Solicitudes/Index';
                        return null;
                    }
                    return r.json();
                })
                .then(response => {
                    if (!response) return;

                    const modalEl = document.getElementById('modalSolicitudExitosa');
                    const msgEl = document.getElementById('mensajeSolicitudExitosa');
                    const titleEl = document.getElementById('tituloModalSolicitud');
                    const btnTrack = document.getElementById('btnVerTrackingModal');

                    if (!modalEl || !msgEl || !titleEl || !window.bootstrap?.Modal) {
                        const msg = (response && (response.message || response.error)) || 'Hubo un error, por favor intente de nuevo.';
                        alert(msg);
                        return;
                    }

                    const modal = new bootstrap.Modal(modalEl);

                    if (response.success) {
                        const cedulaSel = document.getElementById('CedulaSelect');
                        const cedulaTxt = cedulaSel?.selectedOptions[0]?.value || '';
                        const montoNumber = Number(document.getElementById('Monto')?.value || 0);

                        const mensaje = `
                            Se ha realizado una solicitud para el usuario con cédula:
                            <strong>${cedulaTxt}</strong>
                            por un monto de
                            <strong>₡${montoNumber.toLocaleString('es-CR')}</strong>.
                        `;

                        titleEl.textContent = 'Solicitud creada';
                        msgEl.innerHTML = mensaje;

                        const id = response.idSolicitud || response.IdSolicitud || response.data?.idSolicitud;
                        Solicitudes.ultimoIdCreado = id || null;

                        if (btnTrack) {
                            btnTrack.style.display = id ? 'inline-block' : 'none';
                        }

                        form.reset();
                        modal.show();
                    } else {
                        const msg = (response && (response.message || response.error)) || 'Hubo un error, por favor intente de nuevo.';

                        titleEl.textContent = 'No se pudo crear la solicitud';
                        msgEl.innerHTML = `<span class="text-danger">${msg}</span>`;
                        Solicitudes.ultimoIdCreado = null;

                        if (btnTrack) {
                            btnTrack.style.display = 'none';
                        }

                        modal.show();
                    }
                })
                .catch(() => {
                    const modalEl = document.getElementById('modalSolicitudExitosa');
                    const msgEl = document.getElementById('mensajeSolicitudExitosa');
                    const titleEl = document.getElementById('tituloModalSolicitud');

                    if (modalEl && msgEl && titleEl && window.bootstrap?.Modal) {
                        titleEl.textContent = 'Error de conexión';
                        msgEl.innerHTML = '<span class="text-danger">No se pudo procesar la solicitud.</span>';
                        const modal = new bootstrap.Modal(modalEl);
                        modal.show();
                    } else {
                        alert('No se pudo procesar la solicitud.');
                    }
                });
        },

        // Modal actions
        bindModalActions() {
            const btnListado = document.getElementById('btnVerListadoModal');
            const btnTracking = document.getElementById('btnVerTrackingModal');

            if (btnListado) {
                btnListado.addEventListener('click', () => {
                    window.location.href = '/Solicitudes/Index';
                });
            }

            if (btnTracking) {
                btnTracking.addEventListener('click', () => {
                    if (!Solicitudes.ultimoIdCreado) {
                        window.location.href = '/Solicitudes/Index';
                        return;
                    }
                    const id = encodeURIComponent(Solicitudes.ultimoIdCreado);
                    window.location.href = `/FlujoSolicitudes/Tracking?id=${id}`;
                });
            }
        },

        // DELETE
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