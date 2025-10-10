-- Tạo database nếu chưa tồn tại
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

-- Tạo extension uuid-ossp
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Tạo hàm đặt trường created_at tự động khi INSERT
CREATE OR REPLACE FUNCTION set_created_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.created_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Tạo hàm cập nhật trường updated_at tự động khi UPDATE
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Tạo ENUM types nếu chưa tồn tại
DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_type WHERE typname = 'vehicle_status') THEN
        CREATE TYPE vehicle_status AS ENUM ('AVAILABLE', 'RENTED', 'MAINTENANCE', 'CHARGING');
        RAISE NOTICE 'Tạo kiểu ENUM vehicle_status thành công.';
    ELSE
        RAISE NOTICE 'Kiểu ENUM vehicle_status đã tồn tại.';
    END IF;
END
$$;

DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_type WHERE typname = 'order_status') THEN
        CREATE TYPE order_status AS ENUM ('PENDING', 'CONFIRMED', 'ONGOING', 'COMPLETED', 'CANCELED');
        RAISE NOTICE 'Tạo kiểu ENUM order_status thành công.';
    ELSE
        RAISE NOTICE 'Kiểu ENUM order_status đã tồn tại.';
    END IF;
END
$$;

-- Xóa các bảng nếu đã tồn tại, theo thứ tự phụ thuộc để tránh lỗi khóa ngoại
DROP TABLE IF EXISTS "Staff_Revenues" CASCADE;
DROP TABLE IF EXISTS "Reports" CASCADE;
DROP TABLE IF EXISTS "Feedbacks" CASCADE;
DROP TABLE IF EXISTS "Payments" CASCADE;
DROP TABLE IF EXISTS "Contracts" CASCADE;
DROP TABLE IF EXISTS "Orders" CASCADE;
DROP TABLE IF EXISTS "Promotions" CASCADE;
DROP TABLE IF EXISTS "Vehicles" CASCADE;
DROP TABLE IF EXISTS "Stations" CASCADE;
DROP TABLE IF EXISTS "VehicleModels" CASCADE;
DROP TABLE IF EXISTS "VehicleTypes" CASCADE;
DROP TABLE IF EXISTS "Licenses" CASCADE;
DROP TABLE IF EXISTS "Account_Roles" CASCADE;
DROP TABLE IF EXISTS "Roles" CASCADE;
DROP TABLE IF EXISTS "Accounts" CASCADE;

-- Tạo bảng Accounts
CREATE TABLE "Accounts" (
  account_id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  username varchar NOT NULL,
  password varchar NOT NULL,
  email varchar NOT NULL,
  contact_number varchar,
  created_at timestamp NOT NULL,
  updated_at timestamp,
  isActive boolean NOT NULL DEFAULT true
);

-- Tạo bảng Roles
CREATE TABLE "Roles" (
  role_id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  role_name varchar NOT NULL,
  created_at timestamp NOT NULL,
  updated_at timestamp,
  isActive boolean NOT NULL DEFAULT true
);

-- Tạo bảng Account_Roles
CREATE TABLE "Account_Roles" (
  account_role_id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  account_id uuid NOT NULL,
  role_id uuid NOT NULL,
  created_at timestamp NOT NULL,
  updated_at timestamp,
  isActive boolean NOT NULL DEFAULT true
);

-- Tạo bảng Licenses
CREATE TABLE "Licenses" (
  license_id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  account_id uuid NOT NULL,
  license_image_url varchar NOT NULL,
  identity_card_image_url varchar NOT NULL,
  created_at timestamp NOT NULL,
  updated_at timestamp,
  isActive boolean NOT NULL DEFAULT true
);

-- Tạo bảng VehicleTypes
CREATE TABLE "VehicleTypes" (
  vehicle_type_id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  type_name varchar UNIQUE NOT NULL,
  description text,
  created_at timestamp NOT NULL,
  updated_at timestamp,
  isActive boolean NOT NULL DEFAULT true
);

-- Tạo bảng VehicleModels
CREATE TABLE "VehicleModels" (
  vehicle_model_id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  type_id uuid NOT NULL,
  name varchar NOT NULL,
  manufacturer varchar NOT NULL,
  price_per_hour decimal NOT NULL,
  specs jsonb,
  created_at timestamp NOT NULL,
  updated_at timestamp,
  isActive boolean NOT NULL DEFAULT true
);

-- Tạo bảng Vehicles
CREATE TABLE "Vehicles" (
  vehicle_id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  serial_number varchar UNIQUE NOT NULL,
  model_id uuid NOT NULL,
  station_id uuid,
  status vehicle_status NOT NULL DEFAULT 'AVAILABLE',
  battery_level int,
  battery_capacity int,
  range int,
  color varchar,
  last_maintenance date,
  img varchar,
  location_lat decimal,
  location_long decimal,
  created_at timestamp NOT NULL,
  updated_at timestamp,
  isActive boolean NOT NULL DEFAULT true
);

