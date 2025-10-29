(() => {
    const Clientes = {
        $modal: null,
        bsModal: null,
        modalBodyEl: null,
        editGetUrl: '',
        editPostUrl: '',

        init() {
            this.cacheDom();
            this.inicializarTabla();   
            this.registrarEventos();   // engancha editar/eliminar
        },

        cacheDom() {
            this.$modal = $('#editModal');
            this.modalBodyEl = document.getElementById('editModalBody');
            if (this.$modal.length) {
                this.bsModal = new bootstrap.Modal(this.$modal[0]);
                this.editGetUrl = this.$modal.data('edit-get');   // /Clientes/EditModal
                this.editPostUrl = this.$modal.data('edit-post');  // /Clientes/EditModal
            }
        },

        inicializarTabla() {
           
            if (!$.fn.DataTable) return;
            const $t = $('#tablaClientes');
            if (!$t.length) return;

            
            $t.DataTable({
                responsive: true,
                autoWidth: false,
                language: { url: '//cdn.datatables.net/plug-ins/1.13.4/i18n/es-ES.json' }
            });
        },

        // ================== EVENTOS ==================
        registrarEventos() {
            // Delegación global para Editar y Eliminar
            document.addEventListener('click', (e) => this.onEditClick(e));
            document.addEventListener('click', (e) => this.onDeleteClick(e));
        },

        // ================== EDITAR (MODAL AJAX) ==================
        async onEditClick(e) {
            const btn = e.target.closest('.btn-edit');
            if (!btn) return;

            if (!this.bsModal) return;
            const id = btn.dataset.id;

            this.modalBodyEl.innerHTML = `<div class="text-center text-muted py-5">Cargando…</div>`;

            // Cargar parcial GET /Clientes/EditModal/{id}
            const res = await fetch(`${this.editGetUrl}/${id}`, { method: 'GET' });
            const html = await res.text();
            this.modalBodyEl.innerHTML = html;
            this.bsModal.show();

            // Enviar POST del formulario dentro del modal
            const form = this.modalBodyEl.querySelector('#editForm');
            if (!form) return;

            form.addEventListener('submit', async (ev) => {
                ev.preventDefault();
                const fd = new FormData(form); // incluye el antiforgery del parcial

                const postRes = await fetch(this.editPostUrl, {
                    method: 'POST',
                    body: fd
                });

                const ct = postRes.headers.get('content-type') || '';
                if (ct.includes('application/json')) {
                    const data = await postRes.json();
                    if (data.ok) {
                        this.bsModal.hide();
                        if (window.Swal) {
                            await Swal.fire({ icon: 'success', title: 'Listo', text: data.msg || 'Actualizado correctamente.' });
                        }
                        location.reload();
                    } else {
                        if (window.Swal) {
                            await Swal.fire({ icon: 'error', title: 'Ups…', text: data.msg || 'No se pudo actualizar.' });
                        }
                    }
                } else {
                    
                    this.modalBodyEl.innerHTML = await postRes.text();
                }
            }, { once: true });
        },

        // ================== ELIMINAR ==================
        async onDeleteClick(e) {
            const btn = e.target.closest('.btn-delete');
            if (!btn) return;

            const id = btn.dataset.id;
            const name = btn.dataset.name;

            let confirmed = true;
            if (window.Swal) {
                const r = await Swal.fire({
                    icon: 'warning',
                    title: '¿Eliminar cliente?',
                    html: `<div>Se eliminará <b>${name}</b>.</div>`,
                    showCancelButton: true,
                    confirmButtonText: 'Sí, eliminar',
                    cancelButtonText: 'Cancelar'
                });
                confirmed = r.isConfirmed;
            }

            if (confirmed) {
                const form = document.querySelector(`.delete-form-${id}`);
                if (form) form.submit();
            }
        }
    };

    // Exponer (opcional)
    window.Clientes = Clientes;

    // Auto-init cuando el DOM esté listo
    $(function () { Clientes.init(); });
})();
