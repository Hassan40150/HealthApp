let skip = 10; // Already loaded 10 entries on page load
const take = 10;
let loading = false;

// Submit new entry
document.getElementById('submitEntryButton').addEventListener('click', async function () {
    const text = document.getElementById('entryText').value.trim();

    if (text.length === 0 || text.length > 280) {
        alert("Entry must be between 1 and 280 characters.");
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
            alert("Entry submitted!");

            // Reset modal
            document.getElementById('entryText').value = '';
            document.getElementById('entryModal').style.display = 'none';

            // Reload page to show new entry (easy way)
            location.reload();
        } else {
            const errorText = await response.text();
            alert(errorText);
        }
    } catch (error) {
        console.error('Error submitting entry:', error);
    }
});

// Infinite Scroll Load More
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