-- Tạo bảng Stations
CREATE TABLE "Stations" (
  station_id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  name varchar NOT NULL,
  address varchar NOT NULL,
  lat decimal NOT NULL,
  long decimal NOT NULL,
  capacity int NOT NULL,
  image_url varchar,
  created_at timestamp NOT NULL,
  updated_at timestamp,
  isActive boolean NOT NULL DEFAULT true
);

-- Tạo bảng Promotions
CREATE TABLE "Promotions" (
  promotion_id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  promo_code varchar NOT NULL,
  discount_percentage decimal NOT NULL,
  start_date timestamp NOT NULL,
  end_date timestamp NOT NULL,
  station_id uuid,
  created_at timestamp NOT NULL,
  updated_at timestamp,
  isActive boolean NOT NULL DEFAULT true
);

-- Tạo bảng Orders
CREATE TABLE "Orders" (
  order_id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  customer_id uuid NOT NULL,
  vehicle_id uuid NOT NULL,
  order_date timestamp NOT NULL,
  start_time timestamp NOT NULL,
  end_time timestamp,
  total_price decimal NOT NULL,
  status order_status NOT NULL DEFAULT 'PENDING',
  promotion_id uuid,
  staff_id uuid NOT NULL,
  created_at timestamp NOT NULL,
  updated_at timestamp,
  isActive boolean NOT NULL DEFAULT true
);

-- Tạo bảng Contracts
CREATE TABLE "Contracts" (
  contract_id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  order_id uuid NOT NULL,
  contract_date timestamp NOT NULL,
  terms text NOT NULL,
  signature varchar,
  status varchar NOT NULL,
  created_at timestamp NOT NULL,
  updated_at timestamp,
  isActive boolean NOT NULL DEFAULT true
);

-- Tạo bảng Payments
CREATE TABLE "Payments" (
  payment_id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  order_id uuid NOT NULL,
  amount decimal NOT NULL,
  payment_date timestamp NOT NULL,
  payment_method varchar NOT NULL,
  status varchar NOT NULL,
  created_at timestamp NOT NULL,
  updated_at timestamp,
  isActive boolean NOT NULL DEFAULT true
);

-- Tạo bảng Feedbacks
CREATE TABLE "Feedbacks" (
  feedback_id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  customer_id uuid NOT NULL,
  order_id uuid NOT NULL,
  rating int NOT NULL,
  comment text,
  feedback_date timestamp NOT NULL,
  created_at timestamp NOT NULL,
  updated_at timestamp,
  isActive boolean NOT NULL DEFAULT true
);

-- Tạo bảng Reports
CREATE TABLE "Reports" (
  report_id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  report_type varchar NOT NULL,
  generated_date timestamp NOT NULL,
  data json NOT NULL,
  account_id uuid NOT NULL,
  created_at timestamp NOT NULL,
  updated_at timestamp,
  isActive boolean NOT NULL DEFAULT true
);

-- Tạo bảng Staff_Revenues
CREATE TABLE "Staff_Revenues" (
  staff_revenue_id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  staff_id uuid NOT NULL,
  revenue_date timestamp NOT NULL,
  total_revenue decimal NOT NULL,
  commission decimal,
  created_at timestamp NOT NULL,
  updated_at timestamp,
  isActive boolean NOT NULL DEFAULT true
);

-- Thêm foreign keys
ALTER TABLE "Account_Roles" ADD FOREIGN KEY (account_id) REFERENCES "Accounts" (account_id);
ALTER TABLE "Account_Roles" ADD FOREIGN KEY (role_id) REFERENCES "Roles" (role_id);
ALTER TABLE "Licenses" ADD FOREIGN KEY (account_id) REFERENCES "Accounts" (account_id);
ALTER TABLE "VehicleModels" ADD FOREIGN KEY (type_id) REFERENCES "VehicleTypes" (vehicle_type_id);
ALTER TABLE "Vehicles" ADD FOREIGN KEY (model_id) REFERENCES "VehicleModels" (vehicle_model_id);
ALTER TABLE "Vehicles" ADD FOREIGN KEY (station_id) REFERENCES "Stations" (station_id);
ALTER TABLE "Promotions" ADD FOREIGN KEY (station_id) REFERENCES "Stations" (station_id);
ALTER TABLE "Orders" ADD FOREIGN KEY (customer_id) REFERENCES "Accounts" (account_id);
ALTER TABLE "Orders" ADD FOREIGN KEY (vehicle_id) REFERENCES "Vehicles" (vehicle_id);
ALTER TABLE "Orders" ADD FOREIGN KEY (promotion_id) REFERENCES "Promotions" (promotion_id);
ALTER TABLE "Orders" ADD FOREIGN KEY (staff_id) REFERENCES "Accounts" (account_id);
ALTER TABLE "Contracts" ADD FOREIGN KEY (order_id) REFERENCES "Orders" (order_id);
ALTER TABLE "Payments" ADD FOREIGN KEY (order_id) REFERENCES "Orders" (order_id);
ALTER TABLE "Feedbacks" ADD FOREIGN KEY (customer_id) REFERENCES "Accounts" (account_id);
ALTER TABLE "Feedbacks" ADD FOREIGN KEY (order_id) REFERENCES "Orders" (order_id);
ALTER TABLE "Reports" ADD FOREIGN KEY (account_id) REFERENCES "Accounts" (account_id);
ALTER TABLE "Staff_Revenues" ADD FOREIGN KEY (staff_id) REFERENCES "Accounts" (account_id);

