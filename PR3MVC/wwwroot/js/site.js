
(() => {
    const path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.navbar .nav-link').forEach(a => {
        const href = a.getAttribute('href') || '';
        if (href && path.startsWith(href.toLowerCase()))
            a.classList.add('active');
    });
})();

document.addEventListener('click', (e) => {
    const a = e.target.closest('a[data-confirm]');
    if (a && !confirm(a.getAttribute('data-confirm'))) {
        e.preventDefault();
    }
});
