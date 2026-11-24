# Compie C# Exercise

C#/.NET home assignment for Compie: a Foundation layer that sends messages to a queue (a simulated DATQ or an in-memory simulator), with a session state machine, an audit and unit tests.

## Code review of the legacy ChannelManager

1. If/else chain on QueueType. Send knows every queue type, so adding a new type means editing existing code. With about 30 methods in the same style, one change gets scattered across the class and can break working code.

2. Hard-coded connection details. The server name (AS400PRD), the user and the password are written in the source. They stay in the git history forever, they can't be different per environment, and a local test can end up writing to the production server.

3. State is a string. A comparison like State != "Active" is exact and case-sensitive, so a typo such as "Activ" or "active" compiles fine and silently changes the behavior. Nothing checks that a transition is allowed either.

4. Silent failure in Send. When the state is not "Active", the method just returns. There is no exception, no return value and no log, so the caller thinks the message was sent while it was lost and nobody knows.

## Abstract class or interface

An interface is a pure contract. It says what a type can do, with no shared code and no state. I use it when very different classes need to be replaceable behind the same contract, or when one class needs to implement several contracts (C# allows one base class but many interfaces).

An abstract class is a partial implementation. It can hold shared code, default behavior and fields, and it forces the subclasses to fill in the missing parts. I use it when the types are closely related ("is a") and share real behavior.

Examples from this project:

IMessageQueue and IAudit are interfaces. DatqQueue, SimulatorQueue, FileAudit and InMemoryAudit have nothing in common except the contract. This is what lets the tests replace the real queue and the real audit with in-memory versions, and lets Queue:Type choose the implementation.

SessionState is an abstract class. All the states share a default (CanSend is false) and each one must implement CanTransitionTo. Only ActiveState overrides CanSend, so the rule "send only in Active" is written in one place.

My rule of thumb: start with an interface, and use an abstract class only when there is shared code worth inheriting.

## How to run

The project targets net10.0. The PDF mentions .NET 8, but my team told me to use .NET 10.

From the solution folder:

dotnet build
dotnet test
dotnet run --project QueueFoundation.App

The app reads appsettings.json from its own output folder, so it works from any working directory. The demo in Program.cs sends a message while the session is Disconnected (it is rejected), moves the session to Active, and sends again (it goes through).

## Switching the queue type

Change Queue:Type in QueueFoundation.App/appsettings.json. "Sim" uses the in-memory SimulatorQueue. "Datq" uses DatqQueue, which only simulates the connection by printing to the console. The server, user and password come from the Datq section of the same file, and they are empty in the repository.

Any other value stops the app at startup with an exception, so a typo can't silently send nothing.

## How I implemented the Session states

I used the State Pattern. SessionState is an abstract class, and every state is its own class that knows which states it can move to. Session.TransitionTo asks the current state, and throws InvalidOperationException if the move is not allowed, leaving the state unchanged.

I chose it over an enum with a transition table because each state keeps its own rules in one place, and adding a state doesn't require touching the others. The downside is that the rules are spread over several files, and there is no single table to look at.

## Adding Reconnecting

I changed 3 places: a new file ReconnectingState.cs (it can move to LoggingIn or Disconnected), and one line each in ActiveState and SuspendedState to allow the move to Reconnecting. Session and MessageSender did not change at all, which is the reason I chose the State Pattern. The tests cover one legal move (Active to Reconnecting) and one forbidden move (Reconnecting to Active).

## Sending while Suspended

A message sent while the session is Suspended is held in memory, and it is sent in order when the session becomes Active again. Send returns false for these messages, because they were not sent yet. In a banking system I prefer a delayed message to a lost one, so I did not choose to drop it or to throw.

The downsides:

1. The held messages exist only in memory, so they are lost if the process crashes.
2. A message can be old by the time it is sent.
3. The order has to be kept, so the held messages are sent before any new message.
4. The buffer has no size limit.

A test checks that the messages are held and then sent in the right order when the session goes back to Active.

## What I did not do, or would do differently

1. FileAudit starts its sequence number from 1 on every run, so it can't be used to resume after a restart (see question 1 in ANSWERS.md). A real version would read the last number from the file or from the database. Also, the audit.log path is relative to the working directory.
2. MessageSender checks State is SuspendedState. It would be cleaner to have a property on the state, for example "holds messages", so the sender doesn't need to know a specific state class.
3. Send returns only a bool, so the caller can't tell why a send failed.
4. I kept the code simple: no namespaces and no null checks in the queue classes.

## Time spent

About X hours.