-- Thêm trigger cập nhật created_at
CREATE TRIGGER set_accounts_created_at BEFORE INSERT ON "Accounts" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_roles_created_at BEFORE INSERT ON "Roles" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_account_roles_created_at BEFORE INSERT ON "Account_Roles" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_licenses_created_at BEFORE INSERT ON "Licenses" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_vehicle_types_created_at BEFORE INSERT ON "VehicleTypes" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_vehicle_models_created_at BEFORE INSERT ON "VehicleModels" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_vehicles_created_at BEFORE INSERT ON "Vehicles" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_stations_created_at BEFORE INSERT ON "Stations" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_promotions_created_at BEFORE INSERT ON "Promotions" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_orders_created_at BEFORE INSERT ON "Orders" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_contracts_created_at BEFORE INSERT ON "Contracts" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_payments_created_at BEFORE INSERT ON "Payments" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_feedbacks_created_at BEFORE INSERT ON "Feedbacks" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_reports_created_at BEFORE INSERT ON "Reports" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();
CREATE TRIGGER set_staff_revenues_created_at BEFORE INSERT ON "Staff_Revenues" FOR EACH ROW EXECUTE FUNCTION set_created_at_column();

-- Thêm trigger cập nhật updated_at
CREATE TRIGGER update_accounts_updated_at BEFORE UPDATE ON "Accounts" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_roles_updated_at BEFORE UPDATE ON "Roles" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_account_roles_updated_at BEFORE UPDATE ON "Account_Roles" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_licenses_updated_at BEFORE UPDATE ON "Licenses" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_vehicle_types_updated_at BEFORE UPDATE ON "VehicleTypes" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_vehicle_models_updated_at BEFORE UPDATE ON "VehicleModels" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_vehicles_updated_at BEFORE UPDATE ON "Vehicles" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_stations_updated_at BEFORE UPDATE ON "Stations" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_promotions_updated_at BEFORE UPDATE ON "Promotions" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_orders_updated_at BEFORE UPDATE ON "Orders" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_contracts_updated_at BEFORE UPDATE ON "Contracts" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_payments_updated_at BEFORE UPDATE ON "Payments" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_feedbacks_updated_at BEFORE UPDATE ON "Feedbacks" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_reports_updated_at BEFORE UPDATE ON "Reports" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_staff_revenues_updated_at BEFORE UPDATE ON "Staff_Revenues" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- Chèn dữ liệu vào bảng Roles
INSERT INTO "Roles" (role_id, role_name, isActive) VALUES
(gen_random_uuid(), 'Admin', true),
(gen_random_uuid(), 'Manager', true),
(gen_random_uuid(), 'Staff', true),
(gen_random_uuid(), 'Customer', true),
(gen_random_uuid(), 'Driver', true),
(gen_random_uuid(), 'Technician', true),
(gen_random_uuid(), 'Supervisor', true),
(gen_random_uuid(), 'Guest', true),
(gen_random_uuid(), 'VIP Customer', true),
(gen_random_uuid(), 'Maintenance', true);

-- Chèn dữ liệu vào bảng Accounts
INSERT INTO "Accounts" (account_id, username, password, email, contact_number, isActive) VALUES
(gen_random_uuid(), 'user1', 'pass1', 'user1@email.com', '0123456789', true),
(gen_random_uuid(), 'user2', 'pass2', 'user2@email.com', '0987654321', true),
(gen_random_uuid(), 'staff1', 'pass3', 'staff1@email.com', '0111222333', true),
(gen_random_uuid(), 'staff2', 'pass4', 'staff2@email.com', '0444555666', true),
(gen_random_uuid(), 'customer1', 'pass5', 'customer1@email.com', '0777888999', true),
(gen_random_uuid(), 'customer2', 'pass6', 'customer2@email.com', '0001112222', true),
(gen_random_uuid(), 'admin1', 'pass7', 'admin1@email.com', '3334445555', true),
(gen_random_uuid(), 'manager1', 'pass8', 'manager1@email.com', '6667778888', true),
(gen_random_uuid(), 'user9', 'pass9', 'user9@email.com', '9990001111', true),
(gen_random_uuid(), 'user10', 'pass10', 'user10@email.com', '2223334444', true),
(gen_random_uuid(), 'staff3', 'pass11', 'staff3@email.com', '5556667777', true),
(gen_random_uuid(), 'customer3', 'pass12', 'customer3@email.com', '8889990000', true);

