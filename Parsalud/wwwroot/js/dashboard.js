async function formatCode(code, parser) {
    const formatted = await prettier.format(code, {
        parser: parser,
        plugins: prettierPlugins,
    });
    return formatted;
}

window.setDotnetReference = (dotnetReference) => {
    window.dotnetReference = dotnetReference;
}

document.addEventListener('keydown', function (event) {
    if (event.ctrlKey && event.key === 's') {
        event.preventDefault();
        window.dotnetReference?.invokeMethodAsync('Save');
    }
});