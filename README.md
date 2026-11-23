# Compie_C#_Exercise
C#/.NET Exercise - Compie course

## Code review: legacy `ChannelManager`

1. **`if/else` chain on `QueueType` (coupling).** `Send` knows every queue type. Adding a new type means editing existing code, and with ~30 methods in the same style a single change is scattered across the class and can break working code.
2. **Hard-coded connection details.** The server name (`AS400PRD`), user and password are written in the source, so they end up in git history forever, can't differ per environment, and a local test can hit the production server.
3. **`State` is a `string`.** Comparisons like `State != "Active"` are exact and case-sensitive, so a typo (`"Activ"`, `"active"`) compiles fine and silently changes behavior. There is also no check that a transition is allowed.
4. **Silent failure in `Send`.** When the state isn't `"Active"` the method just `return`s: no exception, no return value, no log. The caller believes the message was sent, and it is lost without anyone knowing.
