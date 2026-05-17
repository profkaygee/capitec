# ⚙️ CapitecFraud.Worker

## “Background brain”

The `CapitecFraud.Worker` is the **asynchronous processing engine** of the system.

It runs continuously in the background, consuming events from message queues and executing fraud-related processing without blocking the API or user-facing systems.

This layer acts as the **background brain of the fraud detection system**.

---

## 🎯 Purpose

The `CapitecFraud.Worker` is responsible for:

- Consuming messages from queues (e.g. RabbitMQ)
- Executing fraud evaluation workflows
- Running the Fraud Engine asynchronously
- Persisting fraud decisions
- Triggering downstream notifications

It enables the system to be:

- Scalable
- Event-driven
- Non-blocking
- Highly responsive

---

## 🧠 Core Responsibility

This layer answers:

> “What should happen in the background after a transaction or event is received?”

It does NOT:

- Expose APIs
- Handle user requests
- Contain UI logic
- Replace Application or Domain logic

Instead, it orchestrates **background processing workflows** using Application and Domain layers.

---

## 🧩 Project Structure

### 🟡 Queue Consumers

Responsible for listening to message brokers (e.g. RabbitMQ, Kafka).

Examples:

- `TransactionConsumer`
- `FraudEvaluationConsumer`
- `RiskScoringConsumer`

### Responsibilities:

- Listening to incoming messages
- Deserializing events
- Passing data to Application layer
- Handling retries and failures

---

### 🔵 Fraud Engine Execution

The Worker triggers the **Fraud Engine indirectly via Application layer use cases**.

Responsibilities:

- Execute fraud evaluation in background
- Process batch or streaming transactions
- Apply domain rules through orchestration layer

Example flow:
- Message received → Use case executed → Fraud engine evaluates → Decision produced

---

### 🟣 Persistence (Save Results)

After processing, results are stored via Application/Infrastructure layers.

Examples of persisted data:

- Fraud decisions
- Risk scores
- Transaction evaluation results
- Audit logs

Responsibilities:

- Ensuring data consistency
- Writing results to SQL Server
- Updating cache if required

---

### 🟢 Notification Triggers

The Worker is responsible for **triggering downstream notifications** after processing.

Examples:

- SignalR updates (via API layer integration)
- Email alerts (if configured)
- Push notifications
- Webhook triggers

Example events:

- `FraudDetected`
- `TransactionBlocked`
- `RiskThresholdExceeded`

---

## ⚙️ Worker Flow

A typical background processing flow:

1. Event is published to queue (e.g. RabbitMQ)
2. Worker consumes event
3. Message is deserialized
4. Application Use Case is executed
5. Fraud Engine evaluates transaction
6. Result is persisted
7. Notifications are triggered

---

## 📡 Event-Driven Architecture

The Worker enables **event-driven processing**:

- Decouples API from heavy processing
- Allows asynchronous fraud evaluation
- Supports high-throughput transaction streams
- Improves system resilience

---

## 🧱 Design Principles

- **Asynchronous First** – all processing happens off the main thread
- **Stateless Workers** – no dependency on local memory state
- **Resilient Processing** – retry and error handling built-in
- **Idempotent Operations** – safe to reprocess messages
- **Separation of Concerns** – no business rules inside worker itself

---

## 🔄 Failure Handling

The Worker must handle:

- Message retries
- Dead-letter queues (DLQ)
- Partial failures
- Idempotent reprocessing

Example strategy:

- Retry transient failures (network, DB)
- Send poison messages to DLQ
- Log and audit all failures

---

## 🧪 Testing Strategy

The Worker is tested using:

- Integration tests with in-memory queue
- Mocked Application layer
- Message replay scenarios
- Failure simulation tests

Focus areas:

- Message consumption correctness
- Processing accuracy
- Retry behavior
- Persistence validation

---

## 📊 Example Processing Flow

1. Transaction event arrives in queue
2. Worker consumes message
3. Fraud evaluation use case is executed
4. Domain rules are applied
5. Decision is generated (APPROVE / REVIEW / DECLINE)
6. Result is saved to database
7. Notification is triggered to API/SignalR

---

## 🔐 Design Boundaries

The Worker MUST NOT:

- Expose HTTP endpoints
- Contain fraud business logic
- Replace Application or Domain logic
- Directly implement rule evaluation logic

It ONLY:

- Consumes messages
- Orchestrates workflows
- Calls Application layer
- Persists results
- Triggers notifications

---

## 📌 Summary

The `CapitecFraud.Worker` is the **background intelligence layer** of the system.

It ensures:

- Continuous fraud processing
- Asynchronous execution at scale
- Reliable event consumption
- Fast system responsiveness

It is the **silent engine that keeps fraud detection running in real time behind the scenes**.

---