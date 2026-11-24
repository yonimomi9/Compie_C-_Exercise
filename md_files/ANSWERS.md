## Part B - Questions:
### Question 1:
#### Answer:
After a restart occurs, the component needs to know the sequence number of the last message that was processed successfully. It then continues from the sequence number that follows the last message and asks the exchange to send messages from there.
This number is stored in the database, not in memory. Memory is wiped when the process crashes and restarts, so we would lose track of where we stopped.
A message counts as "processed successfully" only after it was written to both destinations: the bank's queue and the database table. In practice, the marker is the highest sequence number whose row is marked as sent.

In my implementation, "FileAudit" and "InMemoryAudit" keep their sequence counter ("_sequence" variable) in memory. After a restart, "FileAudit" starts counting from 1 again, even though the audit file already contains earlier entries. So my audit counter cannot be used to resume processing.

### Question 2:
#### Answer:
After the restart, the saved marker still points to the previous message, so the component asks the exchange to continue from message N and tries to process it again. Without protection it creates a duplicate row in the table, and a second copy of the message in the bank's queue.

To prevent duplicates in the table, the sequence number column has a unique index. The database rejects a second insert of the same sequence number. The component treats this rejection as "already stored", not as a fatal error. If it crashed on it, it would crash again on every restart and never move on to the next message.

The unique constraint protects only the table, not the bank's queue. Therefore, each row also has a status column, for example "sent to bank". The component writes the row first with status "not sent", then sends to the queue, then updates the status to "sent". After a restart, rows that are "not sent" are sent again and rows that are "sent" are skipped. This way no message is lost.

A small window remains: a crash after sending but before updating the status causes one repeated send. Two separate systems cannot guarantee that a message was written exactly once without help, so I prefer a duplicate message over a lost message. This relies on an assumption: the bank's side ignores a message whose sequence number has already been received.

### Question 3:
#### Answer:
The question is whether we should keep sending messages to the bank's queue while the database is down, and what happens to the messages that were not written to the database.

Yes, we should keep sending, if we can do it safely. The bank's queue is the main purpose of this component, and the database is a record of what was sent. Stopping for a few minutes could delay messages that the bank needs on time.
Safely means that before sending each message to the bank, we save it to a local file on disk (a buffer), in sequential order.

The messages that weren't written into the database will wait in the local file. A background process keeps trying to write them to the database, waiting a bit longer after each failure. When the database is back, they are written in sequential order.

If the component crashes during the outage, the file is still there, so nothing is lost. After the restart, the database still shows the last message that was stored, so we continue from there. Some messages may be sent to the bank twice, so (as in question 2) we assume the bank ignores a sequence number it already received. The unique constraint on the sequence number also protects the database from duplicates.

However, there are limitations. At 1,000 messages per second, five minutes are approximately 300,000 messages, so the file needs a size limit. If the limit is reached, or if we cannot write to the disk, the component should stop reading from the exchange until the database is back. A delay is better than losing messages.

In every case the component should send an alert, so people know that the database is down.