# 🟡 CapitecFraud.Domain

## “What is fraud?”

Fraud is any intentional act of deception designed to gain an unfair or illegal advantage.  
In this system, the Fraud Domain represents the **core business rules and logic used to detect, evaluate, and classify fraudulent activity**.

It is the **decision-making heart** of the fraud detection system.

---

## 🎯 Purpose

The `CapitecFraud.Domain` layer is responsible for:

- Defining what constitutes fraud in the system
- Evaluating transactions and user behavior against fraud rules
- Producing fraud decisions (Approve, Flag, Reject)
- Providing a deterministic and testable fraud engine

It is completely **framework-agnostic** and does NOT depend on:

- Databases (SQL Server, Redis, etc.)
- Messaging systems (Kafka, RabbitMQ)
- APIs or UI layers
- External services

---

## 🧠 Core Responsibilities

The Fraud Domain answers questions like:

- Is this transaction suspicious?
- Does this behavior violate fraud rules?
- What is the risk score of this event?
- Should this action be blocked, flagged, or approved?

---

## 🧩 Domain Structure

### 🟣 Entities

Entities represent core fraud-related business objects with identity and behavior.

Examples:

- `Transaction`
- `Account`
- `Customer`
- `FraudCase`

Entities are responsible for:
- Holding state
- Enforcing invariants
- Triggering domain behavior

---

### 🟡 Rules

Rules define **how fraud is detected**.

They are:
- Stateless
- Independent
- Fully testable
- Composable

Examples:

- Transaction velocity rules
- Daily limit violations
- Geographic anomaly detection
- High-risk merchant rules

Each rule returns a clear result:
- ✔ Pass
- ⚠ Flag
- ❌ Fail

---

### 🔵 Fraud Engine

The Fraud Engine is the **orchestrator of all rules**.

It is responsible for:

- Receiving a transaction or event
- Running it through all fraud rules
- Aggregating results
- Calculating risk score
- Producing a final fraud decision

### Output decisions:

- ✅ APPROVE → Transaction is safe
- ⚠ REVIEW → Requires manual inspection
- ❌ DECLINE → Confirmed fraud risk

---

## ⚙️ Fraud Evaluation Flow

1. A transaction/event enters the system
2. The Fraud Engine loads relevant rules
3. Each rule evaluates independently
4. Results are aggregated
5. Risk score is calculated
6. Final decision is returned

---

## 📦 Design Principles

- **Deterministic** – same input always produces same output
- **Stateless Rules** – rules do not retain memory
- **Extensible** – new fraud rules can be added without breaking existing ones
- **Testable** – every rule can be unit tested in isolation
- **Separation of Concerns** – engine, rules, and entities are isolated

---

## 🧪 Testing Strategy

The Fraud Domain is fully unit-testable.

Recommended approach:

- Test each rule independently
- Test fraud engine aggregation logic
- Simulate transaction scenarios
- Validate decision outputs

Example:

- High-value transaction → flagged
- Rapid repeated transactions → rejected
- Normal behavior → approved

---

## 🚨 Domain Philosophy

This domain does NOT try to “guess” fraud.

Instead, it:
- Applies explicit business rules
- Produces explainable outcomes
- Keeps decisions transparent and auditable

---

## 📌 Summary

The `CapitecFraud.Domain` is the **core intelligence layer** of the fraud detection system.

It ensures that every decision is:

- Consistent
- Explainable
- Testable
- Business-driven

---