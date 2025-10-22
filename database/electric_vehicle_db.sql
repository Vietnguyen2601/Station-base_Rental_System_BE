-- =========================================
-- TẠO DATABASE NẾU CHƯA TỒN TẠI
-- =========================================
DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_database WHERE datname = 'electric_vehicle_db') THEN
        EXECUTE 'CREATE DATABASE electric_vehicle_db';
        RAISE NOTICE 'Database electric_vehicle_db đã được tạo.';
    ELSE
        RAISE NOTICE 'Database electric_vehicle_db đã tồn tại.';
    END IF;
END
$$;

-- =========================================
-- TẠO EXTENSION & FUNCTION CƠ BẢN
-- =========================================
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE OR REPLACE FUNCTION set_created_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.created_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- =========================================
-- TẠO ENUM TYPES
-- =========================================
DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_type WHERE typname = 'vehicle_status') THEN
        CREATE TYPE vehicle_status AS ENUM ('AVAILABLE', 'RENTED', 'MAINTENANCE', 'CHARGING');
    END IF;
END
$$;

DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_type WHERE typname = 'order_status') THEN
        CREATE TYPE order_status AS ENUM ('PENDING', 'CONFIRMED', 'ONGOING', 'COMPLETED', 'CANCELED');
    END IF;
END
$$;

-- =========================================
-- XÓA BẢNG NẾU TỒN TẠI
-- =========================================
DROP TABLE IF EXISTS "Staff_Revenues" CASCADE;
DROP TABLE IF EXISTS "Reports" CASCADE;
DROP TABLE IF EXISTS "Feedbacks" CASCADE;
DROP TABLE IF EXISTS "Payments" CASCADE;
DROP TABLE IF EXISTS "Contracts" CASCADE;
DROP TABLE IF EXISTS "Orders" CASCADE;
DROP TABLE IF EXISTS "Promotions" CASCADE;
DROP TABLE IF EXISTS "Vehicles" CASCADE;
DROP TABLE IF EXISTS "VehicleModels" CASCADE;
DROP TABLE IF EXISTS "VehicleTypes" CASCADE;
DROP TABLE IF EXISTS "Stations" CASCADE;
DROP TABLE IF EXISTS "Licenses" CASCADE;
DROP TABLE IF EXISTS "Account_Roles" CASCADE;
DROP TABLE IF EXISTS "Roles" CASCADE;
DROP TABLE IF EXISTS "Accounts" CASCADE;

-- =========================================
-- TẠO BẢNG
-- =========================================

