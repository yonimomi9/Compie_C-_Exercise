### AI_NOTES

**Tools**
I used Claude Code inside VS Code as a mentor while working on this assignment. I also used the regular Claude chat to help me write the prompts and instructions I gave to Claude Code.

**How I worked with it**
I worked step by step, one file at a time. For each step I asked it to show me the actual project code and explain it briefly. Then I typed the code into the project myself, built it, ran it and fixed the errors I got. In some places I asked it to write a file directly, and I list those below.

**What I used it for**

*Setup:* creating the solution, the App and Tests projects, the .gitignore, the target framework and the git flow (a branch, a commit per phase, and push).

*Code review:* I read ChannelManager and found the four problems myself. The AI only pointed me to the kinds of problems to look for, and the README text was written from what I found.

*Queues:* I wrote IMessageQueue and SimulatorQueue. I asked the AI to write DatqOptions and DatqQueue, and then asked it to walk me through how they work.

*DI and configuration:* the AI gave me the Program.cs code. When it failed because of the working directory, I tried running it from different folders until we found the cause (ContentRootPath), and then fixed it.

*Session, Audit, MessageSender and tests:* we discussed the design first, and then the AI wrote most of this code at my request. I ran the tests, broke one of them on purpose to see it fail, fixed my own build errors, and reorganized the folders.

*Part B:* the AI asked me guiding questions and I answered them. It then helped me phrase the English text of the answers from what I said, and I edited it.

**Where I disagreed with the AI or changed its suggestions**
1. Learning approach. I decided to do the predict / break / explain exercises at the end of the project instead of in every step, and I asked it not to compare C# to other languages.
2. Return type of Send. I first wanted string so the caller would get a message. After we discussed how the caller uses it, I chose bool. The downside is that it does not say why a send failed.
3. State machine. The AI recommended an enum with a transition table because it is shorter. I chose the State Pattern, one class per state, because each state keeps its own rules and adding Reconnecting did not require touching the other states. The downside is that there is no single table to look at.
4. Messages in Suspended. The AI recommended dropping the message and returning false. I did not want to lose messages in a banking system, so I chose to hold them and send them in order when the session is Active again. I accepted the risks: they are lost if the process crashes, they can be old by the time they are sent, and the order has to be kept.
5. Answer 2. My first version treated a duplicate key as "skip and continue". Then I realized the unique key only protects the table and not the bank's queue, so I added a status column to the answer.
6. Things I decided not to add. I did not add namespaces to the files or a null check in DatqQueue.Send, because I judged they were not needed for this task.
