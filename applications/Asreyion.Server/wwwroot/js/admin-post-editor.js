const blogImageExtensions = new Set([".png", ".jpg", ".jpeg", ".gif", ".webp"]);
const blogFileExtensions = new Set([".pdf", ".zip", ".7z", ".rar", ".csv", ".json", ".xml", ".yaml", ".yml", ".toml", ".ini", ".config", ".cs", ".csx", ".js", ".mjs", ".cjs", ".ts", ".tsx", ".jsx", ".py", ".html", ".htm", ".css", ".scss", ".sql", ".sh", ".ps1", ".go", ".rs", ".java", ".c", ".h", ".cpp", ".hpp", ".php", ".rb", ".log", ".txt", ".md", ".markdown", ".docx", ".xlsx", ".pptx"]);
const maxImageBytes = 10 * 1024 * 1024;
const maxFileBytes = 25 * 1024 * 1024;
const fileExtension = (file) => `.${file.name.split(".").pop().toLowerCase()}`;
const isImageFile = (file) => blogImageExtensions.has(fileExtension(file));
const presetTextColors = {
    Red: "#dc2626",
    Orange: "#ea580c",
    Amber: "#ca8a04",
    Green: "#16a34a",
    Blue: "#2563eb",
    Purple: "#9333ea",
    Pink: "#db2777"
};

const editor = document.getElementById("post-body-editor");
const postForm = document.querySelector(".admin-form");
const status = document.getElementById("post-editor-status");
const imageAltByFile = new WeakMap();
let customColorSelection = null;
const customColorPicker = document.createElement("input");
customColorPicker.type = "color";
customColorPicker.value = "#3366cc";
customColorPicker.className = "admin-visually-hidden-color-picker";
customColorPicker.tabIndex = -1;
postForm?.append(customColorPicker);
customColorPicker.addEventListener("change", () => {
    const color = customColorPicker.value.toLowerCase();
    const attribute = `textColorCustom${color.slice(1)}`;
    Trix.config.textAttributes[attribute] = { style: { color }, inheritable: true };
    window.adminBlogTextColors ||= {};
    window.adminBlogTextColors[attribute] = color;
    if (customColorSelection && editor?.editor) editor.editor.setSelectedRange(customColorSelection);
    applyTextColor(attribute);
    customColorSelection = null;
});
const imagePicker = document.createElement("input");
imagePicker.type = "file";
imagePicker.accept = [...blogImageExtensions].join(",");
imagePicker.hidden = true;
postForm?.append(imagePicker);
imagePicker.addEventListener("change", () => {
    const [file] = imagePicker.files || [];
    if (file && editor) editor.editor.insertFile(file);
    imagePicker.value = "";
});
const filePicker = document.createElement("input");
filePicker.type = "file";
filePicker.accept = [...blogImageExtensions, ...[...blogFileExtensions].map((extension) => `.${extension.slice(1)}`)].join(",");
filePicker.className = "admin-visually-hidden-color-picker";
filePicker.tabIndex = -1;
postForm?.append(filePicker);
filePicker.addEventListener("change", () => {
    const [file] = filePicker.files || [];
    if (file && editor) editor.editor.insertFile(file);
    filePicker.value = "";
});

function showEditorStatus(message) {
    if (status) status.textContent = message;
}

function applyTextColor(attribute) {
    if (!editor?.editor) return;
    const colors = window.adminBlogTextColors || {};
    for (const name of Object.keys(colors)) editor.editor.deactivateAttribute(name);
    for (const name of Object.keys(presetTextColors)) editor.editor.deactivateAttribute(`textColor${name}`);
    if (attribute) editor.editor.activateAttribute(attribute);
}