-- Chèn dữ liệu vào bảng Account_Roles
INSERT INTO "Account_Roles" (account_role_id, account_id, role_id, isActive) VALUES
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user1'), (SELECT role_id FROM "Roles" WHERE role_name = 'Admin'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user2'), (SELECT role_id FROM "Roles" WHERE role_name = 'Customer'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'staff1'), (SELECT role_id FROM "Roles" WHERE role_name = 'Staff'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'staff2'), (SELECT role_id FROM "Roles" WHERE role_name = 'Manager'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer1'), (SELECT role_id FROM "Roles" WHERE role_name = 'Customer'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer2'), (SELECT role_id FROM "Roles" WHERE role_name = 'VIP Customer'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'admin1'), (SELECT role_id FROM "Roles" WHERE role_name = 'Admin'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'manager1'), (SELECT role_id FROM "Roles" WHERE role_name = 'Supervisor'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user9'), (SELECT role_id FROM "Roles" WHERE role_name = 'Customer'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user10'), (SELECT role_id FROM "Roles" WHERE role_name = 'Technician'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'staff3'), (SELECT role_id FROM "Roles" WHERE role_name = 'Staff'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer3'), (SELECT role_id FROM "Roles" WHERE role_name = 'Customer'), true);

-- Chèn dữ liệu vào bảng Licenses
INSERT INTO "Licenses" (license_id, account_id, license_image_url, identity_card_image_url, isActive) VALUES
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user1'), '/images/license1.jpg', '/images/id1.jpg', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user2'), '/images/license2.jpg', '/images/id2.jpg', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer1'), '/images/license3.jpg', '/images/id3.jpg', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer2'), '/images/license4.jpg', '/images/id4.jpg', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user9'), '/images/license5.jpg', '/images/id5.jpg', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user10'), '/images/license6.jpg', '/images/id6.jpg', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer3'), '/images/license7.jpg', '/images/id7.jpg', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'staff1'), '/images/license8.jpg', '/images/id8.jpg', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'staff2'), '/images/license9.jpg', '/images/id9.jpg', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'staff3'), '/images/license10.jpg', '/images/id10.jpg', true);

-- Chèn dữ liệu vào bảng VehicleTypes
INSERT INTO "VehicleTypes" (vehicle_type_id, type_name, description, isActive) VALUES
(gen_random_uuid(), 'Scooter', 'Electric scooter for short trips', true),
(gen_random_uuid(), 'Bike', 'Electric bike for urban commuting', true),
(gen_random_uuid(), 'Car', 'Small electric car', true),
(gen_random_uuid(), 'Moped', 'Electric moped', true),
(gen_random_uuid(), 'Quad', 'Electric quad bike', true),
(gen_random_uuid(), 'Hoverboard', 'Self-balancing hoverboard', true),
(gen_random_uuid(), 'Segway', 'Personal transporter', true),
(gen_random_uuid(), 'E-Bike', 'Enhanced electric bike', true),
(gen_random_uuid(), 'Scooter Pro', 'Professional scooter', true),
(gen_random_uuid(), 'Mini Car', 'Mini electric vehicle', true);

-- Chèn dữ liệu vào bảng VehicleModels
INSERT INTO "VehicleModels" (vehicle_model_id, type_id, name, manufacturer, price_per_hour, specs, isActive) VALUES
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Scooter'), 'Scoot1', 'Xiaomi', 5.00, '{"speed": 25, "range": 30}', true),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Scooter'), 'Scoot2', 'Segway', 6.00, '{"speed": 30, "range": 35}', true),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Bike'), 'Bike1', 'Giant', 4.00, '{"speed": 20, "range": 40}', true),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Bike'), 'Bike2', 'Trek', 4.50, '{"speed": 25, "range": 45}', true),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Car'), 'Car1', 'Tesla', 20.00, '{"speed": 100, "range": 300}', true),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Car'), 'Car2', 'Nissan', 18.00, '{"speed": 90, "range": 250}', true),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Moped'), 'Moped1', 'Honda', 7.00, '{"speed": 40, "range": 50}', true),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Moped'), 'Moped2', 'Yamaha', 8.00, '{"speed": 45, "range": 55}', true),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Quad'), 'Quad1', 'Polaris', 12.00, '{"speed": 50, "range": 60}', true),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Hoverboard'), 'Hover1', 'Segway', 3.00, '{"speed": 15, "range": 20}', true),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'Segway'), 'Segway1', 'Segway', 10.00, '{"speed": 20, "range": 25}', true),
(gen_random_uuid(), (SELECT vehicle_type_id FROM "VehicleTypes" WHERE type_name = 'E-Bike'), 'EBike1', 'Specialized', 5.50, '{"speed": 28, "range": 50}', true);

