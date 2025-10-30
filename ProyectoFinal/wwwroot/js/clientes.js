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

    // Helpers
    const openModalWith = async (getUrl, title) => {
        titleEl.textContent = title;
        bodyEl.innerHTML = '<div class="text-center text-muted py-5">Cargando…</div>';
        modal.show();
        const r = await fetch(getUrl);
        bodyEl.innerHTML = await r.text();
        wireForm(); // engancha submit del form dentro del modal
    };

    const wireForm = () => {
        const form = bodyEl.querySelector('form');
        if (!form) return;

        form.addEventListener('submit', async (ev) => {
            ev.preventDefault();

            const action = form.getAttribute('action');
            const fd = new FormData(form);

            // antiforgery
            const tokenInput = form.querySelector('input[name="__RequestVerificationToken"]');
            const token = tokenInput ? tokenInput.value : null;

            const resp = await fetch(action, {
                method: 'POST',
                headers: token ? { 'RequestVerificationToken': token } : {},
                body: fd
            });

            const ct = resp.headers.get('content-type') || '';
            if (!ct.includes('application/json')) {
                // si por algún motivo vuelve html (no debería con nuestros controllers)
                bodyEl.innerHTML = await resp.text();
                wireForm();
                return;
            }

            const r = await resp.json(); // {esError, mensaje, data}
            if (!r.esError) {
                Swal.fire({ icon: 'success', title: '¡Listo!', text: r.mensaje || 'Operación exitosa.' });
                modal.hide();
                location.reload(); // refresca la tabla
            } else {
                Swal.fire({ icon: 'error', title: 'Oops', text: r.mensaje || 'Error al procesar.' });
            }
        });
    };

   
    document.addEventListener('click', (e) => {
        const btn = e.target.closest('.btn-new');
        if (!btn) return;
        openModalWith(url.createGet, 'Nuevo cliente');
    });

    // Botón EDITAR
    document.addEventListener('click', (e) => {
        const btn = e.target.closest('.btn-edit');
        if (!btn) return;
        const id = btn.dataset.id;
        openModalWith(`${url.editGet}?id=${encodeURIComponent(id)}`, 'Editar cliente');
    });

    // Botón ELIMINAR
    document.addEventListener('click', (e) => {
        const btn = e.target.closest('.btn-delete');
        if (!btn) return;
        const id = btn.dataset.id;
        openModalWith(`${url.deleteGet}?id=${encodeURIComponent(id)}`, 'Eliminar cliente');
    });
})();
