# SETUP GUIDE AFTER CLONE / RECEIVE PROJECT

และใน Solution มี:

* FoodApi
* CustomerApp
* RestaurantApp

ให้ทำตามนี้เพื่อเปิดใช้งานโปรเจกต์

---

# STEP 1 — ติดตั้งโปรแกรมที่จำเป็น

## ต้องมี

### 1. Visual Studio 2022 / 2026

ตอนติดตั้งให้เลือก workload:

* ASP.NET and web development
* .NET desktop development

---

### 2. PostgreSQL

ดาวน์โหลด:
https://www.postgresql.org/download/

ตอนติดตั้ง:

* Port = 5432
* จำ Password ของ postgres user ไว้

---

### 3. pgAdmin 4

ใช้จัดการ Database

---

# STEP 2 — CREATE DATABASE

เปิด pgAdmin 4

---

## เปิด Query Tool

คลิก:

Databases
→ Right Click
→ Query Tool

---

# STEP 3 — CREATE USER

รัน:

```sql id="1"
CREATE ROLE fooduser LOGIN PASSWORD '1234';
```

---

# STEP 4 — CREATE DATABASE

รันอีกครั้งแยกจากอันบน:

```sql id="2"
CREATE DATABASE fooddb OWNER fooduser;
```

IMPORTANT:
ห้ามรัน CREATE ROLE กับ CREATE DATABASE พร้อมกัน

---

# STEP 5 — CREATE TABLE

เข้า:

fooddb
→ Schemas
→ public
→ Tables

เปิด Query Tool แล้วรัน:

```sql id="3"
CREATE TABLE "Orders"
(
    "Id" SERIAL PRIMARY KEY,
    "CustomerName" TEXT NOT NULL,
    "RestaurantId" INTEGER NOT NULL,
    "Status" TEXT NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "Items" TEXT NOT NULL DEFAULT '',
    "Total" INTEGER NOT NULL DEFAULT 0
);
```

---

# STEP 6 — OPEN PROJECT

เปิดไฟล์:

```text id="4"
FoodOrderingSystem.sln
```

ใน Visual Studio

---

# STEP 7 — RESTORE NUGET PACKAGES

Visual Studio ส่วนใหญ่จะ Restore อัตโนมัติ

ถ้าไม่โหลด package ให้:

คลิกขวา Solution
→ Restore NuGet Packages

---

# STEP 8 — INSTALL PACKAGES (ถ้ายังไม่ครบ)

## FoodApi

เปิด:

Tools
→ NuGet Package Manager
→ Manage NuGet Packages

ติดตั้ง:

```text id="5"
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.Design
Npgsql.EntityFrameworkCore.PostgreSQL
Swashbuckle.AspNetCore
Microsoft.AspNetCore.SignalR
```

---

## RestaurantApp

ติดตั้ง:

```text id="6"
Microsoft.AspNetCore.SignalR.Client
```

---

# STEP 9 — CHECK appsettings.json

ใน FoodApi

เปิด:

```text id="7"
appsettings.json
```

ต้องเป็น:

```json id="8"
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=fooddb;Username=fooduser;Password=1234"
  }
}
```

---

# STEP 10 — BUILD PROJECT

กด:

```text id="9"
Ctrl + Shift + B
```

ถ้าไม่มี Error = ใช้งานได้

---

# STEP 11 — RUN MULTIPLE PROJECTS

คลิกขวา Solution
→ Configure Startup Projects

เลือก:

```text id="10"
Multiple startup projects
```

ตั้งค่า:

* FoodApi = Start
* CustomerApp = Start
* RestaurantApp = Start

---

# STEP 12 — RUN

กด:

```text id="11"
F5
```

จะเปิด:

* Swagger
* CustomerApp
* RestaurantApp

พร้อมกัน

---

# TEST SYSTEM

## CustomerApp

1. เลือกอาหาร
2. กด Add >>
3. กด ORDER

---

# RESULT

## RestaurantApp

จะแสดง Order Realtime เช่น:

```text id="12"
Order #1
Customer: Customer
Items: Burger, Pizza
Total: 328
```

---

# COMMON ERRORS

## ERROR:

column "Items" does not exist

ให้รันใน pgAdmin:

```sql id="13"
ALTER TABLE "Orders"
ADD COLUMN "Items" text NOT NULL DEFAULT '';

ALTER TABLE "Orders"
ADD COLUMN "Total" integer NOT NULL DEFAULT 0;
```

---

# IMPORTANT

ถ้า RestaurantApp ไม่ขึ้น Order:

ให้ปิดทุกโปรเจกต์ แล้วกด F5 ใหม่ทั้งหมด

เพราะ SignalR อาจค้าง session เก่า

---
