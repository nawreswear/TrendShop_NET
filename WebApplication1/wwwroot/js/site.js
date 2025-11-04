// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Attendre que le DOM soit complètement chargé
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initSiteJS);
} else {
    initSiteJS();
}

function initSiteJS() {
    console.log('Initialisation de site.js');

    // Fonction helper pour exécuter du code de manière sécurisée
    function safeExecute(callback, description) {
        try {
            if (typeof callback === 'function') {
                callback();
            }
        } catch (error) {
            console.warn(`Erreur dans ${description}:`, error);
        }
    }

    // Initialiser la sidebar uniquement si les éléments existent
    safeExecute(initSidebar, 'initialisation sidebar');

    // Initialiser les tooltips si Bootstrap est disponible
    safeExecute(function () {
        if (typeof bootstrap !== 'undefined') {
            const tooltips = document.querySelectorAll('[data-bs-toggle="tooltip"]');
            if (tooltips.length > 0) {
                tooltips.forEach(function (el) {
                    try {
                        new bootstrap.Tooltip(el);
                    } catch (e) {
                        console.warn('Erreur avec tooltip:', e);
                    }
                });
            }
        }
    }, 'initialisation tooltips');

    // Initialiser le thème
    safeExecute(initTheme, 'initialisation thème');
}

// Fonction d'initialisation de la sidebar
function initSidebar() {
    var sidebar = document.getElementById("sidebar");
    var mainContent = document.getElementById("mainContent");
    var toggleButton = document.getElementById("toggleMenu");

    if (sidebar && mainContent && toggleButton) {
        toggleButton.addEventListener("click", function () {
            sidebar.classList.toggle("sidebar-hidden");
            mainContent.classList.toggle("main-content-full");
        });
    }
}

// Fonctions globales sécurisées
window.toggleTheme = function () {
    try {
        const currentTheme = document.documentElement.getAttribute('data-theme') || 'light';
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';

        document.documentElement.setAttribute('data-theme', newTheme);
        localStorage.setItem('theme', newTheme);

        const icon = document.getElementById('themeIcon');
        if (icon) {
            icon.className = newTheme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
        }

        // Émettre un événement personnalisé
        if (typeof window.CustomEvent === 'function') {
            window.dispatchEvent(new CustomEvent('themeChanged', { detail: newTheme }));
        }
    } catch (error) {
        console.warn('Erreur toggleTheme:', error);
    }
};

function initTheme() {
    try {
        const theme = localStorage.getItem('theme') || 'light';
        document.documentElement.setAttribute('data-theme', theme);

        const icon = document.getElementById('themeIcon');
        if (icon) {
            icon.className = theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
        }
    } catch (error) {
        console.warn('Erreur initTheme:', error);
    }
}