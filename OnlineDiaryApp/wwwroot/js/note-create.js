const CLIENT_ID = document.getElementById("googleClientId").value;
const API_KEY = document.getElementById("googleApiKey").value;
const SCOPES = 'https://www.googleapis.com/auth/drive.readonly';

let tokenClient;
let accessToken = null;
let pickerApiLoaded = false;

function gapiLoaded() {
    gapi.load('client:picker', async () => {
        await gapi.client.init({ apiKey: API_KEY });
        pickerApiLoaded = true;
    });
}

function initTokenClient() {
    tokenClient = google.accounts.oauth2.initTokenClient({
        client_id: CLIENT_ID,
        scope: SCOPES,
        callback: (tokenResponse) => {
            accessToken = tokenResponse.access_token;
            if (pickerApiLoaded) createPicker();
        }
    });
}

function createPicker() {
    if (!accessToken || !pickerApiLoaded) return;

    const view = new google.picker.View(google.picker.ViewId.DOCS);
    const picker = new google.picker.PickerBuilder()
        .addView(view)
        .setOAuthToken(accessToken)
        .setDeveloperKey(API_KEY)
        .setCallback(pickerCallback)
        .build();
    picker.setVisible(true);
}

function pickerCallback(data) {
    if (data.action === google.picker.Action.PICKED) {
        const filesContainer = document.getElementById("driveFilesContainer");
        data.docs.forEach(file => {
            const fileUrl = `https://drive.google.com/file/d/${file.id}/view`;
            const div = document.createElement("div");
            div.innerHTML = `<a href="${fileUrl}" target="_blank">${file.name}</a>
                             <input type="hidden" name="GoogleDriveLinks" value="${fileUrl}" />`;
            filesContainer.appendChild(div);
        });
    }
}

document.getElementById('pickFromDrive').addEventListener('click', () => {
    if (!tokenClient) initTokenClient();
    if (!accessToken) {
        tokenClient.requestAccessToken({ prompt: 'consent' });
    } else {
        createPicker();
    }
});

window.addEventListener('DOMContentLoaded', gapiLoaded);

const startBtn = document.getElementById("startRecording");
const stopBtn = document.getElementById("stopRecording");
const audioPlayback = document.getElementById("audioPlayback");
const voiceNoteFile = document.getElementById("voiceNoteFile");
let mediaRecorder;
let audioChunks = [];

startBtn.addEventListener("click", async () => {
    const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
    mediaRecorder = new MediaRecorder(stream);
    audioChunks = [];
    mediaRecorder.ondataavailable = e => audioChunks.push(e.data);
    mediaRecorder.onstop = e => {
        const audioBlob = new Blob(audioChunks, { type: 'audio/webm' });
        audioPlayback.src = URL.createObjectURL(audioBlob);
        audioPlayback.style.display = "block";

        const file = new File([audioBlob], "VoiceNote.webm", { type: "audio/webm" });
        const dt = new DataTransfer();
        dt.items.add(file);
        voiceNoteFile.files = dt.files;
    };
    mediaRecorder.start();
    startBtn.disabled = true;
    stopBtn.disabled = false;
});

stopBtn.addEventListener("click", () => {
    mediaRecorder.stop();
    startBtn.disabled = false;
    stopBtn.disabled = true;
});

const tagSelect = document.getElementById('tagSelect');
const selectedTagsContainer = document.getElementById('selectedTags');

tagSelect.addEventListener('change', function () {
    const val = this.value;
    if (!val || selectedTagsContainer.querySelector(`[data-id="${val}"]`)) return;
    const text = this.options[this.selectedIndex].text;

    const span = document.createElement('span');
    span.className = 'tag-item';
    span.dataset.id = val;
    span.innerHTML = `${text} <button type="button" class="remove-tag">×</button>`;

    const hidden = document.createElement('input');
    hidden.type = 'hidden';
    hidden.name = 'tagIds';
    hidden.value = val;

    selectedTagsContainer.appendChild(span);
    selectedTagsContainer.appendChild(hidden);

    this.value = "";
});

selectedTagsContainer.addEventListener('click', e => {
    if (e.target.classList.contains('remove-tag')) {
        const el = e.target.closest('.tag-item');
        selectedTagsContainer.querySelector(`input[value="${el.dataset.id}"]`)?.remove();
        el.remove();
    }
});

document.querySelector('form').addEventListener('submit', function (e) {
    const editor = document.getElementById('editor');

    if (editor.firstChild && editor.firstChild.nodeType === Node.TEXT_NODE) {
        const p = document.createElement('p');
        p.textContent = editor.firstChild.textContent;
        editor.replaceChild(p, editor.firstChild);
    }

    const editorContent = editor.innerHTML.trim();

    if (!editorContent) {
        alert("Вміст нотатки не може бути порожнім!");
        e.preventDefault();
        return;
    }

    document.getElementById('Content').value = editorContent;
});

function format(cmd) { document.execCommand(cmd, false, null); }
function addLink() {
    const url = prompt("Введіть URL:");
    if (url) document.execCommand("createLink", false, url);
}
