# 🟢 CapitecFraud.Application

## “What should the system do?”

The `CapitecFraud.Application` layer defines the **use cases of the system**.  
It acts as the **orchestration layer** between external inputs (API, messaging, UI) and the core domain logic.

It does NOT contain business rules — instead, it defines **how the system executes business workflows** using the domain layer.

---

## 🎯 Purpose

The `CapitecFraud.Application` layer is responsible for:

- Coordinating fraud detection workflows
- Defining application use cases (e.g. Evaluate Transaction)
- Exposing interfaces (ports) for external systems
- Transforming domain results into application responses
- Acting as a bridge between Domain and Infrastructure

---

## 🧠 Core Responsibility

This layer answers the question:

> “How does the system perform fraud detection end-to-end?”

It does NOT decide what fraud is — that is the Domain’s responsibility.

Instead, it:

- Receives requests
- Calls domain services / fraud engine
- Applies orchestration logic
- Returns structured results

---

## 🧩 Project Structure

### 🔵 Interfaces (Ports)

Interfaces define **abstractions for external dependencies**.

Examples:

- `IFraudEngine`
- `ITransactionRepository`
- `IRiskScoringService`
- `IAuditLogger`

These interfaces:
- Live in the Application layer
- Are implemented in Infrastructure
- Keep the system loosely coupled

---

### 🟡 Use Cases

Use cases represent **application actions** (business workflows).

Each use case:
- Has a single responsibility
- Is triggered by an external request (API, message, job)
- Coordinates domain execution

Examples:

- `EvaluateTransactionUseCase`
- `CreateFraudCaseUseCase`
- `ReevaluateRiskUseCase`
- `GetFraudDecisionHistoryUseCase`

### Example flow:
1. Receive request
2. Load required data
3. Call Fraud.Domain engine
4. Persist results (via interface)
5. Return response DTO

---

### 🟣 Contracts

Contracts define **data structures used by the application layer**.

They include:

#### Request DTOs
- Input models for use cases

Example:
- `EvaluateTransactionRequest`

#### Response DTOs
- Output models returned to external layers

Example:
- `FraudDecisionResponse`

#### Internal Application Models
- Mapping models between domain and external systems

---

## ⚙️ Application Workflow

A typical fraud evaluation flow:

1. External system sends transaction request
2. Application layer receives request
3. `EvaluateTransactionUseCase` is executed
4. Domain Fraud Engine evaluates rules
5. Application aggregates results
6. Decision is stored and returned

---

## 🔄 Dependency Direction

The Application layer depends on:

- ✅ Fraud.Domain
- ❌ Fraud.Infrastructure (never directly)
- ❌ UI / API layers

Instead, it defines interfaces that Infrastructure implements.

---

## 🧪 Testing Strategy

The Application layer is fully testable using:

- Fake implementations of interfaces
- Mock Fraud Engine
- In-memory repositories

### Test focus:

- Use case correctness
- Workflow orchestration
- Response mapping
- Error handling

---

## 🧱 Design Principles

- **Use Case Driven** – each class represents a business action
- **Thin Layer** – no business rules inside application layer
- **Interface First** – dependencies are always abstracted
- **Framework Independent** – no API, DB, or UI logic
- **Testable by Design**

---

## 📌 Summary

The `CapitecFraud.Application` layer defines **what the system does**.

It ensures:

- Clear separation between orchestration and business rules
- Clean communication between layers
- Scalable and maintainable use case structure

It is the **workflow engine of the system**, while the Domain is the **decision engine**.

---