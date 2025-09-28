\c "gamenest-orderservice-db";
-- ============================================================
--  Table: Customer
--  Stores registered users
-- ============================================================
CREATE TABLE customer (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(50) UNIQUE NOT NULL, 
    email VARCHAR(100) UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT NOW(),
    created_by UUID,
    updated_at TIMESTAMP DEFAULT NOW(),
    updated_by UUID,
    is_deleted BOOLEAN DEFAULT FALSE
);

CREATE INDEX idx_customer_username ON customer(username);
CREATE INDEX idx_customer_email ON customer(email);

-- ============================================================
--  Table: Product
--  Stores available products
-- ============================================================
CREATE TABLE product (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(200) NOT NULL,
    description TEXT,                    
    price DECIMAL(10,2) NOT NULL CHECK (price > 0),
    created_at TIMESTAMP DEFAULT NOW(),
    created_by UUID,
    updated_at TIMESTAMP DEFAULT NOW(),
    updated_by UUID,
    is_deleted BOOLEAN DEFAULT FALSE
);

CREATE INDEX idx_product_title ON product(title);

-- ============================================================
--  Table: Order
--  Stores customer orders
-- ============================================================
CREATE TABLE "order" (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_id UUID NOT NULL REFERENCES customer(id) ON DELETE CASCADE,
    order_date TIMESTAMP DEFAULT NOW(),
    status VARCHAR(20) NOT NULL CHECK (status IN ('Pending','Paid','Cancelled','Refunded')),
    total_amount DECIMAL(10,2) CHECK (total_amount >= 0),
    created_at TIMESTAMP DEFAULT NOW(),
    created_by UUID,
    updated_at TIMESTAMP DEFAULT NOW(),
    updated_by UUID,
    is_deleted BOOLEAN DEFAULT FALSE
);

CREATE INDEX idx_order_customer_id ON "order"(customer_id);

-- ============================================================
--  Table: OrderItem
--  Items that belong to an order (M:N relation between orders and products)
-- ============================================================
CREATE TABLE order_item (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL REFERENCES "order"(id) ON DELETE CASCADE,
    product_id UUID NOT NULL REFERENCES product(id) ON DELETE CASCADE,
    quantity INT NOT NULL CHECK (quantity > 0),
    price DECIMAL(10,2) NOT NULL CHECK (price >= 0),
    created_at TIMESTAMP DEFAULT NOW(),
    created_by UUID,
    updated_at TIMESTAMP DEFAULT NOW(),
    updated_by UUID,
    is_deleted BOOLEAN DEFAULT FALSE
);

CREATE INDEX idx_order_item_order_id ON order_item(order_id);
CREATE INDEX idx_order_item_product_id ON order_item(product_id);

-- ============================================================
--  Table: PaymentRecord
--  Stores payments for orders
-- ============================================================
CREATE TABLE payment_record (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL REFERENCES "order"(id) ON DELETE CASCADE,
    payment_date TIMESTAMP DEFAULT NOW(),
    method VARCHAR(20) NOT NULL CHECK (method IN ('Card','Paypal','Wallet')),
    amount DECIMAL(10,2) NOT NULL CHECK (amount >= 0),
    status VARCHAR(20) NOT NULL CHECK (status IN ('Pending','Success','Failed','Refunded')),
    created_at TIMESTAMP DEFAULT NOW(),
    created_by UUID,
    updated_at TIMESTAMP DEFAULT NOW(),
    updated_by UUID,
    is_deleted BOOLEAN DEFAULT FALSE
);

CREATE INDEX idx_payment_order_id ON payment_record(order_id);

-- ============================================================
--  Seed Data with Audit
-- ============================================================

INSERT INTO customer (username, email, created_by, updated_by) VALUES
('john_doe', 'john@example.com', NULL, NULL),
('alice_wonder', 'alice@example.com', NULL, NULL),
('mark_smith', 'mark@example.com', NULL, NULL);

INSERT INTO product (title, description, price, created_by, updated_by) VALUES
('Gaming Laptop', 'High-end gaming laptop with RTX 4080', 1200.00, NULL, NULL),
('Wireless Mouse', 'Ergonomic wireless mouse', 25.50, NULL, NULL),
('Mechanical Keyboard', 'RGB mechanical keyboard with blue switches', 89.99, NULL, NULL),
('4K Monitor', '27-inch 4K UHD monitor', 350.00, NULL, NULL);

INSERT INTO "order" (customer_id, status, total_amount, created_by, updated_by)
VALUES
((SELECT id FROM customer WHERE username = 'john_doe'), 'Pending', 1225.50, NULL, NULL),
((SELECT id FROM customer WHERE username = 'alice_wonder'), 'Paid', 89.99, NULL, NULL);

INSERT INTO order_item (order_id, product_id, quantity, price, created_by, updated_by)
VALUES
-- John's Order: Laptop + Mouse
((SELECT id FROM "order" WHERE total_amount = 1225.50),
 (SELECT id FROM product WHERE title = 'Gaming Laptop'), 1, 1200.00, NULL, NULL),
((SELECT id FROM "order" WHERE total_amount = 1225.50),
 (SELECT id FROM product WHERE title = 'Wireless Mouse'), 1, 25.50, NULL, NULL),

-- Alice's Order: Keyboard
((SELECT id FROM "order" WHERE total_amount = 89.99),
 (SELECT id FROM product WHERE title = 'Mechanical Keyboard'), 1, 89.99, NULL, NULL);

INSERT INTO payment_record (order_id, method, amount, status, created_by, updated_by)
VALUES
((SELECT id FROM "order" WHERE total_amount = 89.99), 'Card', 89.99, 'Success', NULL, NULL);