CREATE TABLE "Roles" (
  role_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  role_name VARCHAR(100) NOT NULL,
  isActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE "Accounts" (
  account_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  username VARCHAR(100) NOT NULL,
  password VARCHAR(255) NOT NULL,
  email VARCHAR(150) NOT NULL,
  contact_number VARCHAR(20),
  role_id UUID NOT NULL,
  isActive BOOLEAN NOT NULL DEFAULT TRUE,
  FOREIGN KEY (role_id) REFERENCES "Roles"(role_id)
);

CREATE TABLE "VehicleTypes" (
  vehicle_type_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  type_name VARCHAR(100) UNIQUE NOT NULL,
  description TEXT,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  isActive BOOLEAN NOT NULL DEFAULT TRUE,
  updated_at TIMESTAMP
);

CREATE TABLE "VehicleModels" (
  vehicle_model_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  type_id UUID NOT NULL,
  name VARCHAR(100) NOT NULL,
  manufacturer VARCHAR(100) NOT NULL,
  price_per_hour DECIMAL NOT NULL,
  specs VARCHAR(255),
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  isActive BOOLEAN NOT NULL DEFAULT TRUE,
  updated_at TIMESTAMP,
  FOREIGN KEY (type_id) REFERENCES "VehicleTypes"(vehicle_type_id)
);

CREATE TABLE "Stations" (
  station_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(100) NOT NULL,
  address VARCHAR(255) NOT NULL,
  lat DECIMAL NOT NULL,
  long DECIMAL NOT NULL,
  capacity INT NOT NULL,
  image_url VARCHAR(255),
  isActive BOOLEAN NOT NULL DEFAULT TRUE,
  updated_at TIMESTAMP
);

CREATE TABLE "Vehicles" (
  vehicle_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  serial_number VARCHAR(100) UNIQUE NOT NULL,
  model_id UUID NOT NULL,
  station_id UUID,
  status vehicle_status NOT NULL DEFAULT 'AVAILABLE',
  battery_level INT,
  battery_capacity INT,
  range INT,
  color VARCHAR(50),
  last_maintenance DATE,
  img VARCHAR(255),
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  isActive BOOLEAN NOT NULL DEFAULT TRUE,
  updated_at TIMESTAMP,
  FOREIGN KEY (model_id) REFERENCES "VehicleModels"(vehicle_model_id),
  FOREIGN KEY (station_id) REFERENCES "Stations"(station_id)
);

CREATE TABLE "Promotions" (
  promotion_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  promo_code VARCHAR(50) NOT NULL,
  discount_percentage DECIMAL NOT NULL,
  start_date TIMESTAMP NOT NULL,
  end_date TIMESTAMP NOT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP,
  isActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE "Orders" (
  order_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  customer_id UUID NOT NULL,
  vehicle_id UUID NOT NULL,
  order_date TIMESTAMP NOT NULL,
  start_time TIMESTAMP NOT NULL,
  end_time TIMESTAMP,
  total_price DECIMAL NOT NULL,
  status order_status NOT NULL DEFAULT 'PENDING',
  promotion_id UUID,
  staff_id UUID NOT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP,
  isActive BOOLEAN NOT NULL DEFAULT TRUE,
  FOREIGN KEY (customer_id) REFERENCES "Accounts"(account_id),
  FOREIGN KEY (vehicle_id) REFERENCES "Vehicles"(vehicle_id),
  FOREIGN KEY (promotion_id) REFERENCES "Promotions"(promotion_id),
  FOREIGN KEY (staff_id) REFERENCES "Accounts"(account_id)
);

CREATE TABLE "Payments" (
  payment_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  order_id UUID NOT NULL,
  amount DECIMAL NOT NULL,
  payment_date TIMESTAMP NOT NULL,
  payment_method VARCHAR(100) NOT NULL,
  status VARCHAR(50) NOT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP,
  isActive BOOLEAN NOT NULL DEFAULT TRUE,
  FOREIGN KEY (order_id) REFERENCES "Orders"(order_id)
);

CREATE TABLE "Feedbacks" (
  feedback_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  customer_id UUID NOT NULL,
  order_id UUID NOT NULL,
  rating INT NOT NULL,
  comment TEXT,
  feedback_date TIMESTAMP NOT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP,
  isActive BOOLEAN NOT NULL DEFAULT TRUE,
  FOREIGN KEY (customer_id) REFERENCES "Accounts"(account_id),
  FOREIGN KEY (order_id) REFERENCES "Orders"(order_id)
);

CREATE TABLE "Reports" (
  report_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  report_type VARCHAR(100) NOT NULL,
  generated_date TIMESTAMP NOT NULL,
  text VARCHAR(255) NOT NULL,
  account_id UUID NOT NULL,
  vehicle_id UUID NOT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP,
  isActive BOOLEAN NOT NULL DEFAULT TRUE,
  FOREIGN KEY (account_id) REFERENCES "Accounts"(account_id),
  FOREIGN KEY (vehicle_id) REFERENCES "Vehicles"(vehicle_id)
);

CREATE TABLE "Staff_Revenues" (
  staff_revenue_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  staff_id UUID NOT NULL,
  revenue_date TIMESTAMP NOT NULL,
  total_revenue DECIMAL NOT NULL,
  commission DECIMAL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP,
  isActive BOOLEAN NOT NULL DEFAULT TRUE,
  FOREIGN KEY (staff_id) REFERENCES "Accounts"(account_id)
);

-- =========================================
-- TRIGGER created_at
-- =========================================
CREATE TRIGGER set_vehicle_types_created_at BEFORE INSERT ON "VehicleTypes" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_vehicle_models_created_at BEFORE INSERT ON "VehicleModels" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_vehicles_created_at BEFORE INSERT ON "Vehicles" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_promotions_created_at BEFORE INSERT ON "Promotions" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_orders_created_at BEFORE INSERT ON "Orders" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_payments_created_at BEFORE INSERT ON "Payments" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_feedbacks_created_at BEFORE INSERT ON "Feedbacks" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_reports_created_at BEFORE INSERT ON "Reports" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_staff_revenues_created_at BEFORE INSERT ON "Staff_Revenues" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();

-- =========================================
-- TRIGGER updated_at
-- =========================================
CREATE TRIGGER update_accounts_updated_at BEFORE UPDATE ON "Accounts" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_roles_updated_at BEFORE UPDATE ON "Roles" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_vehicle_types_updated_at BEFORE UPDATE ON "VehicleTypes" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_vehicle_models_updated_at BEFORE UPDATE ON "VehicleModels" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_vehicles_updated_at BEFORE UPDATE ON "Vehicles" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_stations_updated_at BEFORE UPDATE ON "Stations" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_promotions_updated_at BEFORE UPDATE ON "Promotions" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_orders_updated_at BEFORE UPDATE ON "Orders" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_payments_updated_at BEFORE UPDATE ON "Payments" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_feedbacks_updated_at BEFORE UPDATE ON "Feedbacks" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_reports_updated_at BEFORE UPDATE ON "Reports" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_staff_revenues_updated_at BEFORE UPDATE ON "Staff_Revenues" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- =========================================
-- Insert Data
-- =========================================
-- Chèn 3 bản ghi vào bảng Roles
INSERT INTO "Roles" (role_id, role_name, isActive) VALUES
(gen_random_uuid(), 'Customer', TRUE),
(gen_random_uuid(), 'Staff', TRUE),
(gen_random_uuid(), 'Admin', TRUE);

-- Chèn 10 bản ghi vào bảng Accounts
INSERT INTO "Accounts" (account_id, username, password, email, contact_number, role_id, isActive) VALUES
(gen_random_uuid(), 'john_doe', 'hashed_password1', 'john.doe@example.com', '1234567890', (SELECT role_id FROM "Roles" WHERE role_name = 'Customer'), TRUE),
(gen_random_uuid(), 'jane_smith', 'hashed_password2', 'jane.smith@example.com', '1234567891', (SELECT role_id FROM "Roles" WHERE role_name = 'Customer'), TRUE),
(gen_random_uuid(), 'alice_wong', 'hashed_password3', 'alice.wong@example.com', '1234567892', (SELECT role_id FROM "Roles" WHERE role_name = 'Customer'), TRUE),
(gen_random_uuid(), 'bob_jones', 'hashed_password4', 'bob.jones@example.com', '1234567893', (SELECT role_id FROM "Roles" WHERE role_name = 'Staff'), TRUE),
(gen_random_uuid(), 'carol_white', 'hashed_password5', 'carol.white@example.com', '1234567894', (SELECT role_id FROM "Roles" WHERE role_name = 'Staff'), TRUE),
(gen_random_uuid(), 'david_brown', 'hashed_password6', 'david.brown@example.com', '1234567895', (SELECT role_id FROM "Roles" WHERE role_name = 'Staff'), TRUE),
(gen_random_uuid(), 'emma_clark', 'hashed_password7', 'emma.clark@example.com', '1234567896', (SELECT role_id FROM "Roles" WHERE role_name = 'Admin'), TRUE),
(gen_random_uuid(), 'frank_lee', 'hashed_password8', 'frank.lee@example.com', '1234567897', (SELECT role_id FROM "Roles" WHERE role_name = 'Admin'), TRUE),
(gen_random_uuid(), 'grace_kim', 'hashed_password9', 'grace.kim@example.com', '1234567898', (SELECT role_id FROM "Roles" WHERE role_name = 'Admin'), TRUE),
(gen_random_uuid(), 'henry_park', 'hashed_password10', 'henry.park@example.com', '1234567899', (SELECT role_id FROM "Roles" WHERE role_name = 'Admin'), TRUE);

-- Chèn 10 bản ghi vào bảng VehicleTypes
INSERT INTO "VehicleTypes" (vehicle_type_id, type_name, description, created_at, isActive) VALUES
(gen_random_uuid(), 'Sedan', 'Four-door passenger car', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'SUV', 'Sport Utility Vehicle', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Truck', 'Heavy-duty vehicle for cargo', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Van', 'Large vehicle for passengers', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Electric', 'Battery-powered vehicle', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Hybrid', 'Gasoline and electric powered', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Motorcycle', 'Two-wheeled vehicle', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Convertible', 'Car with retractable roof', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Coupe', 'Two-door sporty car', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Hatchback', 'Car with rear door', CURRENT_TIMESTAMP, TRUE);

-- Chèn 10 bản ghi vào bảng VehicleModels
INSERT INTO "VehicleModels" (vehicle_model_id, type_id, name, manufacturer, price_per_hour, specs, created_at, isActive) VALUES
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Sedan'), 'Camry', 'Toyota', 15.00, '2.5L 4-cylinder', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Sedan'), 'Accord', 'Honda', 14.50, '1.5L Turbo', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'SUV'), 'RAV4', 'Toyota', 20.00, '2.5L 4-cylinder', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'SUV'), 'CR-V', 'Honda', 19.50, '1.5L Turbo', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Electric'), 'Model 3', 'Tesla', 25.00, 'Electric 75 kWh', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Electric'), 'Leaf', 'Nissan', 22.00, 'Electric 40 kWh', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Truck'), 'F-150', 'Ford', 30.00, '3.5L V6', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Van'), 'Sienna', 'Toyota', 28.00, '3.5L V6', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Motorcycle'), 'Ninja', 'Kawasaki', 10.00, '400cc', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Coupe'), 'Mustang', 'Ford', 18.00, '5.0L V8', CURRENT_TIMESTAMP, TRUE);

