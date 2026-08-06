# Ashborne

> A text-based role-playing game exploring power, identity, morality, and consequence through a reactive world and simulated human behaviour.

**Ashborne** is a text-based role-playing game and software development project focused on creating interactive experiences that more realistically represent human behaviour. Set in a politically unstable fantasy world inspired by medieval Europe and Shakespearean themes, Ashborne allows players to explore complex ideas such as power, identity, violence, and consequence through their own decisions.

Unlike a traditional scripted RPG, Ashborne is designed around the idea that actions should have persistent consequences. NPCs can remember events, develop emotions and attitudes towards others, and change their behaviour and dialogue based on their experiences.

The project is also a platform for experimenting with software architecture and simulation systems, including a custom behaviour-oriented object architecture, persistent cognitive modelling, dynamic dialogue, and a universal object identification system.

---

## How to Play

Go to halfcomplete.github.io/Ashborne to play!

The best way to experience Ashborne is to interact with its characters and experiment with different choices. Try approaching the same NPC in different ways, revisit previous interactions, and observe how their responses change based on what they remember and how they feel.

---

## Key Features

### Cognitive NPCs

NPCs maintain persistent cognitive states rather than relying solely on scripted dialogue flags.

Their experiences can influence:

* Memories
* Emotions
* Attitudes towards other characters
* Dialogue
* Behaviour
* Future interactions

Memories can strengthen through reinforcement, decay over time, and eventually be forgotten.

---

### Memory & Emotion System

Ashborne's Cognitive System models a layered relationship between events, memories, emotions, and attitudes.

```text
    World Event
         │
         ▼
   Memory Source
         │
         ▼
      Memory
         │
         ├──────► Emotion Modifiers
         │                │
         │                ▼
         │         Emotional State
         │
         ▼
     Attitudes
         │
         ▼
Dialogue & Behaviour
```

The system distinguishes between:

* **Events**: what happened in the world.
* **Memories**: how an NPC remembers and interprets what happened.
* **Emotions**: the transient emotional state produced by accumulated experiences.
* **Attitudes**: longer-term opinions towards specific people.

This separation allows NPC behaviour to emerge from their accumulated experiences rather than being individually scripted.

---

### Behaviour-based Object Component System (BOCS)

Ashborne uses a custom **Behaviour Object Component System (BOCS)** to construct game objects from independent behaviours.

Rather than relying on a rigid class hierarchy, every entity is represented by a `BOCSObject`, with its capabilities determined by the behaviours attached to it.

For example, an NPC and an item are not fundamentally different classes. They are both BOCSObjects with different combinations of behaviours.

```text
BOCSObject
├── Behaviour
├── Behaviour
├── Behaviour
└── Behaviour
```

Behaviours implement capabilities that communicate what an object can do to external systems.

This architecture allows Ashborne's objects to be composed and extended without requiring new concrete classes for every type of entity.

Read more about the BOCS architecture at halfcomplete.github.io.

---

### Dynamic Dialogue

Ashborne uses **Ink** alongside a custom C# integration layer to create dynamic, context-sensitive dialogue (see the official Ink website inklestudios.com/ink).

Dialogue can respond to:

* NPC memories
* Emotional states
* Attitudes
* Player actions
* World state
* Object state
* Other game conditions

The C# integration layer also allows Ink dialogue to communicate with the underlying game systems and trigger external events.

---

### Universal Object Identification

Ashborne uses a two-layer identification system consisting of **Definition IDs** and **Instance IDs**.

**Definition IDs** identify compile-time object templates, while **Instance IDs** uniquely identify individual runtime objects.

```text
 Definition
     │
     ▼
BOCSFactory
     │
     ├──► BOCSObject (Instance ID: A)
     ├──► BOCSObject (Instance ID: B)
     └──► BOCSObject (Instance ID: C)
```

This allows multiple instances of the same object definition to exist independently while providing stable references for systems such as:

* NPC memory
* Dialogue
* World management
* Saving and loading

---

### Saving & Loading

Ashborne supports persistent save data by serialising the state of the game world and reconstructing it when a save is loaded.

The save system works alongside the universal ID system and BOCS architecture to preserve:

