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