-- Chèn dữ liệu vào bảng Stations
INSERT INTO "Stations" (station_id, name, address, lat, long, capacity, image_url, isActive) VALUES
(gen_random_uuid(), 'Station A', '123 Main St, District 1', 10.7626, 106.6602, 20, '/images/station_a.jpg', true),
(gen_random_uuid(), 'Station B', '456 Oak Ave, District 3', 10.7726, 106.6702, 15, '/images/station_b.jpg', true),
(gen_random_uuid(), 'Station C', '789 Pine Rd, District 5', 10.7826, 106.6802, 25, '/images/station_c.jpg', true),
(gen_random_uuid(), 'Station D', '101 Elm St, District 7', 10.7926, 106.6902, 18, '/images/station_d.jpg', true),
(gen_random_uuid(), 'Station E', '202 Birch Ln, Binh Thanh', 10.8026, 106.7002, 22, '/images/station_e.jpg', true),
(gen_random_uuid(), 'Station F', '303 Cedar Dr, Phu Nhuan', 10.8126, 106.7102, 12, '/images/station_f.jpg', true),
(gen_random_uuid(), 'Station G', '404 Maple Blvd, Tan Binh', 10.8226, 106.7202, 30, '/images/station_g.jpg', true),
(gen_random_uuid(), 'Station H', '505 Walnut Way, Go Vap', 10.8326, 106.7302, 16, '/images/station_h.jpg', true),
(gen_random_uuid(), 'Station I', '606 Spruce Pl, Thu Duc', 10.8426, 106.7402, 19, '/images/station_i.jpg', true),
(gen_random_uuid(), 'Station J', '707 Fir Ct, Hoc Mon', 10.8526, 106.7502, 14, '/images/station_j.jpg', true);

