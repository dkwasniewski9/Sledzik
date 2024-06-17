# Sledzik

Sledzik is an application designed to create a service and event log, where it periodically records CPU and memory usage of a specified application. Additionally, upon launching the executable file, it opens an application allowing users to modify the monitored application, delete entries, or view a graph depicting resource usage over time.

## Technologies Used

- **C#**: Primary programming language.
- **NUnit**: Used for testing.
- **Windows Service Installer**: Used for installing the service.
- **WPF (Windows Presentation Foundation)**: Used for the graphical user interface.

## Installation

1. Navigate to the `installer/Release` folder.
2. Run the installation file.
3. After installation, the application files will appear in the installed directory.

## Usage

The application interface is designed to be intuitive and straightforward. Upon installation and launch, users can perform the following actions:

- Monitor and record CPU and memory usage of specified applications.
- Modify the application being monitored.
- Delete entries from the event log.
- View a graphical representation of resource consumption over recorded entries.

## Notes

This project was developed as part of the "Programming in C#" course.
