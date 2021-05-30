define(['loading', 'globalize', 'emby-input', 'emby-button', 'emby-select'], function (loading, globalize) {
    'use strict';

    function loadPage(page, config) {
        console.log(config);

        page.querySelector('#txtEmbyStatUrl').value = config.EmbyStatUrl;

        loading.hide();
    }

    function onSubmit(e) {
        e.preventDefault();
        loading.show();

        var form = this;
        ApiClient.getNamedConfiguration("embystat").then(function (config) {
            config.EmbyStatUrl = form.querySelector('#txtEmbyStatUrl').value || null;
            ApiClient.updateNamedConfiguration("embystat", config).then(Dashboard.processServerConfigurationUpdateResult);
        });

        // Disable default form submission
        return false;
    }

    function getConfig() {
        return ApiClient.getNamedConfiguration("embystat");
    }

    return function (view, params) {
        view.querySelector('form').addEventListener('submit', onSubmit);
        view.addEventListener('viewshow', function () {
            loading.show();
            var page = this;
            getConfig().then(function (response) {
                loadPage(page, response);
            });
        });
    };
});