-- Chèn dữ liệu vào bảng Vehicles
INSERT INTO "Vehicles" (vehicle_id, serial_number, model_id, station_id, status, battery_level, battery_capacity, range, color, last_maintenance, img, location_lat, location_long, isActive) VALUES
(gen_random_uuid(), 'SN001', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Scoot1'), (SELECT station_id FROM "Stations" WHERE name = 'Station A'), 'AVAILABLE', 90, 100, 30, 'Red', '2025-09-01', '/images/vehicle_sn001.jpg', 10.7626, 106.6602, true),
(gen_random_uuid(), 'SN002', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Scoot2'), (SELECT station_id FROM "Stations" WHERE name = 'Station A'), 'RENTED', 75, 120, 35, 'Blue', '2025-08-15', '/images/vehicle_sn002.jpg', 10.7700, 106.6650, true),
(gen_random_uuid(), 'SN003', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Bike1'), (SELECT station_id FROM "Stations" WHERE name = 'Station B'), 'AVAILABLE', 85, 150, 40, 'Black', '2025-09-10', '/images/vehicle_sn003.jpg', 10.7726, 106.6702, true),
(gen_random_uuid(), 'SN004', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Bike2'), (SELECT station_id FROM "Stations" WHERE name = 'Station B'), 'CHARGING', 100, 150, 45, 'Green', '2025-08-20', '/images/vehicle_sn004.jpg', 10.7726, 106.6702, true),
(gen_random_uuid(), 'SN005', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Car1'), (SELECT station_id FROM "Stations" WHERE name = 'Station C'), 'MAINTENANCE', 60, 500, 300, 'White', '2025-09-05', '/images/vehicle_sn005.jpg', 10.7826, 106.6802, true),
(gen_random_uuid(), 'SN006', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Car2'), (SELECT station_id FROM "Stations" WHERE name = 'Station C'), 'AVAILABLE', 95, 450, 250, 'Silver', '2025-09-15', '/images/vehicle_sn006.jpg', 10.7826, 106.6802, true),
(gen_random_uuid(), 'SN007', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Moped1'), (SELECT station_id FROM "Stations" WHERE name = 'Station D'), 'RENTED', 80, 200, 50, 'Yellow', '2025-08-25', '/images/vehicle_sn007.jpg', 10.7900, 106.6950, true),
(gen_random_uuid(), 'SN008', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Moped2'), (SELECT station_id FROM "Stations" WHERE name = 'Station D'), 'AVAILABLE', 70, 220, 55, 'Red', '2025-09-02', '/images/vehicle_sn008.jpg', 10.7926, 106.6902, true),
(gen_random_uuid(), 'SN009', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Quad1'), (SELECT station_id FROM "Stations" WHERE name = 'Station E'), 'CHARGING', 100, 300, 60, 'Green', '2025-08-30', '/images/vehicle_sn009.jpg', 10.8026, 106.7002, true),
(gen_random_uuid(), 'SN010', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Hover1'), (SELECT station_id FROM "Stations" WHERE name = 'Station E'), 'AVAILABLE', 88, 80, 20, 'Black', '2025-09-08', '/images/vehicle_sn010.jpg', 10.8026, 106.7002, true),
(gen_random_uuid(), 'SN011', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'Segway1'), (SELECT station_id FROM "Stations" WHERE name = 'Station F'), 'MAINTENANCE', 50, 100, 25, 'Blue', '2025-09-12', '/images/vehicle_sn011.jpg', 10.8126, 106.7102, true),
(gen_random_uuid(), 'SN012', (SELECT vehicle_model_id FROM "VehicleModels" WHERE name = 'EBike1'), (SELECT station_id FROM "Stations" WHERE name = 'Station F'), 'RENTED', 65, 150, 50, 'White', '2025-08-10', '/images/vehicle_sn012.jpg', 10.8150, 106.7150, true);

-- Chèn dữ liệu vào bảng Promotions
INSERT INTO "Promotions" (promotion_id, promo_code, discount_percentage, start_date, end_date, station_id, isActive) VALUES
(gen_random_uuid(), 'PROMO1', 10.00, '2025-10-01 00:00:00', '2025-10-31 23:59:59', (SELECT station_id FROM "Stations" WHERE name = 'Station A'), true),
(gen_random_uuid(), 'PROMO2', 15.00, '2025-10-01 00:00:00', '2025-10-15 23:59:59', (SELECT station_id FROM "Stations" WHERE name = 'Station B'), true),
(gen_random_uuid(), 'PROMO3', 20.00, '2025-09-15 00:00:00', '2025-10-04 23:59:59', (SELECT station_id FROM "Stations" WHERE name = 'Station C'), true),
(gen_random_uuid(), 'PROMO4', 5.00, '2025-10-05 00:00:00', '2025-10-31 23:59:59', NULL, true),
(gen_random_uuid(), 'PROMO5', 25.00, '2025-10-01 00:00:00', '2025-10-10 23:59:59', (SELECT station_id FROM "Stations" WHERE name = 'Station D'), true),
(gen_random_uuid(), 'PROMO6', 12.00, '2025-09-20 00:00:00', '2025-10-20 23:59:59', (SELECT station_id FROM "Stations" WHERE name = 'Station E'), true),
(gen_random_uuid(), 'PROMO7', 8.00, '2025-10-01 00:00:00', '2025-12-31 23:59:59', NULL, true),
(gen_random_uuid(), 'PROMO8', 18.00, '2025-10-04 00:00:00', '2025-11-04 23:59:59', (SELECT station_id FROM "Stations" WHERE name = 'Station F'), true),
(gen_random_uuid(), 'PROMO9', 30.00, '2025-10-01 00:00:00', '2025-10-07 23:59:59', (SELECT station_id FROM "Stations" WHERE name = 'Station G'), true),
(gen_random_uuid(), 'PROMO10', 7.00, '2025-09-01 00:00:00', '2025-12-31 23:59:59', NULL, true);

-- Chèn dữ liệu vào bảng Orders
INSERT INTO "Orders" (order_id, customer_id, vehicle_id, order_date, start_time, end_time, total_price, status, promotion_id, staff_id, isActive) VALUES
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user2'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN001'), '2025-10-01 10:00:00', '2025-10-01 11:00:00', '2025-10-01 12:00:00', 5.00, 'COMPLETED', (SELECT promotion_id FROM "Promotions" WHERE promo_code = 'PROMO1'), (SELECT account_id FROM "Accounts" WHERE username = 'staff1'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer1'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN002'), '2025-10-02 14:00:00', '2025-10-02 15:00:00', '2025-10-02 16:00:00', 6.00, 'ONGOING', (SELECT promotion_id FROM "Promotions" WHERE promo_code = 'PROMO2'), (SELECT account_id FROM "Accounts" WHERE username = 'staff2'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer2'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN003'), '2025-10-03 09:00:00', '2025-10-03 10:00:00', '2025-10-03 11:00:00', 4.00, 'CONFIRMED', (SELECT promotion_id FROM "Promotions" WHERE promo_code = 'PROMO3'), (SELECT account_id FROM "Accounts" WHERE username = 'staff1'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user9'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN004'), '2025-10-04 16:00:00', '2025-10-04 17:00:00', NULL, 4.50, 'PENDING', NULL, (SELECT account_id FROM "Accounts" WHERE username = 'staff3'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer3'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN005'), '2025-09-30 12:00:00', '2025-09-30 13:00:00', '2025-09-30 14:00:00', 20.00, 'COMPLETED', (SELECT promotion_id FROM "Promotions" WHERE promo_code = 'PROMO4'), (SELECT account_id FROM "Accounts" WHERE username = 'staff2'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user2'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN006'), '2025-10-01 18:00:00', '2025-10-01 19:00:00', '2025-10-01 20:00:00', 18.00, 'CANCELED', NULL, (SELECT account_id FROM "Accounts" WHERE username = 'staff1'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer1'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN007'), '2025-10-02 08:00:00', '2025-10-02 09:00:00', '2025-10-02 10:00:00', 7.00, 'COMPLETED', (SELECT promotion_id FROM "Promotions" WHERE promo_code = 'PROMO5'), (SELECT account_id FROM "Accounts" WHERE username = 'staff3'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer2'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN008'), '2025-10-03 20:00:00', '2025-10-03 21:00:00', NULL, 8.00, 'ONGOING', (SELECT promotion_id FROM "Promotions" WHERE promo_code = 'PROMO6'), (SELECT account_id FROM "Accounts" WHERE username = 'staff2'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user9'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN009'), '2025-10-04 11:00:00', '2025-10-04 12:00:00', '2025-10-04 13:00:00', 12.00, 'COMPLETED', NULL, (SELECT account_id FROM "Accounts" WHERE username = 'staff1'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer3'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN010'), '2025-09-28 15:00:00', '2025-09-28 16:00:00', '2025-09-28 17:00:00', 3.00, 'CONFIRMED', (SELECT promotion_id FROM "Promotions" WHERE promo_code = 'PROMO7'), (SELECT account_id FROM "Accounts" WHERE username = 'staff3'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user2'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN011'), '2025-10-01 22:00:00', '2025-10-01 23:00:00', NULL, 10.00, 'PENDING', (SELECT promotion_id FROM "Promotions" WHERE promo_code = 'PROMO8'), (SELECT account_id FROM "Accounts" WHERE username = 'staff2'), true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer1'), (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN012'), '2025-10-03 13:00:00', '2025-10-03 14:00:00', '2025-10-03 15:00:00', 5.50, 'COMPLETED', NULL, (SELECT account_id FROM "Accounts" WHERE username = 'staff1'), true);

-- Chèn dữ liệu vào bảng Contracts
INSERT INTO "Contracts" (contract_id, order_id, contract_date, terms, signature, status, isActive) VALUES
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1), '2025-10-01 10:30:00', 'Standard rental terms and conditions', 'sig1', 'Signed', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 1), '2025-10-02 14:30:00', 'Terms with insurance coverage', 'sig2', 'Pending', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 2), '2025-10-03 09:30:00', 'Short trip terms, no liability for minor damages', 'sig3', 'Signed', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 3), '2025-10-04 16:30:00', 'Full terms including full insurance', NULL, 'Draft', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 4), '2025-09-30 12:30:00', 'Long rental terms with extended warranty', 'sig4', 'Signed', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 5), '2025-10-01 18:30:00', 'Canceled terms due to customer request', 'sig5', 'Void', false),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 6), '2025-10-02 08:30:00', 'Premium terms with priority support', 'sig6', 'Signed', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 7), '2025-10-03 20:30:00', 'Ongoing terms for active rental', NULL, 'Active', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 8), '2025-10-04 11:30:00', 'Basic terms for standard use', 'sig7', 'Signed', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 9), '2025-09-28 15:30:00', 'Confirmed terms with deposit', 'sig8', 'Signed', true);

