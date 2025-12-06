// usuarios.js
$(document).ready(function () {
    var $modal = $('#crudModal');
    var $modalTitle = $('#crudTitle');
    var $modalBody = $('#crudBody');

    // ------------------------
    // Función para abrir modal
    // ------------------------
    function abrirModal(url, titulo) {
        $modalTitle.text(titulo);
        $modalBody.html('<div class="text-center text-muted py-5">Cargando…</div>');
        $modal.modal('show');

        $.get(url)
            .done(function (data) {
                $modalBody.html(data);
            })
            .fail(function () {
                $modalBody.html('<div class="alert alert-danger">Error al cargar el contenido</div>');
            });
    }

    // ------------------------
    // Crear usuario
    // ------------------------
    $('.btn-new').click(function () {
        abrirModal($modal.data('create-get'), 'Crear Usuario');
    });

    // ------------------------
    // Editar usuario
    // ------------------------
    $(document).on('click', '.btn-edit', function () {
        var id = $(this).data('id');
        abrirModal($modal.data('edit-get') + '?id=' + id, 'Editar Usuario');
    });

    // ------------------------
    // Eliminar usuario
    // ------------------------
    $(document).on('click', '.btn-delete', function () {
        var id = $(this).data('id');
        abrirModal($modal.data('delete-get') + '?id=' + id, 'Eliminar Usuario');
    });

    // ------------------------
    // Enviar formulario via AJAX
    // ------------------------
    $(document).on('submit', 'form', function (e) {
        e.preventDefault();
        var $form = $(this);
        var url = $form.attr('action');
        var method = $form.attr('method') || 'POST';
        var data = $form.serialize();

        $.ajax({
            url: url,
            method: method,
            data: data
        })
            .done(function (response) {
                if (response.success) {
                    $modal.modal('hide');
                    // Recargar la página para actualizar la tabla
                    location.reload();
                } else {
                    // Si devuelve HTML, reemplazamos el contenido del modal
                    $modalBody.html(response);
                }
            })
            .fail(function () {
                Swal.fire('Error', 'Ha ocurrido un error al procesar la solicitud', 'error');
            });
    });
});