-- Chèn 10 bản ghi vào bảng Stations
INSERT INTO "Stations" (station_id, name, address, lat, long, capacity, image_url, isActive) VALUES
(gen_random_uuid(), 'Downtown Hub', '123 Main St, City', 10.123456, 106.123456, 50, 'http://example.com/station1.jpg', TRUE),
(gen_random_uuid(), 'North Station', '456 North Rd, City', 10.234567, 106.234567, 30, 'http://example.com/station2.jpg', TRUE),
(gen_random_uuid(), 'South Station', '789 South Rd, City', 10.345678, 106.345678, 40, 'http://example.com/station3.jpg', TRUE),
(gen_random_uuid(), 'East Station', '101 East Ave, City', 10.456789, 106.456789, 25, 'http://example.com/station4.jpg', TRUE),
(gen_random_uuid(), 'West Station', '202 West Blvd, City', 10.567890, 106.567890, 35, 'http://example.com/station5.jpg', TRUE),
(gen_random_uuid(), 'Central Hub', '303 Central St, City', 10.678901, 106.678901, 60, 'http://example.com/station6.jpg', TRUE),
(gen_random_uuid(), 'Airport Station', '404 Airport Rd, City', 10.789012, 106.789012, 45, 'http://example.com/station7.jpg', TRUE),
(gen_random_uuid(), 'Suburban Stop', '505 Suburb Ln, City', 10.890123, 106.890123, 20, 'http://example.com/station8.jpg', TRUE),
(gen_random_uuid(), 'City Park', '606 Park Ave, City', 10.901234, 106.901234, 30, 'http://example.com/station9.jpg', TRUE),
(gen_random_uuid(), 'Mall Station', '707 Mall Rd, City', 10.012345, 106.012345, 50, 'http://example.com/station10.jpg', TRUE);