-- Chèn dữ liệu vào bảng Payments
INSERT INTO "Payments" (payment_id, order_id, amount, payment_date, payment_method, status, isActive) VALUES
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1), 5.00, '2025-10-01 12:30:00', 'Credit Card', 'Completed', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 1), 6.00, '2025-10-02 16:30:00', 'PayPal', 'Pending', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 2), 4.00, '2025-10-03 11:30:00', 'Bank Transfer', 'Completed', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 3), 4.50, '2025-10-04 17:30:00', 'Credit Card', 'Failed', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 4), 20.00, '2025-09-30 14:30:00', 'Cash', 'Completed', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 5), 18.00, '2025-10-01 20:30:00', 'PayPal', 'Refunded', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 6), 7.00, '2025-10-02 10:30:00', 'Credit Card', 'Completed', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 7), 8.00, '2025-10-03 21:30:00', 'Bank Transfer', 'Pending', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 8), 12.00, '2025-10-04 13:30:00', 'Cash', 'Completed', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 9), 3.00, '2025-09-28 17:30:00', 'Credit Card', 'Completed', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 10), 10.00, '2025-10-01 23:30:00', 'PayPal', 'Pending', true),
(gen_random_uuid(), (SELECT order_id FROM "Orders" ORDER BY order_date LIMIT 1 OFFSET 11), 5.50, '2025-10-03 15:30:00', 'Bank Transfer', 'Completed', true);

