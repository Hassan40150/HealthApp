// Open and close modal
document.getElementById('addEntryButton').addEventListener('click', function () {
    document.getElementById('entryModal').style.display = 'block';
});

document.getElementById('closeModalButton').addEventListener('click', function () {
    document.getElementById('entryModal').style.display = 'none';
});

// Submit journal entry (with error handling inside modal)
document.getElementById('submitEntryButton').addEventListener('click', async function () {
    const text = document.getElementById('entryText').value.trim();
    const errorDiv = document.getElementById('entryError');
    errorDiv.style.display = 'none'; // Hide error
    errorDiv.innerText = '';

    if (text.length === 0 || text.length > 280) {
        errorDiv.innerText = "Entry must be between 1 and 280 characters.";
        errorDiv.style.display = 'block';
        return;
    }

    try {
        const response = await fetch('/journal/addentry', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: `entryText=${encodeURIComponent(text)}`
        });

        if (response.ok) {
            document.getElementById('entryText').value = '';
            document.getElementById('entryModal').style.display = 'none';
            location.reload();
        } else {
            const errorText = await response.text();
            errorDiv.innerText = errorText;
            errorDiv.style.display = 'block';
        }
    } catch (error) {
        errorDiv.innerText = "Something went wrong.";
        errorDiv.style.display = 'block';
        console.error('Error submitting entry:', error);
    }
});

// Infinite scroll to load more entries
let skip = 10;
const take = 10;
let loading = false;

async function loadMoreEntries() {
    if (loading) return;
    loading = true;

    try {
        const response = await fetch(`/journal/getentries?skip=${skip}&take=${take}`);
        const entries = await response.json();

        if (entries.length > 0) {
            entries.forEach(e => {
                const entryHtml = `
                    <div class="journal-entry">
                        <div><b>${e.timestamp.replace('T', ' ').substring(0, 16)}</b></div>
                        <div>${e.entry}</div>
                        <hr />
                    </div>
                `;
                document.getElementById('entriesContainer').insertAdjacentHTML('beforeend', entryHtml);
            });

            skip += entries.length;
        }
    } catch (error) {
        console.error('Error loading more entries:', error);
    }

    loading = false;
}

window.addEventListener('scroll', function () {
    if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 100) {
        loadMoreEntries();
    }
});
