# Compie_C#_Exercise
C#/.NET Exercise - Compie course

## Code review: legacy `ChannelManager`

1. **`if/else` chain on `QueueType` (coupling).** `Send` knows every queue type. Adding a new type means editing existing code, and with ~30 methods in the same style a single change is scattered across the class and can break working code.
2. **Hard-coded connection details.** The server name (`AS400PRD`), user and password are written in the source, so they end up in git history forever, can't differ per environment, and a local test can hit the production server.
3. **`State` is a `string`.** Comparisons like `State != "Active"` are exact and case-sensitive, so a typo (`"Activ"`, `"active"`) compiles fine and silently changes behavior. There is also no check that a transition is allowed.
4. **Silent failure in `Send`.** When the state isn't `"Active"` the method just `return`s: no exception, no return value, no log. The caller believes the message was sent, and it is lost without anyone knowing.

## Part 7: When to use an abstract class and when an interface

An interface is a pure contract: it says what a type can do, with no shared code and no state. Use it when unrelated or very different classes must be replaceable behind the same contract, and when a class may need to implement several contracts (C# allows one base class but many interfaces).

An abstract class is a partial implementation: it can hold shared code, default behavior and fields, and it forces subclasses to fill in the missing parts. Use it when the types are closely related ("is a") and share real behavior.

Examples from this project:

- `IMessageQueue` and `IAudit` are interfaces. `DatqQueue`, `SimulatorQueue`, `FileAudit` and `InMemoryAudit` have nothing in common except the contract. This is what lets the tests replace the real queue and the real audit with in-memory versions, and lets `Queue:Type` choose the implementation.
- `SessionState` is an abstract class. All states share a default (`CanSend` is `false`) and must each implement `CanTransitionTo`. Only `ActiveState` overrides `CanSend`, so the rule "send only in Active" is written in one place.

Rule of thumb: start with an interface; use an abstract class only when there is shared code worth inheriting.
