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

                // Activar validaciones ASP.NET MVC
                $.validator.unobtrusive.parse($modalBody.find('form'));
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
    // Enviar formulario vía AJAX
    // ------------------------
    $(document).on('submit', '#crudBody form', function (e) {
        e.preventDefault();
        var $form = $(this);

        var url = $form.attr('action');
        var method = $form.attr('method') || 'POST';
        var data = $form.serialize();

        // Botón correcto independientemente del modal recargado
        var $submitBtn = $form.find('button[type="submit"]');
        $submitBtn.prop('disabled', true).text('Guardando...');

        $.ajax({
            url: url,
            method: method,
            data: data
        })
            .done(function (response) {
                if (response.success) {
                    $modal.modal('hide');
                    location.reload();
                } else {
                    // Reemplazar HTML con errores
                    $modalBody.html(response);

                    // Importante: volver a activar validaciones
                    $.validator.unobtrusive.parse($modalBody.find('form'));
                }
            })
            .fail(function () {
                Swal.fire('Error', 'Ha ocurrido un error al procesar la solicitud', 'error');
            })
            .always(function () {
                // Reactivar botón correctamente
                $submitBtn.prop('disabled', false).text('Guardar');
            });
    });
});