* Object state
* Behaviour state
* NPC memories
* Emotional state
* Relationships and attitudes
* World state

---

## Technologies Used

Ashborne is primarily developed using:

* **C#**: core game and engine systems
* **HTML & CSS**: used as the frontend of the website
* **Blazor WASM**: used as a bridge between the frontend HTML and CSS and the backend C#
* **Ink**: dynamic narrative and dialogue
* **JSON**: used for serialisation/deserialisation of save/load data
* **Github Pages**: used to host & deploy the website

The project is intentionally not built using a traditional game engine such as Unity so that the underlying architecture could be designed specifically around the requirements of a reactive, text-based narrative simulation.

---

## Repository Structure


A possible structure is:

```text
AshborneCode/
├── Ashborne/
│   ├── Core/
│   ├── BOCS/
│   ├── Cognitive/
│   ├── Dialogue/
│   ├── World/
│   ├── Saving/
│   └── ...
│
├── Definitions/
│
├── Ink/
│
├── Documentation/
│
├── Tests/
│
└── README.md
```

---

## Getting Started

### Requirements

**[ADD REQUIRED .NET VERSION HERE]**

**[ADD OTHER REQUIREMENTS HERE]**

### Installation

Clone the repository:

```bash
git clone [ADD REPOSITORY URL HERE]
```

Navigate to the project:

```bash
cd AshborneCode
```

**[ADD ANY REQUIRED SETUP STEPS HERE]**

### Running the Project

**To run the project locally (on localhost:xxxx):**

Simply use an IDE of your choice (my preference is Visual Studio) and run the AshborneWASM project on the "release" version.

**To run the project on the web:**

Go to halfcomplete.github.io/Ashborne

---

## Documentation

More detailed technical documentation is available in the following areas:

* **[BOCS Architecture](ADD LINK)**
* **[Cognitive System](ADD LINK)**
* **[Memory & Emotion](ADD LINK)**
* **[Dynamic Dialogue](ADD LINK)**
* **[Universal ID System](ADD LINK)**
* **[Save & Load System](ADD LINK)**
* **[World Building](ADD LINK)**

**[ADD ANY OTHER DOCUMENTATION LINKS HERE]**

---

## Development Status

Ashborne is an ongoing personal project and is currently under active development.

The project is being developed incrementally, with a focus on improving both the playable experience and the underlying architecture.

Current development priorities include:

* **[ADD CURRENT PRIORITY HERE]**
* **[ADD CURRENT PRIORITY HERE]**
* **[ADD CURRENT PRIORITY HERE]**

---

## Project Goals

Ashborne was created with two connected goals.

The first is to create an engaging interactive narrative where players can explore complex themes such as power, identity, morality, and consequence through their own decisions.

The second is to investigate how software systems can represent aspects of human behaviour in interactive environments. The project explores questions such as how memory can persist, how experiences can influence emotion, and how those changes can affect future decisions and relationships.

Although developed as a game, the systems created for Ashborne may have applications beyond entertainment, including educational software, training simulations, and other interactive systems where modelling human behaviour is valuable.

---

## Who am I?

**Eric Hu**

High-school student and software developer in Australia.

Ashborne is a long-term personal project combining my interests in software architecture, simulation, interactive narrative, and game development.

My portfolio: halfcomplete.github.io

**[ADD CONTACT / SOCIAL LINKS HERE, IF DESIRED]**

---

## Project History

Ashborne originally began development in **2025** and was entered into the **2025 YICTE** competition, winning 1st place at the Nationals Finals.

The 2026 version represents a major architectural evolution of the project and was also entered into the **2026 YICTE** competition (the result of which hasn't been announced yet). Rather than simply expanding the game's content, development has focused on redesigning the underlying engine and introducing new systems for:

* Modular object composition
* Persistent NPC memory
* Emotional modelling
* Dynamic dialogue
* Universal object identification
* Persistent save and load functionality

---

## Acknowledgements

**[ADD ANY PEOPLE, TEACHERS, FRIENDS, LIBRARIES, OR RESOURCES YOU WOULD LIKE TO CREDIT HERE]**

Special thanks to **[ADD NAME / ORGANISATION]** for **[ADD CONTRIBUTION]**.