if (editor) {
    const enhanceToolbar = () => {
        const toolbar = editor.toolbarElement;
        const heading1 = toolbar?.querySelector('[data-trix-attribute="heading1"]');
        if (heading1) {
            heading1.classList.add("admin-trix-heading-button");
            heading1.textContent = "H1";
            heading1.title = "Heading 1";
            heading1.setAttribute("aria-label", "Heading 1");
        }
        const blockTools = toolbar?.querySelector('[data-trix-button-group="block-tools"]');
        if (blockTools && !blockTools.querySelector('[data-trix-attribute="heading2"]')) {
            for (const [attribute, label] of [["heading2", "H2"], ["heading3", "H3"]]) {
                const button = document.createElement("button");
                button.type = "button";
                button.className = "trix-button admin-trix-heading-button";
                button.dataset.trixAttribute = attribute;
                button.title = `Heading ${label.slice(1)}`;
                button.setAttribute("aria-label", `Heading ${label.slice(1)}`);
                button.textContent = label;
                blockTools.insertBefore(button, blockTools.querySelector('[data-trix-attribute="quote"]'));
            }
        }

        const textTools = toolbar?.querySelector('[data-trix-button-group="text-tools"]');
        if (textTools && !toolbar.querySelector(".admin-trix-image-tools")) {
            const imageGroup = document.createElement("span");
            imageGroup.className = "trix-button-group admin-trix-image-tools";
            imageGroup.setAttribute("role", "group");
            imageGroup.setAttribute("aria-label", "Images");
            const imageButton = document.createElement("button");
            imageButton.type = "button";
            imageButton.className = "trix-button admin-trix-image-button";
            imageButton.dataset.trixAction = "x-choose-image";
            imageButton.title = "Insert image";
            imageButton.setAttribute("aria-label", "Insert image");
            imageButton.textContent = "Image";
            imageGroup.append(imageButton);
            const fileButton = document.createElement("button");
            fileButton.type = "button";
            fileButton.className = "trix-button admin-trix-file-button";
            fileButton.dataset.trixAction = "x-choose-file";
            fileButton.title = "Attach a file";
            fileButton.setAttribute("aria-label", "Attach a file");
            fileButton.textContent = "File";
            imageGroup.append(fileButton);
            textTools.after(imageGroup);
        }

        if (textTools && !textTools.parentElement.querySelector(".admin-trix-color-tools")) {
            const group = document.createElement("span");
            group.className = "trix-button-group admin-trix-color-tools";
            group.setAttribute("role", "group");
            group.setAttribute("aria-label", "Text color");
            for (const [name, color] of Object.entries(presetTextColors)) {
                const button = document.createElement("button");
                button.type = "button";
                button.className = "trix-button admin-trix-color-button";
                button.dataset.trixAction = "x-set-text-color";
                button.dataset.colorAttribute = `textColor${name}`;
                button.title = `${name} text`;
                button.setAttribute("aria-label", `${name} text`);
                button.style.setProperty("--swatch-color", color);
                group.append(button);
            }
            const clear = document.createElement("button");
            clear.type = "button";
            clear.className = "trix-button admin-trix-color-clear";
            clear.dataset.trixAction = "x-clear-text-color";
            clear.title = "Remove text color";
            clear.setAttribute("aria-label", "Remove text color");
            clear.textContent = "A";
            group.append(clear);
            const custom = document.createElement("button");
            custom.type = "button";
            custom.className = "trix-button admin-trix-custom-color";
            custom.dataset.trixAction = "x-choose-custom-color";
            custom.title = "Choose a custom text color";
            custom.setAttribute("aria-label", "Choose a custom text color");
            custom.textContent = "Custom";
            group.append(custom);
            textTools.after(group);
        }
    };
    editor.addEventListener("trix-initialize", enhanceToolbar);
    if (editor.editor) enhanceToolbar();

    editor.addEventListener("trix-file-accept", (event) => {
        const file = event.file;
        const extension = fileExtension(file);
        if (isImageFile(file)) {
            if (file.size > maxImageBytes) {
                event.preventDefault();
                showEditorStatus("Images must be 10 MB or smaller.");
                return;
            }
            const altText = window.prompt("Describe this image for readers using screen readers. Leave blank if it is decorative.", file.name.replace(/\.[^.]+$/, ""));
            if (altText === null) {
                event.preventDefault();
                return;
            }
            imageAltByFile.set(file, altText.trim());
            showEditorStatus("");
        } else if (!blogFileExtensions.has(extension) || file.size > maxFileBytes) {
            event.preventDefault();
            showEditorStatus("Choose a supported document, archive, or source file that is 25 MB or smaller.");
        } else {
            showEditorStatus("");
        }
    });

    editor.addEventListener("trix-attachment-add", async (event) => {
        const attachment = event.attachment;
        const file = attachment.file;
        if (!file) return;

        const token = postForm?.querySelector('input[name="__RequestVerificationToken"]')?.value;
        if (!token) {
            attachment.remove();
            showEditorStatus("The upload could not be verified. Reload this page and try again.");
            return;
        }

        const image = isImageFile(file);
        const formData = new FormData();
        formData.append("file", file);
        formData.append("__RequestVerificationToken", token);
        try {
            const response = await fetch(image ? "/Administration/Blog/Images" : "/Administration/Blog/Files", {
                method: "POST",
                body: formData,
                credentials: "same-origin",
                headers: { Accept: "application/json" }
            });
            const result = await response.json();
            if (!response.ok) throw new Error(result.error || "The attachment could not be uploaded.");
            const attachmentAttributes = {
                url: result.url,
                href: result.url,
                filename: result.fileName || file.name,
                contentType: image ? file.type : "application/octet-stream",
                previewable: image
            };
            if (image) attachmentAttributes.alt = imageAltByFile.get(file) || "";
            attachment.setAttributes(attachmentAttributes);
            attachment.setUploadProgress(100);
            showEditorStatus(image ? "Image uploaded." : "File attached.");
        } catch (error) {
            attachment.remove();
            showEditorStatus(error instanceof Error ? error.message : "The attachment could not be uploaded.");
        }
    });
}

document.addEventListener("trix-action-invoke", (event) => {
    const { actionName, invokingElement, target } = event;
    if (actionName === "x-choose-image") {
        imagePicker.click();
        return;
    }
    if (actionName === "x-choose-file") {
        filePicker.click();
        return;
    }
    if (actionName === "x-choose-custom-color") {
        customColorSelection = target.editor.getSelectedRange();
        customColorPicker.click();
        return;
    }
    if (actionName === "x-set-text-color") applyTextColor(invokingElement.dataset.colorAttribute);
    if (actionName === "x-clear-text-color") applyTextColor(null);
});

if (postForm && editor) {
    postForm.addEventListener("submit", (event) => {
        const body = document.getElementById("post-body-input");
        const content = body.value.replace(/<[^>]*>/g, "").replace(/&nbsp;|\s/g, "");
        if (!content) {
            event.preventDefault();
            editor.focus();
            editor.setAttribute("aria-invalid", "true");
            showEditorStatus("Add some content before saving this post.");
        } else {
            editor.removeAttribute("aria-invalid");
        }
    });
}
