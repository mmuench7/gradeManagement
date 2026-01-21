This guide explains how to set up and run the project locally so it works end-to-end.

---

## Prerequisites

- Local **MySQL** server running
- A MySQL client (MySQL Workbench / DBeaver / CLI)
- The project solution (API + whatever else is in the repo)
- Access to these files:
  - `db1` (SQL script)
  - `db2` (SQL script)
  - `template1` (JSON/body templates for creating principals)
  - `template2` (JSON/body templates for registering teachers)

---

**Disclaimer**: The data provided in this guide is dummy data only and does not reflect reality or actual processes at GIBZ.

---

## Step-by-step setup

### 1) Run `db1` on your local MySQL server
1. Open your MySQL client.
2. Execute the SQL script **`db1`** completely.

Result: base database schema exists locally.

---

### 2) Download the solution and open it
1. Download / clone the solution.
2. Open it in your IDE (e.g., Visual Studio).

---

### 3) Update the database connection string in the API
1. In the API project, open:
   - `appsettings.json`
2. Adjust the database connection string so it points to your **local MySQL** database.

---

### 4) Start the API
1. Run the API project.
2. Open Swagger (should happen automatically).

---

### 5) Create the 4 principals via Swagger (dev endpoint)
1. In Swagger, open the **dev endpoint** for creating principals.
2. Create all **4** principals using the payloads from **`template1`**.

This password is used for all accounts:
**Password:** `Init1234!`

---

### 6) Register teachers via Swagger
1. In Swagger, open the endpoint for **teacher registration**.
2. Register all teachers using the payloads from **`template2`**.

Use this password for all accounts again:
**Password:** `Init1234!`

---

### 7) Run `db2`
1. Go back to your MySQL client.
2. Execute the SQL script **`db2`** completely.

---

## Ready ✅

After these steps the system is fully operational.

---

## Showcase examples

- Requests from **Christian Lindauer** go to **Werner Odermatt**
- Requests from **Peter Gisler** go to **Alex Kobel**
