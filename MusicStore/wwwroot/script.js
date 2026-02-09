const API_BASE = '';

const VIEW = {
    TABLE: 'table',
    GALLERY: 'gallery'
};

let state = {
    language: 'en',
    seed: 1,
    avgLikes: 0,
    page: 1,
    pageSize: 20
};

let viewMode = VIEW.TABLE;
let isLoading = false;


const languageSelect = document.getElementById('language');
const seedInput = document.getElementById('seed');
const likesInput = document.getElementById('likes');
const randomSeedBtn = document.getElementById('randomSeed');

const tableEl = document.getElementById('songsTable');
const tableBody = document.querySelector('#songsTable tbody');

const prevBtn = document.getElementById('prevPage');
const nextBtn = document.getElementById('nextPage');
const pageInfo = document.getElementById('pageInfo');

const toggleViewBtn = document.getElementById('toggleView');
const galleryEl = document.getElementById('gallery');


function buildSongsUrl() {
    return `${API_BASE}/api/songs` +
        `?language=${state.language}` +
        `&seed=${state.seed}` +
        `&page=${state.page}` +
        `&pageSize=${state.pageSize}` +
        `&avgLikes=${state.avgLikes}`;
}

async function loadSongs() {
    if (isLoading) return;
    isLoading = true;

    const res = await fetch(buildSongsUrl());
    const data = await res.json();

    renderTable(data.songs);
    isLoading = false;
}

function renderTable(songs) {
    tableBody.innerHTML = '';
    pageInfo.textContent = `Page ${state.page}`;
    prevBtn.disabled = state.page === 1;

    songs.forEach(song => {
        const tr = document.createElement('tr');

        tr.innerHTML = `
            <td>${song.index}</td>
            <td>${song.title}</td>
            <td>${song.artist}</td>
            <td>${song.album}</td>
            <td>${song.genre}</td>
            <td>${song.likes}</td>
            <td><button class="toggle">Details</button></td>
        `;

        const detailsRow = document.createElement('tr');
        detailsRow.style.display = 'none';
        detailsRow.classList.add('expanded');

        detailsRow.innerHTML = `
            <td colspan="7">
                <div class="details">
                    <strong>${song.title}</strong> — ${song.artist}<br/><br/>
                    <audio controls preload="none">
                        <source src="${API_BASE}${song.audioUrl}?seed=${state.seed}" type="audio/wav">
                    </audio>
                </div>
            </td>
        `;

        tr.querySelector('.toggle').addEventListener('click', () => {
            detailsRow.style.display =
                detailsRow.style.display === 'table-row' ? 'none' : 'table-row';
        });

        tableBody.appendChild(tr);
        tableBody.appendChild(detailsRow);
    });
}


async function loadGalleryPage() {
    if (isLoading) return;
    isLoading = true;

    const res = await fetch(buildSongsUrl());
    const data = await res.json();

    renderGallery(data.songs);
    state.page++;

    isLoading = false;
}

function renderGallery(songs) {
    songs.forEach(song => {
        const card = document.createElement('div');
        card.className = 'card';

        card.innerHTML = `
            <h4>${song.title}</h4>
            <small>${song.artist}</small><br/><br/>
            <strong>${song.album}</strong><br/>
            <em>${song.genre}</em><br/><br/>
            ❤️ ${song.likes}<br/><br/>
            <audio controls preload="none">
                <source src="${API_BASE}${song.audioUrl}?seed=${state.seed}" type="audio/wav">
            </audio>
        `;

        galleryEl.appendChild(card);
    });
}

function resetGallery() {
    galleryEl.innerHTML = '';
    state.page = 1;
    window.scrollTo(0, 0);
}


function resetAndReload() {
    state.page = 1;
    window.scrollTo(0, 0);
    loadSongs();
}

languageSelect.addEventListener('change', e => {
    state.language = e.target.value;

    if (viewMode === VIEW.GALLERY) {
        resetGallery();
        loadGalleryPage();
    } else {
        resetAndReload();
    }
});


seedInput.addEventListener('input', e => {
    const value = e.target.value.trim();

    if (value === '') {
        return;
    }

    const parsed = Number(value);

    if (Number.isNaN(parsed)) {
        return;
    }

    state.seed = parsed;

    if (viewMode === VIEW.GALLERY) {
        resetGallery();
        loadGalleryPage();
    } else {
        resetAndReload();
    }
});


likesInput.addEventListener('change', e => {
    state.avgLikes = Number(e.target.value) || 0;

    if (viewMode === VIEW.GALLERY) {
        resetGallery();
        loadGalleryPage();
    } else {
        loadSongs();
    }
});

randomSeedBtn.addEventListener('click', () => {
    const randomSeed = Math.floor(Math.random() * 1_000_000_000_000);
    state.seed = randomSeed;
    seedInput.value = randomSeed;
    viewMode === VIEW.GALLERY ? resetGallery() || loadGalleryPage() : resetAndReload();
});

prevBtn.addEventListener('click', () => {
    if (state.page > 1) {
        state.page--;
        loadSongs();
    }
});

nextBtn.addEventListener('click', () => {
    state.page++;
    loadSongs();
});

toggleViewBtn.addEventListener('click', () => {
    if (viewMode === VIEW.TABLE) {
        viewMode = VIEW.GALLERY;
        toggleViewBtn.textContent = 'Table';
        tableEl.classList.add('hidden');
        galleryEl.classList.remove('hidden');

        resetGallery();
        loadGalleryPage();
    } else {
        viewMode = VIEW.TABLE;
        toggleViewBtn.textContent = 'Gallery';

        galleryEl.classList.add('hidden');
        tableEl.classList.remove('hidden');

        document.body.style.height = 'auto';
        document.documentElement.style.height = 'auto';
        window.scrollTo(0, 0);

        resetAndReload();
    }
});

//INFINITE SCROLL FOR GALLERY VIEW
window.addEventListener('scroll', () => {
    if (viewMode !== VIEW.GALLERY) return;

    if (window.innerHeight + window.scrollY >= document.body.offsetHeight - 300) {
        loadGalleryPage();
    }
});

loadSongs();
