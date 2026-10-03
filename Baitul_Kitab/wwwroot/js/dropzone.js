(function ($) {
    $.fn.imageUploader = function (options) {
        let defaults = {
            preloaded: [],
            imagesInputName: 'images',
            preloadedInputName: 'preloaded',
            label: 'Drag & Drop files here or click to browse'
        };
        let plugin = this;
        plugin.settings = {};

        let updateContainerState = function () {
            plugin.each(function (_, wrapper) {
                let $container = $(wrapper).find('.image-uploader');
                let $uploadedContainer = $container.find('.uploaded');
                let $textContainer = $container.find('.upload-text');

                if ($uploadedContainer.children().length > 0) {
                    $container.addClass('has-files');
                    $textContainer.hide();
                } else {
                    $container.removeClass('has-files');
                    $textContainer.show();
                }
            });
        };

        plugin.init = function () {
            plugin.settings = $.extend(plugin.settings, defaults, options);
            plugin.each(function (i, wrapper) {
                let $container = createContainer();
                $(wrapper).append($container);
                $container.on("dragover", fileDragHover.bind($container));
                $container.on("dragleave", fileDragHover.bind($container));
                $container.on("drop", fileSelectHandler.bind($container));
                if (plugin.settings.preloaded.length) {
                    $container.addClass('has-files');
                    let $uploadedContainer = $container.find('.uploaded');
                    for (let i = 0; i < plugin.settings.preloaded.length; i++) {
                        $uploadedContainer.append(createImg(plugin.settings.preloaded[i].src, plugin.settings.preloaded[i].id, true));
                    }
                }
            });
        };

        let dataTransfer = new DataTransfer();

        let createContainer = function () {
            let $container = $('<div>', { class: 'image-uploader' }),
                $input = $('<input>', { type: 'file', id: plugin.settings.imagesInputName + '-' + random(), name: plugin.settings.imagesInputName + '[]', multiple: '', }).appendTo($container),
                $uploadedContainer = $('<div>', { class: 'uploaded' }).appendTo($container),
                $textContainer = $('<div>', { class: 'upload-text' }).appendTo($container),
                $i = $('<i>', { class: 'fas fa-cloud-upload-alt color-blue', text: '' }).appendTo($textContainer),
                $span = $('<span>', { text: plugin.settings.label }).appendTo($textContainer);
            var $captureIcon = $('<i>', { class: ' capture-icon' }).appendTo($container);
            var $captureText = $('<span>').addClass('capture-text').text('').appendTo($container);
            $container.on('click', function (e) {
                prevent(e);
                $input.trigger('click');
            });
            $input.on("click", function (e) { e.stopPropagation() });
            $input.on('change', fileSelectHandler.bind($container));
            $captureIcon.on('click', openCamera);

            return $container;
        };

        let openCamera = function (e) {
            e.preventDefault();
            console.log('Open camera clicked');

            if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
                navigator.mediaDevices.getUserMedia({ video: { facingMode: 'environment' }, audio: false })
                    .then(function (stream) {
                        console.log('Camera access granted');

                        let video = document.createElement('video');
                        video.setAttribute('autoplay', '');
                        video.srcObject = stream;
                        document.body.appendChild(video);

                        let canvas = document.createElement('canvas');
                        let ctx = canvas.getContext('2d');
                        video.onloadedmetadata = function () {
                            canvas.width = video.videoWidth;
                            canvas.height = video.videoHeight;

                            ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

                            stream.getTracks().forEach(track => track.stop());
                            video.remove();
                            let imageDataURL = canvas.toDataURL('image/png');
                            console.log('Captured image data URL:', imageDataURL);
                            // Handle the captured image (e.g., add it to the uploader)
                        };
                    })
                    .catch(function (error) {
                        console.error('Error accessing camera:', error);
                    });
            } else {
                console.error('getUserMedia is not supported');
            }
        };

        let prevent = function (e) {
            e.preventDefault();
            e.stopPropagation();
        };

        let createImg = function (src, id, isPreloaded) {
            let $container = $('<div>', { class: 'uploaded-image' }),
                $img = $('<img>', { src: src }).appendTo($container),
                $button = $('<i>', { class: 'delete-image fa-regular fa-circle-xmark' }).appendTo($container);
            $container.attr('data-index', id);
            $container.on("click", function (e) {

                prevent(e)
                // Show confirmation alert before proceeding with deletion
                if (confirm('Are you sure you want to delete this image?')) {
                    let index = parseInt($container.data('index'));
                    dataTransfer.items.remove(index);
                    $container.remove();
                    $container.siblings().each(function (i, cont) {
                        let newIndex = parseInt($(cont).attr('data-index'));
                        if (newIndex > index) {
                            $(cont).attr('data-index', newIndex - 1);
                        }
                    });
                    let $input = $container.closest('.image-uploader').find('input[type="file"]');
                    $input.prop('files', dataTransfer.files);
    
    
                    if (index == 0) {
                        updateContainerState();
                    }
                } else {
                   
                    return;
                }

            });

            return $container;
        }

        let fileDragHover = function (e) { prevent(e); if (e.type === "dragover") { $(this).addClass('drag-over') } else { $(this).removeClass('drag-over') } };

        let fileSelectHandler = function (e) {
            prevent(e);
            let $container = $(this);
            $container.removeClass('drag-over');
            let files = e.target.files || e.originalEvent.dataTransfer.files;

            let heicFiles = [];
            let originalImageFiles = [];
            let convertedImageFiles = [];

            Array.from(files).forEach(file => {
                const isHeic = file.name.toLowerCase().endsWith('.heic') || file.name.toLowerCase().endsWith('.heif');

                if (isHeic) {
                    heicFiles.push(file);
                } else {
                    const allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp', 'image/bmp', 'image/tiff', 'image/svg+xml', 'image/x-icon'];
                    if (allowedTypes.includes(file.type)) {
                        originalImageFiles.push(file);
                    }
                }
            });

            if (heicFiles.length > 0) {
                const conversionPromises = heicFiles.map(file => {
                    return convertHEICtoImage(file)
                        .then(imageBlob => {
                            convertedImageFiles.push(imageBlob);
                        })
                        .catch(error => {
                            console.error("Error converting HEIC file:", error);
                        });
                });

                Promise.all(conversionPromises)
                    .then(() => {
                        setPreview($container, originalImageFiles.concat(convertedImageFiles));
                    });
            } else {
                setPreview($container, originalImageFiles);
            }
        };

        function convertHEICtoImage(file) {
            return new Promise((resolve, reject) => {
                const reader = new FileReader();
                reader.onload = function (event) {
                    fetch(event.target.result)
                        .then(res => res.blob())
                        .then(blob => heic2any({ blob }))
                        .then(conversionResult => {
                            resolve(new File([conversionResult], `${file.name}.jpeg`, { type: 'image/jpeg' }));
                        })
                        .catch(error => {
                            console.error("Error converting HEIC to JPEG:", error);
                            reject(error);
                        });
                };
                reader.readAsDataURL(file);
            });
        }

        let setPreview = function ($container, files) {
            $container.addClass('has-files');
            let $uploadedContainer = $container.find('.uploaded');

            $(files).each(function (i, file) {
                dataTransfer.items.add(file);
                $uploadedContainer.append(createImg(URL.createObjectURL(file), dataTransfer.items.length - 1));
            });
            let $input = $container.find('input[type="file"]');
            $input.prop('files', dataTransfer.files);
        };

        let random = function () { return Date.now() + Math.floor((Math.random() * 100) + 1) };

        this.init();
        return this;
    };
}(jQuery));
