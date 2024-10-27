function addHashtagCharacter(el) {
    const start = el.selectionStart;
    const value = el.value;
    el.value = value.substring(0, start) + '#' + value.substring(start);
    el.setSelectionRange(start + 1, start + 1);
    el.focus();
}

function showPicker(el) {
    el.showPicker();
}