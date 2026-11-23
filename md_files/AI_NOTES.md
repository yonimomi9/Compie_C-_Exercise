# AI_NOTES

## Tools
I used Claude Code inside VS Code. It worked as a tutor, and when I asked it to, it also wrote code. I did not use other AI tools.

## What I used it for
- **Setup.** Creating the solution, the App and Tests projects, the .NET `.gitignore`, the target framework and the git flow.
- **Code review.** I read `ChannelManager` and found the four problems myself. The AI only told me which kind of problems to look for. It wrote the final README text from what I found.
- **Queues.** I wrote `IMessageQueue` and `SimulatorQueue`. I asked the AI to write `DatqOptions` and `DatqQueue`, and then asked it to explain how they work.
- **DI and config.** The AI gave me the `Program.cs` code. I pasted and ran it, and when it failed because of the working directory, I tested both ways of running until we found the cause (`ContentRootPath`).
- **Session, Audit, MessageSender, tests.** I asked to see the real project code in every step instead of general examples, so most of this code was written by the AI. I ran it, broke one test on purpose to see it fail, fixed my own build errors, and reorganized the folders.
- **Part B.** The AI asked me questions and I answered them step by step. When I got stuck it wrote the English text from my answers.

## Where I disagreed with the AI or changed its suggestions
1. **How to learn.** At first I wanted a predict / break / explain routine in every step. That learning rate was too slow for me, so I moved it to the end of the project. I also asked it to hault comparisons to other languages.
2. **Return type of `Send`.** I first wanted `string` so the caller would get a message. After we talked about how the caller uses it, I changed to `bool`. The downside is that it does not say why a send failed.
3. **State machine.** The AI recommended an enum with a transition table because it is shorter. I chose the State Pattern, one class per state, because each state keeps its own rules and adding `Reconnecting` did not touch the other classes. The downside is that there is no single table to look at.
4. **Messages in `Suspended`.** The AI recommended dropping the message and returning `false`. I did not want to lose messages in a bank, so I chose to hold them and send them in order when the session is `Active` again. I accepted the risks: they are lost if the process crashes, they can be old by then, and the order must be kept.
5. **.NET version.** The AI first used `net8.0` like the PDF. My team told me to use `net10.0`, so I changed it.
6. **Answer 2.** The first version said a duplicate key means "skip and continue". Then I understood that the unique key only protects the table and not the bank's queue, so I added a status column to the answer.
7. **Not done.** I did not add `namespace` to the files or a null check in `DatqQueue.Send`, because I decided they were not needed for this task.

## What I can explain without AI
- Why `IMessageQueue` and `IAudit` are interfaces, and how `Queue:Type` chooses the queue.
- Why `SessionState` is an abstract class and why `State` has a private setter.
- Why `_sequence++` is not safe without a `lock`.
- How `MessageSender` holds messages in `Suspended` and sends them when the state becomes `Active`.
- The ideas in `ANSWERS.md`: sequence number as the marker, unique key, status column, local buffer.