-- Chèn 10 bản ghi vào bảng Vehicles (đã sửa, sử dụng 'RENTED' thay vì 'IN_USE')
INSERT INTO "Vehicles" (vehicle_id, serial_number, model_id, station_id, status, battery_level, battery_capacity, range, color, last_maintenance, img, created_at, isActive) VALUES
(gen_random_uuid(), 'SN123456', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Camry'), (SELECT station_id FROM "Stations" WHERE name = 'Downtown Hub'), 'AVAILABLE', NULL, NULL, NULL, 'Blue', '2025-09-01', 'http://example.com/vehicle1.jpg', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'SN123457', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Accord'), (SELECT station_id FROM "Stations" WHERE name = 'North Station'), 'AVAILABLE', NULL, NULL, NULL, 'Red', '2025-09-02', 'http://example.com/vehicle2.jpg', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'SN123458', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Model 3'), (SELECT station_id FROM "Stations" WHERE name = 'South Station'), 'AVAILABLE', 80, 75, 300, 'White', '2025-09-03', 'http://example.com/vehicle3.jpg', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'SN123459', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Leaf'), (SELECT station_id FROM "Stations" WHERE name = 'East Station'), 'RENTED', 60, 40, 150, 'Black', '2025-09-04', 'http://example.com/vehicle4.jpg', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'SN123460', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'RAV4'), (SELECT station_id FROM "Stations" WHERE name = 'West Station'), 'AVAILABLE', NULL, NULL, NULL, 'Silver', '2025-09-05', 'http://example.com/vehicle5.jpg', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'SN123461', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'CR-V'), (SELECT station_id FROM "Stations" WHERE name = 'Central Hub'), 'AVAILABLE', NULL, NULL, NULL, 'Green', '2025-09-06', 'http://example.com/vehicle6.jpg', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'SN123462', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'F-150'), (SELECT station_id FROM "Stations" WHERE name = 'Airport Station'), 'MAINTENANCE', NULL, NULL, NULL, 'Blue', '2025-09-07', 'http://example.com/vehicle7.jpg', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'SN123463', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Sienna'), (SELECT station_id FROM "Stations" WHERE name = 'Suburban Stop'), 'AVAILABLE', NULL, NULL, NULL, 'White', '2025-09-08', 'http://example.com/vehicle8.jpg', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'SN123464', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Ninja'), (SELECT station_id FROM "Stations" WHERE name = 'City Park'), 'AVAILABLE', NULL, NULL, NULL, 'Black', '2025-09-09', 'http://example.com/vehicle9.jpg', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'SN123465', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Mustang'), (SELECT station_id FROM "Stations" WHERE name = 'Mall Station'), 'AVAILABLE', NULL, NULL, NULL, 'Red', '2025-09-10', 'http://example.com/vehicle10.jpg', CURRENT_TIMESTAMP, TRUE);

-- Chèn 10 bản ghi vào bảng Promotions
INSERT INTO "Promotions" (promotion_id, promo_code, discount_percentage, start_date, end_date, created_at, isActive) VALUES
(gen_random_uuid(), 'SAVE10', 10.00, '2025-10-01 00:00:00', '2025-10-31 23:59:59', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'WELCOME20', 20.00, '2025-10-01 00:00:00', '2025-10-15 23:59:59', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'WEEKEND15', 15.00, '2025-10-03 00:00:00', '2025-10-05 23:59:59', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'FIRST50', 50.00, '2025-10-01 00:00:00', '2025-10-10 23:59:59', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'SUMMER25', 25.00, '2025-10-15 00:00:00', '2025-10-30 23:59:59', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'LOYALTY10', 10.00, '2025-10-01 00:00:00', '2025-12-31 23:59:59', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'FLASH30', 30.00, '2025-10-12 00:00:00', '2025-10-13 23:59:59', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'NEWUSER15', 15.00, '2025-10-01 00:00:00', '2025-10-20 23:59:59', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'RENTAL20', 20.00, '2025-10-05 00:00:00', '2025-10-25 23:59:59', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'SAVE50', 50.00, '2025-10-10 00:00:00', '2025-10-12 23:59:59', CURRENT_TIMESTAMP, TRUE);

-- Chèn 10 bản ghi vào bảng Orders (đã sửa, sử dụng 'ONGOING' thay vì 'PROCESSING')
INSERT INTO "Orders" (order_id, customer_id, vehicle_id, order_date, start_time, end_time, total_price, status, promotion_id, staff_id, created_at, isActive) VALUES
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'john_doe'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123456'), '2025-10-12 10:00:00', '2025-10-12 12:00:00', '2025-10-12 14:00:00', 30.00, 'COMPLETED', (SELECT promotion_id FROM "Promotions" WHERE promo_code = 'SAVE10'), (SELECT account_id FROM "Accounts" WHERE username = 'bob_jones'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'jane_smith'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123457'), '2025-10-12 11:00:00', '2025-10-12 13:00:00', '2025-10-12 15:00:00', 29.00, 'PENDING', NULL, (SELECT account_id FROM "Accounts" WHERE username = 'carol_white'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'alice_wong'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123458'), '2025-10-12 09:00:00', '2025-10-12 10:00:00', '2025-10-12 12:00:00', 50.00, 'ONGOING', (SELECT promotion_id FROM "Promotions" WHERE promo_code = 'WELCOME20'), (SELECT account_id FROM "Accounts" WHERE username = 'david_brown'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'john_doe'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123459'), '2025-10-12 08:00:00', '2025-10-12 09:00:00', '2025-10-12 11:00:00', 44.00, 'COMPLETED', NULL, (SELECT account_id FROM "Accounts" WHERE username = 'bob_jones'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'jane_smith'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123460'), '2025-10-12 14:00:00', '2025-10-12 15:00:00', '2025-10-12 17:00:00', 40.00, 'PENDING', (SELECT promotion_id FROM "Promotions" WHERE promo_code = 'FLASH30'), (SELECT account_id FROM "Accounts" WHERE username = 'carol_white'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'alice_wong'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123461'), '2025-10-12 12:00:00', '2025-10-12 13:00:00', '2025-10-12 15:00:00', 39.00, 'ONGOING', NULL, (SELECT account_id FROM "Accounts" WHERE username = 'david_brown'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'john_doe'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123462'), '2025-10-12 07:00:00', '2025-10-12 08:00:00', '2025-10-12 10:00:00', 60.00, 'COMPLETED', (SELECT promotion_id FROM "Promotions" WHERE promo_code = 'SAVE50'), (SELECT account_id FROM "Accounts" WHERE username = 'bob_jones'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'jane_smith'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123463'), '2025-10-12 16:00:00', '2025-10-12 17:00:00', '2025-10-12 19:00:00', 56.00, 'PENDING', NULL, (SELECT account_id FROM "Accounts" WHERE username = 'carol_white'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'alice_wong'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123464'), '2025-10-12 10:00:00', '2025-10-12 11:00:00', '2025-10-12 13:00:00', 20.00, 'ONGOING', (SELECT promotion_id FROM "Promotions" WHERE promo_code = 'NEWUSER15'), (SELECT account_id FROM "Accounts" WHERE username = 'david_brown'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'john_doe'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123465'), '2025-10-12 13:00:00', '2025-10-12 14:00:00', '2025-10-12 16:00:00', 36.00, 'COMPLETED', NULL, (SELECT account_id FROM "Accounts" WHERE username = 'bob_jones'), CURRENT_TIMESTAMP, TRUE);

