# 🟣 CapitecFraud.Api

## “User interface (HTTP + SignalR)”

The `CapitecFraud.Api` layer is the **entry point of the system**.  
It exposes the fraud detection capabilities to external clients through:

- HTTP REST APIs
- Real-time SignalR communication
- Webhooks (optional extension)
- External integrations (mobile apps, web apps, services)

This layer is responsible for **communication only**, not business logic.

---

## 🎯 Purpose

The `CapitecFraud.Api` layer handles:

- Receiving incoming HTTP requests
- Sending commands to the Application layer
- Returning structured API responses
- Broadcasting real-time fraud updates via SignalR
- Acting as the system’s external interface

---

## 🧠 Core Responsibility

This layer answers:

> “How do users and systems interact with fraud detection in real time?”

It does NOT:

- Apply fraud rules
- Perform business logic
- Access databases directly
- Contain domain or application logic

Instead, it delegates everything to:

- `Fraud.Application` (use cases)
- `Fraud.Domain` (business rules)

---

## 🧩 Project Structure

### 🔵 Controllers (HTTP API)

Controllers expose **REST endpoints** for fraud operations.

Examples:

- `FraudController`
- `TransactionController`
- `RiskController`

### Typical responsibilities:

- Accepting HTTP requests
- Validating request payloads (basic validation only)
- Calling Application use cases
- Returning HTTP responses (200, 400, 500, etc.)

### Example endpoints:

- `POST /api/fraud/evaluate`
- `GET /api/fraud/{transactionId}`
- `GET /api/fraud/history`

---

### 🟣 SignalR Hub (Real-Time Communication)

The SignalR Hub provides **live fraud updates** to connected clients.

Example:

- `FraudHub`

### Responsibilities:

- Broadcasting fraud decisions in real time
- Pushing risk alerts to dashboards
- Notifying clients when transactions are flagged or blocked

### Example events:

- `TransactionEvaluated`
- `FraudDetected`
- `RiskScoreUpdated`

---

### 🟡 Real-Time Notifier Implementation

This component handles **publishing real-time updates from backend to clients**.

It acts as a bridge between:

- Application layer events
- SignalR Hub

### Responsibilities:

- Listening to application/domain events
- Transforming events into DTOs
- Sending messages via SignalR
- Ensuring delivery of real-time notifications

### Example flow:

1. Fraud decision is produced in Application layer
2. Event is published (e.g. `FraudDetectedEvent`)
3. Notifier receives event
4. SignalR Hub broadcasts update to clients

---

## ⚙️ API Workflow

Typical fraud evaluation request:

1. Client sends HTTP request
2. Controller receives request
3. Controller calls Application Use Case
4. Application executes Fraud Engine
5. Result is returned to API layer
6. API responds to client
7. SignalR broadcasts real-time update (if needed)

---

## 📡 Real-Time Architecture

Fraud API supports **event-driven real-time updates**:

- SignalR for live dashboards
- Push notifications for fraud alerts
- Instant risk updates for transactions

This enables:

- Fraud monitoring dashboards
- Live transaction tracking
- Instant alerting systems

---

## 🧱 Design Principles

- **Thin Controllers** – no business logic in API layer
- **Separation of Concerns** – all logic delegated to Application layer
- **Real-Time First Design** – SignalR used for live updates
- **Stateless API** – requests are independent
- **Scalable Communication Layer** – supports horizontal scaling

---

## 🔐 API Boundaries

CapitecFraud.Api MUST NOT:

- Contain business rules
- Access EF Core or Redis directly
- Implement fraud logic
- Replace Application layer responsibilities

It ONLY:

- Handles HTTP requests
- Routes commands to Application layer
- Streams real-time updates via SignalR

---

## 🧪 Testing Strategy

The API layer is tested using:

- Integration tests (HTTP endpoints)
- SignalR hub tests
- Contract testing (request/response validation)

Focus areas:

- Endpoint correctness
- Response formatting
- Authentication & authorization (if applicable)
- Real-time message delivery

---

## 📊 Example Real-Time Flow

1. Transaction is submitted via HTTP
2. Fraud evaluation runs in Application layer
3. Fraud decision is produced
4. SignalR hub broadcasts result:
    - APPROVED
    - FLAGGED
    - DECLINED
5. Client dashboard updates instantly

---

## 📌 Summary

The `CapitecFraud.Api` layer is the **communication gateway of the system**.

It ensures:

- Fast HTTP interaction
- Real-time fraud visibility via SignalR
- Clean separation from business logic
- Scalable external integration layer

It is the **face of the fraud detection system**, while all intelligence remains in the Domain and Application layers.

---