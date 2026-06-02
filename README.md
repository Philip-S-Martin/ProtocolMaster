# ProtocolMaster

**ProtocolMaster** is a self-service scientific research execution platform for
running, monitoring, and logging time-based neurobiology experiments.

It was designed for research labs where experimental workflows are owned by
scientists, not software engineers. Researchers can define protocol schedules in
spreadsheet-oriented workflows, select those protocols through a desktop
application, and run experiments that coordinate hardware, media capture,
logging, and stimulus delivery.

Website: http://protocolmaster.philipm.net/

Author: Philip Martin  
License: MIT

---

## Why ProtocolMaster exists

Many behavioral neuroscience experiments require precise, repeatable stimulus
delivery over time. In practice, those experiments often depend on a mix of:

- spreadsheet-based protocol definitions
- cameras and microphones
- Arduino or other microcontroller-driven devices
- lab-specific file formats
- manual experiment setup
- hand-maintained logs and recordings
- custom hardware built for a specific research protocol

That creates a platform problem: scientists need flexibility, but the lab also
needs consistent execution, reproducible records, and a safe way to integrate
hardware.

ProtocolMaster was built to turn those workflows into a reusable research
execution platform.

The core idea is simple:

```text
Researcher-owned protocol definition
        ↓
ProtocolMaster interpreter
        ↓
Validated protocol event schedule
        ↓
Driver runtime
        ↓
Hardware / media / logging execution
```

Researchers get a workflow that fits how they already work. Technical users get
extension points for new file formats, protocol interpreters, device drivers,
and lab-specific hardware.

---

## What it does

ProtocolMaster supports end-to-end protocol execution for experiments involving
stimulus delivery over time.

User-facing capabilities include:

* protocol schedule automation
* Google Drive / Google Sheets-oriented workflow integration
* camera and microphone automation
* experiment logging
* live operator visibility
* reusable protocol selection flows
* modular support for lab-specific file structures
* extension points for custom interpreters and drivers

Technical capabilities include:

* interpreter extension system
* driver extension system
* schedule programming API
* prompt / user-selection integration for extensions
* C# extension support
* modular desktop architecture
* media capture integration
* pluggable protocol execution pipeline

---

## Researcher workflow

ProtocolMaster was designed around a practical lab workflow:

1. A researcher defines or updates a protocol schedule.
2. The protocol is stored locally or through a Google Drive / Sheets workflow.
3. ProtocolMaster loads the selected protocol file.
4. An interpreter converts the protocol into scheduled protocol events.
5. A driver executes those events against hardware, media systems, or other lab
   services.
6. The platform records logs and experiment outputs for later review.

This lets researchers keep using familiar configuration tools while moving the
execution path into a controlled platform.

The goal is not to force scientists into software-engineering workflows. The goal
is to give them a stable execution environment around the workflows they already
use.

---

## Architecture

ProtocolMaster separates protocol interpretation from protocol execution.

```text
Protocol source
  - Google Sheet / exported spreadsheet
  - local file
  - lab-specific format
        ↓
Interpreter
  - parses protocol source
  - generates ordered ProtocolEvent data
        ↓
Driver
  - executes protocol events
  - controls hardware / media / lab devices
        ↓
Runtime
  - monitoring
  - logging
  - cancellation
  - operator workflow
```

The core runtime is built around an `InterpretAndDriveProtocol` orchestration
object. It owns an interpreter manager and a driver manager, loads available
extensions, interprets a stream into protocol event data, and then runs the
selected driver against that event schedule.

This design keeps file-format concerns separate from execution concerns:

* interpreters decide how to read protocol definitions
* drivers decide how to execute protocol events
* the application coordinates selection, execution, cancellation, and visibility

---

## Extension model

ProtocolMaster uses a modular extension architecture so that labs can adapt the
platform without rewriting the core application.

The extension system supports:

* interpreter plugins for new protocol formats
* driver plugins for new execution targets
* metadata-driven extension selection
* prompt integration for user-selected values
* lifecycle management for running extensions

This matters because research labs often evolve in unpredictable ways. A new
experiment might require a new spreadsheet layout, a new stimulus device, or a
new sequence of hardware actions. ProtocolMaster was designed so those changes
could be isolated in extensions instead of becoming one-off forks of the
application.

---

## Google Drive and spreadsheet workflow

ProtocolMaster includes Google Drive integration so researchers can work with
protocol files through familiar cloud-based workflows.

The application can list available Google spreadsheet files, stream selected
files, and publish Drive revisions where appropriate. This supports a workflow
where researchers maintain protocol schedules in spreadsheet-like documents while
ProtocolMaster handles execution.

This pattern was important for non-technical scientific users:

```text
Google Sheets / Drive
        ↓
ProtocolMaster file selection
        ↓
protocol interpretation
        ↓
experiment execution
```

The result is a configuration-driven research platform rather than a workflow
that requires researchers to edit application code.

---

## Hardware and media integration

ProtocolMaster was designed for experiments involving physical stimulus delivery
and recorded behavioral data.

Supported integration patterns include:

* camera preview and recording
* microphone / audio recording
* video and audio capture
* Arduino and microcontroller-based extensions
* custom lab devices through driver plugins
* real-time protocol execution and monitoring
* logging of experiment execution

The system has been used in research workflows involving physical stimuli such as
touch, sound, light, neural stimulation, cameras, and implant-related experiment
equipment.

---

## Screenshots and media

The project website is available here:

[http://protocolmaster.philipm.net/](http://protocolmaster.philipm.net/)

---

## Repository layout

The repository contains the WPF application, core protocol runtime, extension
interfaces, and lab-specific integrations.

Important areas:

```text
ProtocolMasterCore/
  Protocol/
    InterpretAndDriveProtocol.cs
    ExtensionManager.cs
    Driver/
    Interpreter/
  Prompt/
  Utility/

ProtocolMasterWPF/
  Model/
    Google/
    MediaRecorder.cs
    Streamer.cs
  View/
  ViewModel/
  Theme/

McIntyreAFC/
  Example / lab-specific extension code
```

The exact structure may evolve, but the core split is:

* `ProtocolMasterCore`: protocol runtime, extension interfaces, driver and
  interpreter abstractions
* `ProtocolMasterWPF`: desktop application, user workflow, Google integration,
  media integration, views, and view models
* lab-specific projects: concrete drivers, interpreters, or research extensions

---

## Development background

ProtocolMaster began as a research software platform for the Neurobiology of
Memory Lab at the University of Texas at Dallas.

The original goal was to support experimental workflows where researchers needed
to define time-based protocols, run them consistently, control devices, record
behavioral data, and preserve experiment records without depending on manual
execution.

The platform was designed to be:

* usable by non-technical researchers
* extensible by technical users
* compatible with spreadsheet-driven configuration
* capable of coordinating hardware and media systems
* modular enough for other labs to adapt to their own file structures and devices

This project is part of a broader pattern in my work: building platforms that let
researchers express domain intent while the software handles execution,
integration, logging, and repeatability.

---

## Design principles

### Researcher-first workflow

The platform should fit the way scientists work. Researchers should be able to
configure and run experiments without understanding the internal driver or
interpreter architecture.

### Configuration over code

Protocol schedules and experiment definitions should be expressed through
researcher-owned files where possible. Code should be reserved for reusable
platform behavior and extension points.

### Separation of interpretation and execution

Parsing a protocol file and executing a protocol against hardware are different
concerns. ProtocolMaster keeps those layers separate so each can evolve
independently.

### Extensibility over one-off scripts

Labs often need custom behavior. ProtocolMaster supports extension-based
customization rather than encouraging isolated scripts for each experiment.

### Operator visibility

Running experiments should be observable. The platform includes monitoring,
logging, and media capture workflows so researchers can understand what happened
during execution.

---

## Example workflow

A typical experiment flow looks like this:

```text
1. Researcher prepares protocol schedule
2. Protocol file is saved locally or in Google Drive
3. ProtocolMaster loads available protocol files
4. Researcher selects interpreter and driver
5. Interpreter generates protocol event schedule
6. Driver executes events against lab hardware or services
7. ProtocolMaster records logs, video, audio, and execution output
8. Researcher reviews resulting experiment records
```

This makes ProtocolMaster less like a single experiment script and more like a
small internal research platform.

---

## Technical notes

ProtocolMaster is primarily written in C# using WPF for the desktop application.

The system uses:

* C#
* WPF
* .NET
* Google Drive APIs
* Windows media capture APIs
* Managed Extensibility Framework-style extension composition
* Arduino / hardware integration through lab-specific drivers
* MVVM-style desktop application structure

The extension architecture allows protocol interpreters and execution drivers to
be composed into the application at runtime.

---

## Status

ProtocolMaster is an open-source research engineering project.

It was built for real laboratory workflows and should be read as a research
platform / applied systems project rather than a polished commercial desktop
product.

Areas that may need modernization before broader reuse:

* updated setup and installation documentation
* clearer extension authoring guides
* screenshots and demo media
* automated tests around interpreter / driver behavior
* packaging updates for modern .NET workflows
* additional examples for new lab integrations

---

## Contact

For documentation requests, integration support, or questions about
ProtocolMaster and its extension model, contact:

Philip Martin
[philip@philipm.net](mailto:philip@philipm.net)

---

## License

This project is open source under the MIT license.

