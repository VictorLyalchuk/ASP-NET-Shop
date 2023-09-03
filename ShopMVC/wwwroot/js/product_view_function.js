    function formatNumberWithCommas(number) {
        return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
    var totalAmount = parseFloat(document.getElementById('total-amount').textContent);

    function updateQuantity(productId, quantity) {
        fetch(`/Cart/UpdateQuantity?productId=${productId}&quantity=${quantity}`, {
            method: 'POST'
        })
            .then(response => {
                if (response.ok) {
                    return response.json();
                } else {
                    throw new Error('Network response was not ok');
                }
            })
            .then(data => {
                if (data.success) {
                    var quantityElement = document.getElementById(`quantity-${productId}`);
                    var currentQuantity = parseInt(quantityElement.textContent);

                    var productPriceElement = document.getElementById(`product-price-${productId}`);
                    var productPrice = parseFloat(productPriceElement.textContent);

                    var priceString = productPriceElement.textContent.replace(/\s/g, '');
                    priceString = priceString.replace(',', '.');
                    productPrice = parseFloat(priceString);

                    var productTotalPriceElement = document.getElementById(`product-total-${productId}`);
                    var newTotalPrice = productPrice * currentQuantity;
                    productTotalPriceElement.textContent = formatNumberWithCommas(newTotalPrice.toFixed(2)) + '₴';
                    console.log("new price", newTotalPrice);
                    console.log("new price", currentQuantity);

                    var priceDifference = productPrice * (quantity - currentQuantity);
                    totalAmount += priceDifference;
                    var totalAmountElement = document.getElementById('total-amount');
                    totalAmountElement.textContent = formatNumberWithCommas(totalAmount.toFixed(2)) + '₴';

                    currentQuantity = quantity;
                    quantityElement.textContent = currentQuantity;

                } else {
                    console.error('Update failed');
                }
            })
            .catch(error => {
                console.error('There was a problem with the fetch operation:', error);
            });
    }

    // Обробник події для кнопки "+"
    document.querySelectorAll('.increase').forEach(button => {
        button.addEventListener('click', function () {
            var productId = this.getAttribute('data-product-id');
            var quantityElement = document.getElementById(`quantity-${productId}`);
            var currentQuantity = parseInt(quantityElement.textContent);
            currentQuantity++;
            updateQuantity(productId, currentQuantity);
        });
    });

    // Обробник події для кнопки "-"
    document.querySelectorAll('.decrease').forEach(button => {
        button.addEventListener('click', function () {
            var productId = this.getAttribute('data-product-id');
            var quantityElement = document.getElementById(`quantity-${productId}`);
            var currentQuantity = parseInt(quantityElement.textContent);
            if (currentQuantity > 1) {
                currentQuantity--;
                updateQuantity(productId, currentQuantity);
            }
        });
    });