-- Chèn dữ liệu vào bảng Feedbacks
INSERT INTO "Feedbacks" (feedback_id, customer_id, order_id, rating, comment, feedback_date, isActive) VALUES
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user2'), (SELECT order_id FROM "Orders" WHERE vehicle_id = (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN001') AND order_date = '2025-10-01 10:00:00'), 5, 'Great ride! Smooth and fast.', '2025-10-01 13:00:00', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer1'), (SELECT order_id FROM "Orders" WHERE vehicle_id = (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN002') AND order_date = '2025-10-02 14:00:00'), 4, 'Good but battery low after 20km.', '2025-10-02 17:00:00', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer2'), (SELECT order_id FROM "Orders" WHERE vehicle_id = (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN003') AND order_date = '2025-10-03 09:00:00'), 5, 'Excellent service and vehicle condition.', '2025-10-03 12:00:00', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user9'), (SELECT order_id FROM "Orders" WHERE vehicle_id = (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN004') AND order_date = '2025-10-04 16:00:00'), 3, 'Average performance, needs better brakes.', '2025-10-04 18:00:00', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer3'), (SELECT order_id FROM "Orders" WHERE vehicle_id = (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN005') AND order_date = '2025-09-30 12:00:00'), 5, 'Very comfortable for long trips.', '2025-09-30 15:00:00', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user2'), (SELECT order_id FROM "Orders" WHERE vehicle_id = (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN006') AND order_date = '2025-10-01 18:00:00'), 2, 'Poor condition, tires worn out.', '2025-10-01 21:00:00', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer1'), (SELECT order_id FROM "Orders" WHERE vehicle_id = (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN007') AND order_date = '2025-10-02 08:00:00'), 4, 'Smooth ride, good battery life.', '2025-10-02 11:00:00', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer2'), (SELECT order_id FROM "Orders" WHERE vehicle_id = (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN008') AND order_date = '2025-10-03 20:00:00'), 5, 'Highly recommend for daily commute.', '2025-10-03 22:00:00', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'user9'), (SELECT order_id FROM "Orders" WHERE vehicle_id = (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN009') AND order_date = '2025-10-04 11:00:00'), 1, 'Not working well, charging issues.', '2025-10-04 14:00:00', true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'customer3'), (SELECT order_id FROM "Orders" WHERE vehicle_id = (SELECT vehicle_id FROM "Vehicles" WHERE serial_number = 'SN010') AND order_date = '2025-09-28 15:00:00'), 5, 'Perfect for city exploration.', '2025-09-28 18:00:00', true);

-- Chèn dữ liệu vào bảng Reports
INSERT INTO "Reports" (report_id, report_type, generated_date, data, account_id, isActive) VALUES
(gen_random_uuid(), 'Daily Revenue', '2025-10-01 00:00:00', '{"total": 100.00, "orders": 5}', (SELECT account_id FROM "Accounts" WHERE username = 'admin1'), true),
(gen_random_uuid(), 'Vehicle Status', '2025-10-02 00:00:00', '{"available": 10, "rented": 2}', (SELECT account_id FROM "Accounts" WHERE username = 'manager1'), true),
(gen_random_uuid(), 'User Activity', '2025-10-03 00:00:00', '{"logins": 50, "rentals": 8}', (SELECT account_id FROM "Accounts" WHERE username = 'staff1'), true),
(gen_random_uuid(), 'Maintenance Log', '2025-10-04 00:00:00', '{"vehicles": 3, "issues": 1}', (SELECT account_id FROM "Accounts" WHERE username = 'staff2'), true),
(gen_random_uuid(), 'Promotion Usage', '2025-09-30 00:00:00', '{"codes_used": 4, "savings": 50.00}', (SELECT account_id FROM "Accounts" WHERE username = 'admin1'), true),
(gen_random_uuid(), 'Station Capacity', '2025-10-01 00:00:00', '{"full": 1, "empty": 2}', (SELECT account_id FROM "Accounts" WHERE username = 'manager1'), true),
(gen_random_uuid(), 'Feedback Summary', '2025-10-02 00:00:00', '{"avg_rating": 4.2, "count": 10}', (SELECT account_id FROM "Accounts" WHERE username = 'staff1'), true),
(gen_random_uuid(), 'Payment Report', '2025-10-03 00:00:00', '{"total": 200.00, "methods": {"card": 5}}', (SELECT account_id FROM "Accounts" WHERE username = 'staff2'), true),
(gen_random_uuid(), 'Inventory Check', '2025-10-04 00:00:00', '{"vehicles": 12, "types": 5}', (SELECT account_id FROM "Accounts" WHERE username = 'admin1'), true),
(gen_random_uuid(), 'Staff Performance', '2025-09-28 00:00:00', '{"orders_handled": 6, "revenue": 150.00}', (SELECT account_id FROM "Accounts" WHERE username = 'manager1'), true);

-- Chèn dữ liệu vào bảng Staff_Revenues
INSERT INTO "Staff_Revenues" (staff_revenue_id, staff_id, revenue_date, total_revenue, commission, isActive) VALUES
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'staff1'), '2025-10-01 00:00:00', 50.00, 5.00, true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'staff2'), '2025-10-02 00:00:00', 75.00, 7.50, true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'staff3'), '2025-10-03 00:00:00', 30.00, 3.00, true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'staff1'), '2025-10-04 00:00:00', 100.00, 10.00, true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'staff2'), '2025-09-30 00:00:00', 40.00, 4.00, true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'staff3'), '2025-10-01 00:00:00', 60.00, 6.00, true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'staff1'), '2025-10-02 00:00:00', 80.00, 8.00, true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'staff2'), '2025-10-03 00:00:00', 25.00, 2.50, true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'staff3'), '2025-10-04 00:00:00', 90.00, 9.00, true),
(gen_random_uuid(), (SELECT account_id FROM "Accounts" WHERE username = 'staff1'), '2025-09-28 00:00:00', 35.00, 3.50, true);