-- Chèn 10 bản ghi vào bảng Payments
INSERT INTO "Payments" (payment_id, order_id, amount, payment_date, payment_method, status, created_at, isActive) VALUES
(gen_random_uuid(), (SELECT order_id FROM "Orders" WHERE total_price = 30.00 LIMIT 1), 30.00, '2025-10-12 14:30:00', 'Credit Card', 'SUCCESS', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT order_id FROM "Orders" WHERE total_price = 29.00 LIMIT 1), 29.00, '2025-10-12 15:30:00', 'PayPal', 'PENDING', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT order_id FROM "Orders" WHERE total_price = 50.00 LIMIT 1), 50.00, '2025-10-12 12:30:00', 'Credit Card', 'SUCCESS', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT order_id FROM "Orders" WHERE total_price = 44.00 LIMIT 1), 44.00, '2025-10-12 11:30:00', 'Bank Transfer', 'SUCCESS', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT order_id FROM "Orders" WHERE total_price = 40.00 LIMIT 1), 40.00, '2025-10-12 17:30:00', 'Credit Card', 'PENDING', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT order_id FROM "Orders" WHERE total_price = 39.00 LIMIT 1), 39.00, '2025-10-12 15:30:00', 'PayPal', 'SUCCESS', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT order_id FROM "Orders" WHERE total_price = 60.00 LIMIT 1), 60.00, '2025-10-12 10:30:00', 'Credit Card', 'SUCCESS', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT order_id FROM "Orders" WHERE total_price = 56.00 LIMIT 1), 56.00, '2025-10-12 19:30:00', 'Bank Transfer', 'PENDING', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT order_id FROM "Orders" WHERE total_price = 20.00 LIMIT 1), 20.00, '2025-10-12 13:30:00', 'Credit Card', 'SUCCESS', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT order_id FROM "Orders" WHERE total_price = 36.00 LIMIT 1), 36.00, '2025-10-12 16:30:00', 'PayPal', 'SUCCESS', CURRENT_TIMESTAMP, TRUE);

