const Component = {

    config: {
        "base_url": "https://localhost:7031",
        "api_path": "/api/FormDataContent/",
        "component_id": "",
        "component_data": {},
        "username": "",
        "email": "",
    },

    PostComponent: function () {
        $(document).on('click', '.btn', function () {
            let $parent = $(this).closest('.cls-form')
            Component.config.component_id = $parent.attr('data-attr')
            let $forms = $parent.find('.dy-frm')
            $forms.each(function (key, value) {
                let api_key = $(this).attr('id')
                let api_value = $(this).val()
                Component.config.component_data[api_key] = api_value
                if (api_key.toLowerCase() == 'email') {
                    Component.config.email = $(this).val()
                }
            })
            let data = {
                "FormPid": Component.config.component_id.toString(),
                "Data": JSON.stringify(Component.config.component_data).toString(),
                "CreatedBy": Component.config.email.toString()
            }
            Component.AjaxCall("POST", data, $(this), $forms)
        })
    },

    AjaxCall: function (method, data, $button_instance, $forms = null) {
        if (data != null) {
            $.ajax({
                url: Component.config.base_url + Component.config.api_path,
                method: method,
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(data),
                beforeSend: function () {
                    $button_instance.prop('disabled', true)
                },
                success: function (response) {
                    debugger
                    if (response.status_code == 200) {
                        var popup = $('body').find('.simple-poppup');
                        $('body').find('#popup').removeClass('error').addClass('success').find('p').text("Data inserted Successfully")
                        popup.fadeIn('slow');
                        setTimeout(function () {
                            popup.fadeOut('slow');
                        }, 1300);
                    }
                    setTimeout(function () {
                        $button_instance.prop('disabled', false)
                    }, 1000);
                    Component.ClearForm($forms)
                },
                error: function (xhr, status, error) {
                    var popup = $('body').find('.simple-poppup');
                    $('body').find('#popup').removeClass('success').addClass('error').find('p').text("Error while processing")
                    popup.fadeIn('slow');
                    setTimeout(function () {
                        $button_instance.prop('disabled', false)
                        popup.fadeOut('slow');
                    }, 1300);
                    Component.ClearForm($forms)
                }
            });
        }
    },

    ClearForm: function ($forms) {
        $forms.each(function (key, value) {
            let api_key = $(this).attr('type')
            if (api_key == 'select') {
                return $(this).find('option').eq(0).prop('selected', true)
            }
            $(this).val('')
        })
    },

    PopUp: function () {
        let html
        html += `<div class="simple-poppup" style="display:none;z-index:99999"  id="popup">`
        html += '<p>Simple popup</p>'
        html += '<div>'
        return html
    },

    Validation: function ($forms) {
        $forms.each(function (key, value) {
            let $this = $(this)
            let api_key = $this.attr('type')
        })
    },
    Init: function () {
        $(document).ready(function () {
            Component.PostComponent()
            $('body').append(Component.PopUp())

        });
    }
}
Component.Init()


