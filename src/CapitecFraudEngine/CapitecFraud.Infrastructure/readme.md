# 🟣 CapitecFraud.Infrastructure

## “How it works”

The `CapitecFraud.Infrastructure` layer is responsible for all **external system integrations and technical implementations**.

It contains the concrete implementations of interfaces defined in the `Fraud.Application` layer and provides access to:

- Databases
- Caching systems
- Message brokers
- External services

This layer is **purely technical** and contains **no business rules**.

---

## 🎯 Purpose

The `CapitecFraud.Infrastructure` layer handles:

- Data persistence (SQL Server via EF Core)
- Caching (Redis)
- Messaging (RabbitMQ)
- Repository implementations
- External system communication

It acts as the **bridge between the application and the outside world**.

---

## 🧠 Core Responsibility

This layer answers:

> “How is data stored, retrieved, and communicated externally?”

It does NOT decide:
- What is fraud
- How fraud rules work
- What the system should do

Those responsibilities belong to:
- `Fraud.Domain` (business rules)
- `Fraud.Application` (use cases)

---

## 🧩 Project Structure

### 🟡 SQL Server (EF Core)

Responsible for persistent storage of system data.

Includes:
- DbContext
- Entity configurations
- Migrations
- Database mappings

Typical responsibilities:
- Storing transactions
- Storing fraud decisions
- Persisting audit logs

---

### 🔵 Redis (Caching Layer)

Used for **high-performance data access**.

Common use cases:
- Fraud rule caching
- Transaction velocity tracking
- Temporary risk scores
- Rate limiting data

Benefits:
- Low latency reads
- Reduces database load
- Supports real-time fraud evaluation

---

### 🟣 RabbitMQ (Messaging Layer)

Handles **asynchronous communication between services**.

Used for:
- Publishing fraud evaluation events
- Processing transaction streams
- Triggering background fraud analysis
- Decoupling system components

Example events:
- `TransactionReceived`
- `FraudDetected`
- `RiskScoreUpdated`

---

### 🟢 Repositories

Repository implementations connect the **Application layer interfaces** to actual data sources.

Examples:

- `TransactionRepository`
- `FraudCaseRepository`
- `CustomerRepository`

Responsibilities:
- Querying SQL Server via EF Core
- Mapping database models to domain entities
- Ensuring data consistency

Repositories follow the **interface contracts defined in Fraud.Application**.

---

## ⚙️ Infrastructure Flow

A typical data flow:

1. Application requests data via interface
2. Infrastructure resolves repository implementation
3. EF Core queries SQL Server
4. Redis cache is checked (if applicable)
5. Data is returned to Application layer
6. Optional events are published via RabbitMQ

---

## 🧱 Design Principles

- **Persistence Ignorant Domain** – Domain never depends on Infrastructure
- **Interface Implementation Only** – Implements Application contracts
- **Replaceable Components** – SQL, Redis, RabbitMQ can be swapped
- **No Business Logic Allowed** – strictly technical concerns only
- **Performance Optimized** – caching and async messaging supported

---

## 🗄️ SQL Server (EF Core)

Responsibilities:

- Database schema mapping
- Entity configurations
- Query optimization
- Migrations management

Key concept:
- EF Core models are NOT domain models
- Mapping is handled at the infrastructure boundary

---

## ⚡ Redis Usage

Redis is used as a **high-speed supporting layer**.

Examples:
- `TransactionVelocity:{AccountId}`
- `RiskScoreCache:{TransactionId}`
- `FraudRuleCache:{RuleId}`

Rules:
- Always treat Redis as **temporary storage**
- Never store critical long-term data only in Redis

---

## 📡 RabbitMQ Messaging

Used for **event-driven architecture support**.

Pattern:
- Publish events after persistence
- Consume events for background processing

Example flow:
- Transaction saved → event published
- Fraud engine processes event asynchronously

Benefits:
- Scalability
- Loose coupling
- Asynchronous processing

---

## 🧪 Testing Strategy

Infrastructure tests focus on:

- EF Core database interactions (integration tests)
- Redis caching behavior
- RabbitMQ message publishing
- Repository correctness

Recommended approach:
- Use Testcontainers for SQL Server / Redis
- Mock external services where needed
- Validate real query execution

---

## 🔐 Design Boundaries

Infrastructure must NEVER:

- Contain business rules
- Make fraud decisions
- Contain application logic
- Directly expose database models to external layers

It ONLY:

- Implements interfaces
- Handles I/O operations
- Manages external systems

---

## 📌 Summary

The `CapitecFraud.Infrastructure` layer defines **how the system works internally with external systems**.

It ensures:

- Reliable data persistence (SQL Server)
- High-speed caching (Redis)
- Scalable messaging (RabbitMQ)
- Clean separation from business logic

It is the **execution engine of the system’s technical operations**.

---