-- Chèn 10 bản ghi vào bảng Feedbacks
INSERT INTO "Feedbacks" (feedback_id, customer_id, order_id, rating, comment, feedback_date, created_at, isActive) VALUES
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'john_doe'), (SELECT order_id FROM "Orders" WHERE total_price = 30.00 LIMIT 1), 5, 'Great service!', '2025-10-12 14:45:00', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'jane_smith'), (SELECT order_id FROM "Orders" WHERE total_price = 29.00 LIMIT 1), 4, 'Good but slow', '2025-10-12 15:45:00', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'alice_wong'), (SELECT order_id FROM "Orders" WHERE total_price = 50.00 LIMIT 1), 5, 'Amazing vehicle!', '2025-10-12 12:45:00', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'john_doe'), (SELECT order_id FROM "Orders" WHERE total_price = 44.00 LIMIT 1), 3, 'Battery was low', '2025-10-12 11:45:00', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'jane_smith'), (SELECT order_id FROM "Orders" WHERE total_price = 40.00 LIMIT 1), 4, 'Comfortable ride', '2025-10-12 17:45:00', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'alice_wong'), (SELECT order_id FROM "Orders" WHERE total_price = 39.00 LIMIT 1), 5, 'Very clean', '2025-10-12 15:45:00', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'john_doe'), (SELECT order_id FROM "Orders" WHERE total_price = 60.00 LIMIT 1), 4, 'Good truck', '2025-10-12 10:45:00', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'jane_smith'), (SELECT order_id FROM "Orders" WHERE total_price = 56.00 LIMIT 1), 3, 'Late delivery', '2025-10-12 19:45:00', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'alice_wong'), (SELECT order_id FROM "Orders" WHERE total_price = 20.00 LIMIT 1), 5, 'Fun motorcycle!', '2025-10-12 13:45:00', CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'john_doe'), (SELECT order_id FROM "Orders" WHERE total_price = 36.00 LIMIT 1), 4, 'Nice car', '2025-10-12 16:45:00', CURRENT_TIMESTAMP, TRUE);

