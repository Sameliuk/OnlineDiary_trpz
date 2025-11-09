const CLIENT_ID = document.getElementById("googleClientId")?.value;
const API_KEY = document.getElementById("googleApiKey")?.value;
const SCOPES = 'https://www.googleapis.com/auth/drive.readonly';

let tokenClient;
let accessToken = null;

function gapiLoaded() {
    gapi.load('client:picker', initializePicker);
}

async function initializePicker() {
    await gapi.client.init({ apiKey: API_KEY });
    tokenClient = google.accounts.oauth2.initTokenClient({
        client_id: CLIENT_ID,
        scope: SCOPES,
        callback: (tokenResponse) => {
            accessToken = tokenResponse.access_token;
            createPicker();
        },
    });
}

function createPicker() {
    if (!accessToken) return;
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
            const fileName = file.name;
            const div = document.createElement("div");
            div.innerHTML = `<a href="${fileUrl}" target="_blank">${file.name}</a>
                                 <input type="hidden" name="GoogleDriveLinks" value="${fileUrl}" />
                                 <input type="hidden" name="GoogleDriveNames" value="${fileName}" />`;
            filesContainer.appendChild(div);
        });
    }
}

document.getElementById('pickFromDrive')?.addEventListener('click', () => {
    if (!tokenClient) return;
    tokenClient.requestAccessToken();
});

window.addEventListener('load', gapiLoaded);

document.addEventListener('DOMContentLoaded', function () {
    const tagSelect = document.getElementById('tagSelect');
    const selectedTagsContainer = document.getElementById('selectedTags');

    tagSelect?.addEventListener('change', function () {
        const selectedValue = this.value;
        const selectedText = this.options[this.selectedIndex].text;
        if (!selectedValue) return;
        if (selectedTagsContainer.querySelector(`[data-id="${selectedValue}"]`)) { this.value = ""; return; }

        const tagEl = document.createElement('span');
        tagEl.className = 'tag-item';
        tagEl.dataset.id = selectedValue;
        tagEl.innerHTML = `${selectedText} <button type="button" class="remove-tag">×</button>`;

        const hiddenInput = document.createElement('input');
        hiddenInput.type = 'hidden';
        hiddenInput.name = 'tagIds';
        hiddenInput.value = selectedValue;

        selectedTagsContainer.appendChild(tagEl);
        selectedTagsContainer.appendChild(hiddenInput);

        this.value = "";
    });

    selectedTagsContainer?.addEventListener('click', function (e) {
        if (e.target.classList.contains('remove-tag')) {
            const tagEl = e.target.closest('.tag-item');
            const tagId = tagEl.dataset.id;
            tagEl.remove();
            selectedTagsContainer.querySelector(`input[value="${tagId}"]`)?.remove();
        }
    });

    document.querySelector("form")?.addEventListener("submit", function () {
        document.getElementById("hiddenContent").value = document.getElementById("editor").innerHTML;
    });
});

function formatText(command) {
    document.execCommand(command, false, null);
}

function insertLink() {
    const url = prompt("Введіть URL:");
    if (url) {
        document.execCommand("createLink", false, url);
    }
}

let mediaRecorder;
let audioChunks = [];

const startBtn = document.getElementById("startRecording");
const stopBtn = document.getElementById("stopRecording");
const audioPlayback = document.getElementById("audioPlayback");
const voiceNoteFile = document.getElementById("voiceNoteFile");

startBtn?.addEventListener("click", async () => {
    const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
    mediaRecorder = new MediaRecorder(stream);
    audioChunks = [];

    mediaRecorder.ondataavailable = e => audioChunks.push(e.data);
    mediaRecorder.onstop = e => {
        const audioBlob = new Blob(audioChunks, { type: 'audio/webm' });
        const audioUrl = URL.createObjectURL(audioBlob);
        audioPlayback.src = audioUrl;
        audioPlayback.style.display = "block";

        const file = new File([audioBlob], "VoiceNote.webm", { type: "audio/webm" });
        const dataTransfer = new DataTransfer();
        dataTransfer.items.add(file);
        voiceNoteFile.files = dataTransfer.files;
    };

    mediaRecorder.start();
    startBtn.disabled = true;
    stopBtn.disabled = false;
});

stopBtn?.addEventListener("click", () => {
    mediaRecorder.stop();
    startBtn.disabled = false;
    stopBtn.disabled = true;
});
