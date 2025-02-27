# 🏨 Hotel Management System (Client)

The **Hotel Management System Client** is a web-based application designed to facilitate hotel bookings, room management, and customer interactions. It provides a seamless experience for both guests and administrators, ensuring efficient hotel operations.

---

## 🌟 Features

- **Room Booking & Availability** – Users can browse available rooms and make bookings.
- **User Authentication** – Secure login and registration for customers and hotel staff.
- **Payments & Billing** – Integration with **MPesa** and **Stripe** for seamless transactions.
- **Booking Management** – View, modify, or cancel reservations.
- **Admin Dashboard** – Manage rooms, pricing, and customer inquiries.
- **Notifications** – Email confirmations and reminders for bookings.

---

## 🏗️ Architecture

The system follows a **modular architecture**, ensuring scalability and maintainability.

### 📌 Frontend (Client)

- **Framework:** Angular
- **State Management:** Zustand / Redux
- **UI Components:** Tailwind CSS / Material UI
- **API Communication:** HTTP Client with interceptors for authentication
- **Authentication:** NextAuth.js
- **Payment Integration:** MPesa & Stripe

### 📌 Backend (Server)

The **Hotel Management System Client** interacts with a **.NET 9 backend**, which handles business logic, database operations, and payment processing.

Backend key components include:

- **.NET 9 Web API** – Handles requests from the frontend.
- **Database:** PostgreSQL
- **Concurrency Handling:** RabbitMQ & MediatR for booking management.
- **Authentication & Security:** JWT-based authentication.

---

## 🛠️ Installation & Setup

### Prerequisites

Ensure you have the following installed:

- **Node.js** (LTS version recommended)
- **Angular CLI**
- **Package Manager** (npm or yarn)

### Steps to Run Locally

1. **Clone the repository:**
   ```bash
   git clone https://github.com/musigahstallone/hotelmanagementsystem-client.git
   cd hotelmanagementsystem-client
   ```  

2. **Install dependencies:**
   ```bash
   npm install
   ```  

3. **Start the development server:**
   ```bash
   ng serve
   ```  

4. Open your browser and visit **`http://localhost:4200/`**

---

## 📚 API Integration

The frontend communicates with the backend via RESTful APIs. Some key API endpoints include:

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/auth/login` | POST | User authentication |
| `/api/bookings` | GET | Retrieve all bookings |
| `/api/bookings/{id}` | GET | Get booking details |
| `/api/payments/mpesa` | POST | Process MPesa payment |
| `/api/payments/stripe` | POST | Process Stripe payment |

---

## 🚀 Deployment

### Build for Production
To create an optimized production build, run:

```bash
ng build --prod
```  

The output files will be stored in the `dist/` directory.

### Hosting Options
The frontend can be deployed to:
- **Vercel / Netlify** (for static hosting)
- **NGINX / Apache** (for self-hosting)
- **Cloud Providers** (AWS, Azure, Firebase Hosting)

---

## 📝 License

This project is licensed under the **MIT License**.
