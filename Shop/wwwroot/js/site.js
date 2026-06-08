// Это пример JavaScript функции для взаимодействия с контроллером
function addToCart(productId) {
    alert('Товар с ID ' + productId + ' будет добавлен в корзину через Controller.');
    // В реальном приложении здесь будет AJAX запрос к CartController/AddToCart
    // fetch('/Cart/AddToCart', {
    //     method: 'POST',
    //     headers: { 'Content-Type': 'application/json' },
    //     body: JSON.stringify({ productId: productId, quantity: 1 })
    // }).then(response => response.json())
    //   .then(data => { /* обновить UI, показать сообщение */ });
}

const themeToggle = document.getElementById('themeToggle');
const pageRoot = document.getElementById('pageRoot');
const currentThemeDisplay = document.getElementById('currentTheme');

// Проверяем, есть ли сохранённая тема
const savedTheme = localStorage.getItem('theme') || 'light';
applyTheme(savedTheme);

// Обработчик клика: guard на случай, если элемент не найден на странице
if (themeToggle && pageRoot) {
    themeToggle.addEventListener('click', () => {
        const currentTheme = pageRoot.getAttribute('data-theme') || 'light';
        const newTheme = currentTheme === 'light' ? 'dark' : 'light';

        // Добавляем анимацию вращения
        themeToggle.classList.add('animate');
        setTimeout(() => themeToggle.classList.remove('animate'), 600);

        applyTheme(newTheme);
    });
}

// Отладочная логика: если ссылки в навигации не работают — логируем их href при клике
document.addEventListener('DOMContentLoaded', () => {
    try {
        const topNav = document.querySelector('.top-nav');
        if (topNav) {
            topNav.addEventListener('click', (e) => {
                const a = e.target.closest('a');
                if (!a) return;
                console.log('[top-nav] clicked href=', a.getAttribute('href'));
                // Если ссылка ведёт на локальную секцию и начинается с '#', браузер прокрутит страницу.
                // Для ссылок на контроллеры мы ничего не блокируем — браузер выполнит переход.
            });
        }
    } catch (err) {
        console.error('Error initializing top-nav debug listener', err);
    }
});

function applyTheme(theme) {
    if (theme === 'dark') {
        pageRoot.setAttribute('data-theme', 'dark');
        themeToggle.innerHTML = '<i class="fas fa-moon"></i>';
        //currentThemeDisplay.textContent = 'тёмная тема';
    } else {
        pageRoot.removeAttribute('data-theme'); // светлая тема — значения по умолчанию
        themeToggle.innerHTML = '<i class="fas fa-sun"></i>';
        //currentThemeDisplay.textContent = 'светлая тема';
    }

    // Сохраняем выбор
    localStorage.setItem('theme', theme);
}