-- Chèn 10 bản ghi vào bảng Reports
INSERT INTO "Reports" (report_id, report_type, generated_date, text, account_id, vehicle_id, created_at, isActive) VALUES
(gen_random_uuid(), 'Maintenance', '2025-10-12 09:00:00', 'Vehicle needs tire replacement', (SELECT account_id FROM "Accounts" WHERE username = 'bob_jones'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123456'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Incident', '2025-10-12 10:00:00', 'Minor scratch reported', (SELECT account_id FROM "Accounts" WHERE username = 'carol_white'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123457'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Usage', '2025-10-12 11:00:00', 'High battery usage', (SELECT account_id FROM "Accounts" WHERE username = 'david_brown'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123458'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Maintenance', '2025-10-12 12:00:00', 'Battery check required', (SELECT account_id FROM "Accounts" WHERE username = 'bob_jones'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123459'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Incident', '2025-10-12 13:00:00', 'Bumper damage', (SELECT account_id FROM "Accounts" WHERE username = 'carol_white'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123460'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Usage', '2025-10-12 14:00:00', 'Low fuel efficiency', (SELECT account_id FROM "Accounts" WHERE username = 'david_brown'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123461'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Maintenance', '2025-10-12 15:00:00', 'Brake inspection needed', (SELECT account_id FROM "Accounts" WHERE username = 'bob_jones'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123462'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Incident', '2025-10-12 16:00:00', 'Window crack reported', (SELECT account_id FROM "Accounts" WHERE username = 'carol_white'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123463'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Usage', '2025-10-12 17:00:00', 'High mileage recorded', (SELECT account_id FROM "Accounts" WHERE username = 'david_brown'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123464'), CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), 'Maintenance', '2025-10-12 18:00:00', 'Oil change required', (SELECT account_id FROM "Accounts" WHERE username = 'bob_jones'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN123465'), CURRENT_TIMESTAMP, TRUE);

-- Chèn 10 bản ghi vào bảng Staff_Revenues
INSERT INTO "Staff_Revenues" (staff_revenue_id, staff_id, revenue_date, total_revenue, commission, created_at, isActive) VALUES
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'bob_jones'), '2025-10-12 00:00:00', 500.00, 50.00, CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'carol_white'), '2025-10-12 00:00:00', 450.00, 45.00, CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'david_brown'), '2025-10-12 00:00:00', 600.00, 60.00, CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'bob_jones'), '2025-10-11 00:00:00', 400.00, 40.00, CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'carol_white'), '2025-10-11 00:00:00', 300.00, 30.00, CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'david_brown'), '2025-10-11 00:00:00', 550.00, 55.00, CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'bob_jones'), '2025-10-10 00:00:00', 350.00, 35.00, CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'carol_white'), '2025-10-10 00:00:00', 400.00, 40.00, CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'david_brown'), '2025-10-10 00:00:00', 500.00, 50.00, CURRENT_TIMESTAMP, TRUE),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'bob_jones'), '2025-10-09 00:00:00', 450.00, 45.00, CURRENT_TIMESTAMP, TRUE);

