\c "gamenest-orderservice-db";
-- ============================================================
--  Table: Order
--  Stores customer orders (linked to IdentityService user by ID)
-- ============================================================
CREATE TABLE "order" (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_id UUID NOT NULL,
    order_date TIMESTAMP DEFAULT NOW(),
    status VARCHAR(20) NOT NULL CHECK (status IN ('Pending','Paid','Cancelled','Refunded')),
    total_amount DECIMAL(10,2) NOT NULL CHECK (total_amount >= 0),

    country VARCHAR(100) NOT NULL,
    city VARCHAR(100) NOT NULL,
    street VARCHAR(200) NOT NULL,
    zip_code VARCHAR(20) NOT NULL,

    created_at TIMESTAMP DEFAULT NOW(),
    created_by UUID,
    updated_at TIMESTAMP DEFAULT NOW(),
    updated_by UUID,
    is_deleted BOOLEAN DEFAULT FALSE
);

CREATE INDEX idx_order_customer_id ON "order"(customer_id);
CREATE INDEX idx_order_status ON "order"(status);
CREATE INDEX idx_order_city ON "order"(city);

-- ============================================================
--  Table: OrderItem
--  Items that belong to an order (copied from CartService)
-- ============================================================
CREATE TABLE order_item (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL REFERENCES "order"(id) ON DELETE CASCADE,
    product_id UUID NOT NULL,
    product_title VARCHAR(200) NOT NULL,
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
--  Seed Data (Shared GUIDs)
-- ============================================================

-- Users (from SharedSeedData.Users)
-- JohnDoe = 22222222-2222-2222-2222-222222222222
-- AliceWonder = 44444444-4444-4444-4444-444444444444

-- Games (from SharedSeedData.Games)
-- TheWitcher3 = 11111111-1111-1111-1111-111111111111
-- DoomEternal = 33333333-3333-3333-3333-333333333333
-- StardewValley = 55555555-5555-5555-5555-555555555555
-- Cyberpunk2077 = 77777777-7777-7777-7777-777777777777

INSERT INTO "order" (id, customer_id, status, total_amount, country, city, street, zip_code)
VALUES
(
    'a1a1a1a1-aaaa-aaaa-aaaa-000000000001',
    '22222222-2222-2222-2222-222222222222',
    'Pending',
    1350.00,
    'USA', 'New York', '123 Main St', '10001'
),
(
    'a2a2a2a2-bbbb-bbbb-bbbb-000000000002',
    '44444444-4444-4444-4444-444444444444',
    'Paid',
    89.99,
    'Canada', 'Toronto', '456 Queen St', 'M5H 2N2'
);

INSERT INTO order_item (order_id, product_id, product_title, quantity, price)
VALUES
('a1a1a1a1-aaaa-aaaa-aaaa-000000000001', '11111111-1111-1111-1111-111111111111', 'The Witcher 3: Wild Hunt', 1, 1250.00),
('a1a1a1a1-aaaa-aaaa-aaaa-000000000001', '33333333-3333-3333-3333-333333333333', 'DOOM Eternal', 1, 100.00),
('a2a2a2a2-bbbb-bbbb-bbbb-000000000002', '55555555-5555-5555-5555-555555555555', 'Stardew Valley', 1, 89.99);

INSERT INTO payment_record (order_id, method, amount, status)
VALUES
('a2a2a2a2-bbbb-bbbb-bbbb-000000000002', 'Card', 89.99, 'Success');