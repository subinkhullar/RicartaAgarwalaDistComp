# RicartaAgarwalaDistComp
Ricarta Agarwala Distributed Computing Algorithm implemented in C#

Ricart-Agrawala algorithm for distributed mutual exclusion is an algorithm using which multiple processes or nodes, which do not share memory but exchange messages to communicate with another, may enter their respective critical sections without deadlock or starvation. A process sends a request message to all other processes when the sender process requires to enter its critical section. Each request message includes an identification of the sender process and a timestamp value (i.e., the Lamport logical clock value) associated with the request message. On receipt of a request message from the sender process, a recipient process checks certain conditions, based on the information within the request message, and either defers the request or sends a reply message to the original sender process. On receipt of a reply message from each such process to which the request message was sent, the original sender process enters its critical section. Upon exit from its critical section, a process sends reply messages to each process whose request was earlier deferred by the particular process.
