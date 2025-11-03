(() => {
    const modalEl = document.getElementById('crudModal');
    if (!modalEl) return;

    const modal = new bootstrap.Modal(modalEl);
    const titleEl = document.getElementById('crudTitle');
    const bodyEl = document.getElementById('crudBody');

    const url = {
        createGet: modalEl.dataset.createGet,
        createPost: modalEl.dataset.createPost,
        editGet: modalEl.dataset.editGet,
        editPost: modalEl.dataset.editPost,
        deleteGet: modalEl.dataset.deleteGet,
        deletePost: modalEl.dataset.deletePost
    };


    const showLoading = (el) => el.innerHTML = '<div class="text-center text-muted py-5">Cargando…</div>';

    const openModalWith = async (getUrl, title) => {
        titleEl.textContent = title;
        showLoading(bodyEl);
        modal.show();

        try {
            const r = await fetch(getUrl, { method: 'GET' });
            if (!r.ok) {
                await Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar el formulario.' });
                modal.hide();
                return;
            }
            bodyEl.innerHTML = await r.text();
            wireForm();
        } catch {
            await Swal.fire({ icon: 'error', title: 'Error', text: 'Error de conexión al cargar el formulario.' });
            modal.hide();
        }
    };

    const parseJsonSafe = async (resp) => {
        const ct = resp.headers.get('content-type') || '';
        if (!ct.includes('application/json')) {
            return null;
        }
        try { return await resp.json(); } catch { return null; }
    };

    const wireForm = () => {
        const form = bodyEl.querySelector('form');
        if (!form) return;

        const submitBtn = form.querySelector('[type="submit"]');

        const postForm = async () => {
            const fd = new FormData(form);
            const action = form.getAttribute('action');

           
            const tokenInput = form.querySelector('input[name="__RequestVerificationToken"]');
            const token = tokenInput ? tokenInput.value : null;

            
            const restoreBtn = () => {
                if (submitBtn) {
                    submitBtn.disabled = false;
                    submitBtn.innerHTML = submitBtn.dataset.originalText || 'Guardar';
                }
            };
            if (submitBtn) {
                submitBtn.dataset.originalText = submitBtn.innerHTML;
                submitBtn.disabled = true;
                submitBtn.innerHTML = 'Guardando…';
            }

            try {
                const resp = await fetch(action, {
                    method: 'POST',
                    headers: token ? { 'RequestVerificationToken': token } : {},
                    body: fd
                });

                const data = await parseJsonSafe(resp); 
                if (!resp.ok) {
                    
                    const msg = (data && (data.mensaje || data.message)) || 'No se pudo procesar la solicitud.';
                    await Swal.fire({ icon: 'error', title: 'Error', text: msg });
                    restoreBtn();
                    return;
                }

                // 200 Ok
                if (data && data.esError) {
                    await Swal.fire({ icon: 'error', title: 'Error', text: data.mensaje || 'Error al procesar.' });
                    restoreBtn();
                    return;
                }

                await Swal.fire({
                    icon: 'success',
                    title: '¡Listo!',
                    text: (data && data.mensaje) || 'Operación exitosa.'
                });

                modal.hide();
                location.reload();
            } catch {
                await Swal.fire({ icon: 'error', title: 'Error', text: 'Error de conexión con el servidor.' });
                restoreBtn();
            }
        };

        form.addEventListener('submit', async (ev) => {
            ev.preventDefault();
            await postForm();
        });

     
        const isDelete = /Delete/i.test((titleEl.textContent || '').trim());
        if (isDelete && submitBtn) {
            form.removeEventListener('submit', () => { }); // nos aseguramos
            form.addEventListener('submit', async (ev) => {
                ev.preventDefault();
                const ok = await Swal.fire({
                    icon: 'warning',
                    title: 'Confirmar eliminación',
                    text: 'Esta acción no se puede deshacer.',
                    showCancelButton: true,
                    confirmButtonText: 'Eliminar',
                    cancelButtonText: 'Cancelar'
                });
                if (ok.isConfirmed) {
                    await postForm();
                }
            });
        }
    };

   
    document.addEventListener('click', (e) => {
        const btn = e.target.closest('.btn-new');
        if (!btn) return;
        openModalWith(url.createGet, 'Nuevo cliente');
    });

    // Editar
    document.addEventListener('click', (e) => {
        const btn = e.target.closest('.btn-edit');
        if (!btn) return;
        const id = btn.dataset.id;
        openModalWith(`${url.editGet}?id=${encodeURIComponent(id)}`, 'Editar cliente');
    });

    // Eliminar
    document.addEventListener('click', (e) => {
        const btn = e.target.closest('.btn-delete');
        if (!btn) return;
        const id = btn.dataset.id;
        openModalWith(`${url.deleteGet}?id=${encodeURIComponent(id)}`, 'Eliminar cliente');
    });
})();
