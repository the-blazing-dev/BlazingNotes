function addHashtagCharacter(el) {
    const start = el.selectionStart;
    const value = el.value;
    el.value = value.substring(0, start) + '#' + value.substring(start);
    el.setSelectionRange(start + 1, start + 1);
    el.focus();
}

async function downloadFileFromStream(fileName, contentStreamReference) {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}

function showPicker(el) {
    el.showPicker();
}