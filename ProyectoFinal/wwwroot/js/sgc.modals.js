// Requiere bootstrap y fetch
async function openModal(url, title) {
    const modalContent = document.getElementById("modalContent");
    const modalTitle = document.getElementById("mainModalTitle");

    modalTitle.innerText = title;
    modalContent.innerHTML = "<div class='text-center p-3'>Cargando...</div>";

    const res = await fetch(url, { credentials: "same-origin" });

    if (res.redirected) {
        window.location.href = res.url;
        return;
    }

    const html = await res.text();
    modalContent.innerHTML = html;

    const modalEl = document.getElementById('mainModal');
    const bsModal = new bootstrap.Modal(modalEl);
    bsModal.show();

    attachFormHandler();
}

function attachFormHandler() {
    const form = document.querySelector("#modalContent form");
    if (!form) return;

    // Remove any previous handler
    form.addEventListener("submit", onModalFormSubmit);
}

async function onModalFormSubmit(e) {
    e.preventDefault();
    const form = e.target;
    const submitBtn = form.querySelector("button[type='submit']");
    if (submitBtn) {
        submitBtn.disabled = true;
        const orig = submitBtn.innerHTML;
        submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Procesando...';
    }

    const data = new FormData(form);

    const res = await fetch(form.action, {
        method: form.method || "post",
        body: data,
        credentials: "same-origin"
    });

    const ct = res.headers.get("content-type") || "";
    if (ct.includes("application/json")) {
        const json = await res.json();
        if (json.success) {
            // cerrar modal y recargar (o podrías actualizar solo la tabla)
            const bsModal = bootstrap.Modal.getInstance(document.getElementById('mainModal'));
            if (bsModal) bsModal.hide();
            location.reload();
            return;
        } else {
            // mostrar mensaje de error simple
            alert(json.message || "Error");
        }
    } else {
        // devolvió HTML (form con validaciones) -> reponer dentro del modal
        const html = await res.text();
        document.getElementById("modalContent").innerHTML = html;
        attachFormHandler();
    }

    if (submitBtn) {
        submitBtn.disabled = false;
        submitBtn.innerHTML = orig;
    }
}
